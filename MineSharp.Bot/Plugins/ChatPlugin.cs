using MineSharp.Bot.Chat;
using MineSharp.Commands;
using MineSharp.Core.Common;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System.Diagnostics;
using MineSharp.Data;
using NLog;
using ChatPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatPacket;
using SBChatMessage = MineSharp.Protocol.Packets.Serverbound.Play.ChatPacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;

namespace MineSharp.Bot.Plugins;

/// <summary>
/// ChatPlugin handles chat packets and provides methods to send chat messages and commands.
/// </summary>
public class ChatPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly LastSeenMessageCollector? _messageCollector;
    private readonly UUID                      _chatSession        = new Guid();
    private          int                       _messageCount       = 0;
    private          byte[]?                   _precedingSignature = null; // only used for 1.19.2 chat signing

    private CommandTree? _commandTree = null;

    /// <summary>
    /// Fired when a chat message is received
    /// </summary>
    public event Events.BotChatMessageEvent? OnChatMessageReceived;

    private Task<DeclareCommandsPacket> initDeclareCommandsPacket;

    /// <summary>
    /// Create a new ChatPlugin instance.
    /// </summary>
    /// <param name="bot"></param>
    public ChatPlugin(MineSharpBot bot) : base(bot)
    {
        this._messageCollector = this.Bot.Data.Version.Protocol switch
        {
            >= ProtocolVersion.V_1_19_3 => new LastSeenMessageCollector1_19_3(),
            >= ProtocolVersion.V_1_19_2 => new LastSeenMessageCollector1_19_2(),
            _                           => null
        };

        this.Bot.Client.On<PlayerChatPacket>(this.HandleChatMessagePacket);
        this.Bot.Client.On<ChatPacket>(this.HandleChat);
        this.Bot.Client.On<SystemChatMessagePacket>(this.HandleSystemChatMessage);
        this.Bot.Client.On<DisguisedChatMessagePacket>(this.HandleDisguisedChatMessage);
        
        this.initDeclareCommandsPacket = this.Bot.Client.WaitForPacket<DeclareCommandsPacket>();
    }

    /// <inheritdoc />
    protected override async Task Init()
    {
        if (this.Bot.Data.Version.Protocol >= ProtocolVersion.V_1_19_3 && this.Bot.Session.OnlineSession)
        {
            await this.Bot.Client.SendPacket(
                new PlayerSessionPacket(
                    this._chatSession,
                    new DateTimeOffset(this.Bot.Client.Session.Certificate!.ExpiresAt).ToUnixTimeMilliseconds(),
                    this.Bot.Client.Session.Certificate.Keys.PublicKey,
                    this.Bot.Client.Session.Certificate.PublicKeySignatureV2
                ));
        }

        await HandleDeclareCommandsPacket(await this.initDeclareCommandsPacket);
    }

    /// <summary>
    /// Send a chat message to the Minecraft server.
    /// If the message starts with a '/', a command is sent instead.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task SendChat(string message)
    {
        if (this.Bot.Data.Version.Protocol >= ProtocolVersion.V_1_19
         && message.StartsWith('/'))
        {
            return SendCommand(message.Substring(1));
        }

        // signed chat messages since 1.19
        return this.Bot.Data.Version.Protocol switch
        {
            // 1.19.3
            >= ProtocolVersion.V_1_19_3 => this.SendChat1_19_3(message),
            // 1.19.2
            >= ProtocolVersion.V_1_19_2 => this.SendChat1_19_2(message),
            // 1.19.1
            >= ProtocolVersion.V_1_19 => this.SendChat1_19_1(message),
            // Literally every version before.
            _ => this.Bot.Client.SendPacket(new SBChatMessage(message))
        };
    }

    /// <summary>
    /// Send a command to the server.
    /// Can only be used for versions &gt;= 1.19
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public Task SendCommand(string command)
    {
        var arguments = new List<(string, string)>();
        if (this.Bot.Session.OnlineSession && this.Bot.Session.Certificate != null)
        {
            arguments = CollectCommandArgumentsRecursive(this._commandTree!.RootIndex, command);
        }

        return this.Bot.Data.Version.Protocol switch
        {
            // 1.19.3
            >= ProtocolVersion.V_1_19_3 => this.SendCommand1_19_3(command, arguments),
            // 1.19.2
            >= ProtocolVersion.V_1_19_2 => this.SendCommand1_19_2(command, arguments),
            // 1.19.1
            >= ProtocolVersion.V_1_19 => this.SendCommand1_19_1(command, arguments),
            // Literally every version before
            _ => this.SendChat("/" + command)
        };
    }


    /*
     * Thanks to Minecraft-Console-Client
     * https://github.com/MCCTeam/Minecraft-Console-Client
     *
     * This Method uses a lot of code from MinecraftClient/Protocol/Handlers/Packet/s2c/DeclareCommands.cs from MCC.
     */
    private List<(string, string)> CollectCommandArgumentsRecursive(int nodeIdx, string command, List<(string, string)>? arguments = null)
    {
        if (this._commandTree == null)
            throw new InvalidOperationException();

        arguments ??= new List<(string, string)>();

        var node    = this._commandTree.Nodes[nodeIdx];
        var lastArg = command;

        switch (node.Flags & 0x03)
        {
            case 0: // root
                break;
            case 1: // literal
            {
                string[] arg = command.Split(' ', 2);
                if (!(arg.Length == 2 && node.Name! == arg[0]))
                    return arguments;
                lastArg = arg[1];
            }
                break;
            case 2: // argument
            {
                int      argCnt = node.Parser?.GetArgumentCount() ?? 1;
                string[] arg    = command.Split(' ', argCnt + 1);
                if ((node.Flags & 0x04) > 0)
                {
                    if (node.Parser != null && node.Parser.GetName() == "minecraft:message")
                        arguments.Add((node.Name!, command));
                }

                if (arg.Length != argCnt + 1)
                    return arguments;
                lastArg = arg[^1];
            }
                break;
        }

        while (this._commandTree.Nodes[nodeIdx].RedirectNode >= 0)
            nodeIdx = this._commandTree.Nodes[nodeIdx].RedirectNode;

        return this._commandTree.Nodes[nodeIdx]
                   .Children
                   .Aggregate(arguments, (cur, idx) => this.CollectCommandArgumentsRecursive(idx, lastArg, cur));
    }


    private Task SendChat1_19_1(string message)
    {
        byte[] signature;
        var    timestamp = DateTimeOffset.UtcNow;
        var    salt      = ChatSignature.GenerateSalt();

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            signature = Array.Empty<byte>();
        }
        else
        {
            signature = ChatSignature.SignChat1_19_1(
                this.Bot.Client.Session.Certificate!.RsaPrivate,
                message,
                this.Bot.Client.Session.UUID,
                timestamp,
                salt);
        }

        return this.Bot.Client.SendPacket(
            new SBChatMessagePacket(
                message,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signature,
                false));
    }

    private Task SendCommand1_19_1(string command, List<(string, string)> arguments)
    {
        var timestamp = DateTimeOffset.UtcNow;

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            return this.Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    false));
        }

        var  signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt       = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_1(
                this.Bot.Session.Certificate.RsaPrivate,
                argument,
                this.Bot.Session.UUID,
                timestamp,
                salt);

            signatures.Add(new ChatCommandPacket.ArgumentSignature(name, signature));
        }

        return this.Bot.Client.SendPacket(
            new ChatCommandPacket(
                command,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signatures.ToArray(),
                false));
    }

    private Task SendChat1_19_2(string message)
    {
        var    collector = (LastSeenMessageCollector1_19_2)this._messageCollector!;
        var    messages  = collector.AcknowledgeMessages();
        var    timestamp = DateTimeOffset.UtcNow;
        byte[] signature;
        long   salt = ChatSignature.GenerateSalt();

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            signature = Array.Empty<byte>();
            salt      = 0;
        }
        else
        {
            //salt = ChatSignature.GenerateSalt();
            signature = ChatSignature.SignChat1_19_2(
                this.Bot.Session.Certificate.RsaPrivate,
                message,
                this.Bot.Session.UUID,
                timestamp,
                salt,
                messages,
                this._precedingSignature);

            this._precedingSignature = signature;
        }

        return this.Bot.Client.SendPacket(new SBChatMessagePacket(
            message,
            timestamp.ToUnixTimeMilliseconds(),
            salt,
            signature,
            false,
            messages.Select(x => x.ToProtocolMessage()).ToArray(),
            null
        ));
    }

    private Task SendCommand1_19_2(string command, List<(string, string)> arguments)
    {
        var collector    = (LastSeenMessageCollector1_19_2)this._messageCollector!;
        var acknowledged = collector.AcknowledgeMessages();

        var timestamp = DateTimeOffset.UtcNow;

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            return this.Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    false,
                    acknowledged.Select(x => x.ToProtocolMessage()).ToArray(),
                    null));
        }

        var  signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt       = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_2(
                this.Bot.Session.Certificate.RsaPrivate,
                argument,
                this.Bot.Session.UUID,
                timestamp,
                salt,
                acknowledged,
                this._precedingSignature);

            signatures.Add(new ChatCommandPacket.ArgumentSignature(name, signature));
        }

        return this.Bot.Client.SendPacket(
            new ChatCommandPacket(
                command,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signatures.ToArray(),
                false,
                acknowledged.Select(x => x.ToProtocolMessage()).ToArray(),
                null));
    }

    private Task SendChat1_19_3(string message)
    {
        var    collector            = (LastSeenMessageCollector1_19_3)this._messageCollector!;
        byte[] acknowledgedBitfield = collector.Collect(out var count, out var acknowledged);

        var timestamp = DateTimeOffset.UtcNow;

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            return this.Bot.Client.SendPacket(
                new SBChatMessagePacket(
                    message,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    null,
                    count, acknowledgedBitfield));
        }

        var salt = ChatSignature.GenerateSalt();
        byte[] signature = ChatSignature.SignChat1_19_3(
            this.Bot.Data,
            this.Bot.Client.Session.Certificate!.RsaPrivate,
            message,
            this.Bot.Session.UUID,
            this._chatSession,
            this._messageCount++,
            salt,
            timestamp,
            acknowledged);

        return this.Bot.Client.SendPacket(
            new SBChatMessagePacket(
                message,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signature,
                count,
                acknowledgedBitfield));
    }

    private Task SendCommand1_19_3(string command, List<(string, string)> arguments)
    {
        var    collector            = (LastSeenMessageCollector1_19_3)this._messageCollector!;
        byte[] acknowledgedBitfield = collector.Collect(out var count, out var acknowledged);

        var timestamp = DateTimeOffset.UtcNow;

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            return this.Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    count,
                    acknowledgedBitfield));
        }

        var  signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt       = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_3(
                this.Bot.Data,
                this.Bot.Session.Certificate.RsaPrivate,
                argument,
                this.Bot.Session.UUID,
                this._chatSession,
                this._messageCount++,
                salt,
                timestamp,
                acknowledged);

            signatures.Add(new ChatCommandPacket.ArgumentSignature(name, signature));
        }

        return this.Bot.Client.SendPacket(
            new ChatCommandPacket(
                command,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signatures.ToArray(),
                count,
                acknowledgedBitfield));
    }

    private Task HandleDeclareCommandsPacket(DeclareCommandsPacket packet)
    {
        try
        {
            this._commandTree = CommandTree.Parse(packet.RawBuffer, this.Bot.Data);
        }
        catch (Exception e)
        {
            Logger.Error($"Could not parse command tree: {e}");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Method to receive chat messages after 1.19
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    private Task HandleChatMessagePacket(PlayerChatPacket packet)
    {
        try
        {
            // TODO: Advanced chat stuff like filtering and signature verification.

            if (packet.Body is PlayerChatPacket.V1_19_2_3Body body && body.Signature != null)
            {
                if (this._messageCollector is LastSeenMessageCollector1_19_2 collector1_19_2)
                {
                    collector1_19_2.Push(new AcknowledgedMessage(true, body.Sender, body.Signature));
                    if (collector1_19_2.PendingAcknowledgements++ > 64)
                    {
                        var messages = collector1_19_2.AcknowledgeMessages().Select(x => x.ToProtocolMessage()).ToArray();
                        this.Bot.Client.SendPacket(new MessageAcknowledgementPacket(messages, null));
                    }
                }
                else if (this._messageCollector is LastSeenMessageCollector1_19_3 collector1_19_3)
                {
                    int count = collector1_19_3.ResetCount();
                    if (count > 0)
                        this.Bot.Client.SendPacket(new MessageAcknowledgementPacket(count));
                }
            }

            (UUID sender, string message, int type) = packet.Body switch
            {
                PlayerChatPacket.V1_19Body v19       => (v19.Sender, v19.SignedChat, v19.MessageType),
                PlayerChatPacket.V1_19_2_3Body v19_2 => (v19_2.Sender, v19_2.UnsignedContent ?? v19_2.FormattedMessage ?? v19_2.PlainMessage, v19_2.Type),
                _                                    => throw new UnreachableException()
            };

            var chatType = GetChatMessageTypeFromRegistry(type);
            this.HandleChatInternal(sender, message, chatType);
        }
        catch (Exception e)
        {
            Logger.Error("Error in chat message: " + e);
        }

        return Task.CompletedTask;
    }

    private Task HandleSystemChatMessage(SystemChatMessagePacket packet)
    {
        ChatMessageType type;
        if (packet.ChatType.HasValue)
        {
            type = (ChatMessageType)packet.ChatType;
        }
        else
        {
            type = packet.IsOverlay!.Value
                ? ChatMessageType.GameInfo
                : ChatMessageType.SystemMessage;
        }
        if (packet.Message != null)
        {
            this.HandleChatInternal(null, packet.Message, type);
        }
        else
        {
            this.HandleChatInternal(null, packet.Content, type);
        }
        return Task.CompletedTask;
    }

    private Task HandleDisguisedChatMessage(DisguisedChatMessagePacket packet)
    {
        this.HandleChatInternal(
            null,
            packet.Message,
            this.GetChatMessageTypeFromRegistry(packet.ChatType),
            packet.Target ?? packet.Name);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Method to receive chat messages before 1.19
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private Task HandleChat(ChatPacket packet)
    {
        this.HandleChatInternal(packet.Sender, packet.Message, (ChatMessageType)packet.Position);
        return Task.CompletedTask;
    }

    private void HandleChatInternal(UUID? sender, string message, ChatMessageType type, string? senderName = null)
    {
        this.OnChatMessageReceived?.Invoke(this.Bot, sender, new ChatComponent.Chat(message, this.Bot.Data), type, senderName);
    }

    private void HandleChatInternal(UUID? sender, ChatComponent.Chat message, ChatMessageType type, string? senderName = null)
    {
        this.OnChatMessageReceived?.Invoke(this.Bot, sender, message, type, senderName);
    }

    private ChatMessageType GetChatMessageTypeFromRegistry(int index)
    {
        var val = this.Bot.Registry["minecraft:chat_type"]["value"][index]["name"]!.StringValue!;
        return GetChatMessageType(val);
    }

    private static ChatMessageType GetChatMessageType(string message)
    {
        return message switch
        {
            "minecraft:chat"                      => ChatMessageType.Chat,
            "minecraft:emote_command"             => ChatMessageType.Emote,
            "minecraft:say_command"               => ChatMessageType.SayCommand,
            "minecraft:team_msg_command_incoming" => ChatMessageType.TeamMessage,
            "minecraft:msg_command_incoming"      => ChatMessageType.SayCommand,
            _                                     => UnknownChatMessage(message)
        };
    }

    private static ChatMessageType UnknownChatMessage(string msg)
    {
        Logger.Debug("Unknown chat message type {message}.", msg);
        return ChatMessageType.Raw;
    }
}
