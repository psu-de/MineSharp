using System.Diagnostics;
using fNbt;
using MineSharp.Bot.Chat;
using MineSharp.ChatComponent;
using MineSharp.ChatComponent.Components;
using MineSharp.Commands;
using MineSharp.Core;
using MineSharp.Core.Common;
using MineSharp.Core.Events;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using NLog;
using ChatPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatPacket;
using SBChatMessage = MineSharp.Protocol.Packets.Serverbound.Play.ChatPacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;

namespace MineSharp.Bot.Plugins;

/// <summary>
///     ChatPlugin handles chat packets and provides methods to send chat messages and commands.
/// </summary>
public class ChatPlugin : Plugin
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly Uuid chatSession = new Guid();

    private readonly Task<DeclareCommandsPacket> initDeclareCommandsPacket;

    private readonly LastSeenMessageCollector? messageCollector;

    private CommandTree? commandTree;
    private int messageCount;
    private byte[]? precedingSignature; // only used for 1.19.2 chat signing

    /// <summary>
    ///     Create a new ChatPlugin instance.
    /// </summary>
    /// <param name="bot"></param>
    public ChatPlugin(MineSharpBot bot) : base(bot)
    {
        messageCollector = Bot.Data.Version.Protocol switch
        {
            >= ProtocolVersion.V_1_19_3 => new LastSeenMessageCollector1193(),
            >= ProtocolVersion.V_1_19_2 => new LastSeenMessageCollector1192(),
            _ => null
        };

        Bot.Client.On<PlayerChatPacket>(HandleChatMessagePacket);
        Bot.Client.On<ChatPacket>(HandleChat);
        Bot.Client.On<SystemChatMessagePacket>(HandleSystemChatMessage);
        Bot.Client.On<DisguisedChatMessagePacket>(HandleDisguisedChatMessage);

        initDeclareCommandsPacket = Bot.Client.WaitForPacket<DeclareCommandsPacket>();
    }

    /// <summary>
    ///     Fired when a chat message is received
    /// </summary>
    public AsyncEvent<MineSharpBot, Uuid?, MineSharp.ChatComponent.Chat, ChatMessageType> OnChatMessageReceived = new();

    /// <inheritdoc />
    protected override async Task Init()
    {
        if (Bot.Data.Version.Protocol >= ProtocolVersion.V_1_19_3 && Bot.Session.OnlineSession)
        {
            await Bot.Client.SendPacket(
                new PlayerSessionPacket(
                    chatSession,
                    new DateTimeOffset(Bot.Client.Session.Certificate!.ExpiresAt).ToUnixTimeMilliseconds(),
                    Bot.Client.Session.Certificate.Keys.PublicKey,
                    Bot.Client.Session.Certificate.PublicKeySignatureV2
                ));
        }

        await HandleDeclareCommandsPacket(await initDeclareCommandsPacket.WaitAsync(Bot.CancellationToken)).WaitAsync(Bot.CancellationToken);
    }

    /// <summary>
    ///     Send a chat message to the Minecraft server.
    ///     If the message starts with a '/', a command is sent instead.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public Task SendChat(string message)
    {
        if (Bot.Data.Version.Protocol >= ProtocolVersion.V_1_19
            && message.StartsWith('/'))
        {
            return SendCommand(message.Substring(1));
        }

        // signed chat messages since 1.19
        return Bot.Data.Version.Protocol switch
        {
            // 1.19.3
            >= ProtocolVersion.V_1_19_3 => SendChat1_19_3(message),

            // 1.19.2
            >= ProtocolVersion.V_1_19_2 => SendChat1_19_2(message),

            // 1.19.1
            >= ProtocolVersion.V_1_19 => SendChat1_19_1(message),

            // Literally every version before.
            _ => Bot.Client.SendPacket(new SBChatMessage(message))
        };
    }

    /// <summary>
    ///     Send a command to the server.
    ///     Can only be used for versions &gt;= 1.19
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public Task SendCommand(string command)
    {
        var arguments = new List<(string, string)>();
        if (Bot.Session.OnlineSession && Bot.Session.Certificate != null)
        {
            arguments = CollectCommandArgumentsRecursive(commandTree!.RootIndex, command);
        }

        return Bot.Data.Version.Protocol switch
        {
            // 1.19.3
            >= ProtocolVersion.V_1_19_3 => SendCommand1_19_3(command, arguments),

            // 1.19.2
            >= ProtocolVersion.V_1_19_2 => SendCommand1_19_2(command, arguments),

            // 1.19.1
            >= ProtocolVersion.V_1_19 => SendCommand1_19_1(command, arguments),

            // Literally every version before
            _ => SendChat("/" + command)
        };
    }

    private static readonly Identifier MessageIdentifier = Identifier.Parse("minecraft:message");

    /*
     * Thanks to Minecraft-Console-Client
     * https://github.com/MCCTeam/Minecraft-Console-Client
     *
     * This Method uses a lot of code from MinecraftClient/Protocol/Handlers/Packet/s2c/DeclareCommands.cs from MCC.
     */
    private List<(string, string)> CollectCommandArgumentsRecursive(int nodeIdx, string command,
                                                                    List<(string, string)>? arguments = null)
    {
        if (commandTree == null)
        {
            throw new InvalidOperationException();
        }

        arguments ??= new();

        var node = commandTree.Nodes[nodeIdx];
        var lastArg = command;

        switch (node.Flags & 0x03)
        {
            case 0: // root
                break;
            case 1: // literal
                {
                    var arg = command.Split(' ', 2);
                    if (!(arg.Length == 2 && node.Name! == arg[0]))
                    {
                        return arguments;
                    }

                    lastArg = arg[1];
                }
                break;
            case 2: // argument
                {
                    var argCnt = node.Parser?.GetArgumentCount() ?? 1;
                    var arg = command.Split(' ', argCnt + 1);
                    if ((node.Flags & 0x04) > 0)
                    {
                        if (node.Parser != null && node.Parser.GetName() == MessageIdentifier)
                        {
                            arguments.Add((node.Name!, command));
                        }
                    }

                    if (arg.Length != argCnt + 1)
                    {
                        return arguments;
                    }

                    lastArg = arg[^1];
                }
                break;
        }

        while (commandTree.Nodes[nodeIdx].RedirectNode >= 0)
        {
            nodeIdx = commandTree.Nodes[nodeIdx].RedirectNode;
        }

        return commandTree.Nodes[nodeIdx]
                          .Children
                          .Aggregate(arguments, (cur, idx) => CollectCommandArgumentsRecursive(idx, lastArg, cur));
    }


    private Task SendChat1_19_1(string message)
    {
        byte[] signature;
        var timestamp = DateTimeOffset.UtcNow;
        var salt = ChatSignature.GenerateSalt();

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            signature = Array.Empty<byte>();
        }
        else
        {
            signature = ChatSignature.SignChat1_19_1(
                Bot.Client.Session.Certificate!.RsaPrivate,
                message,
                Bot.Client.Session.Uuid,
                timestamp,
                salt);
        }

        return Bot.Client.SendPacket(
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

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            return Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    false));
        }

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        var salt = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_1(
                Bot.Session.Certificate.RsaPrivate,
                argument,
                Bot.Session.Uuid,
                timestamp,
                salt);

            signatures.Add(new(name, signature));
        }

        return Bot.Client.SendPacket(
            new ChatCommandPacket(
                command,
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signatures.ToArray(),
                false));
    }

    private Task SendChat1_19_2(string message)
    {
        var collector = (LastSeenMessageCollector1192)messageCollector!;
        var messages = collector.AcknowledgeMessages();
        var timestamp = DateTimeOffset.UtcNow;
        byte[] signature;
        var salt = ChatSignature.GenerateSalt();

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            signature = Array.Empty<byte>();
            salt = 0;
        }
        else
        {
            //salt = ChatSignature.GenerateSalt();
            signature = ChatSignature.SignChat1_19_2(
                Bot.Session.Certificate.RsaPrivate,
                message,
                Bot.Session.Uuid,
                timestamp,
                salt,
                messages,
                precedingSignature);

            precedingSignature = signature;
        }

        return Bot.Client.SendPacket(new SBChatMessagePacket(
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
        var collector = (LastSeenMessageCollector1192)messageCollector!;
        var acknowledged = collector.AcknowledgeMessages();

        var timestamp = DateTimeOffset.UtcNow;

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            return Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    false,
                    acknowledged.Select(x => x.ToProtocolMessage()).ToArray(),
                    null));
        }

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        var salt = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_2(
                Bot.Session.Certificate.RsaPrivate,
                argument,
                Bot.Session.Uuid,
                timestamp,
                salt,
                acknowledged,
                precedingSignature);

            signatures.Add(new(name, signature));
        }

        return Bot.Client.SendPacket(
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
        var collector = (LastSeenMessageCollector1193)messageCollector!;
        var acknowledgedBitfield = collector.Collect(out var count, out var acknowledged);

        var timestamp = DateTimeOffset.UtcNow;

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            return Bot.Client.SendPacket(
                new SBChatMessagePacket(
                    message,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    null,
                    count, acknowledgedBitfield));
        }

        var salt = ChatSignature.GenerateSalt();
        var signature = ChatSignature.SignChat1_19_3(
            Bot.Data,
            Bot.Client.Session.Certificate!.RsaPrivate,
            message,
            Bot.Session.Uuid,
            chatSession,
            messageCount++,
            salt,
            timestamp,
            acknowledged);

        return Bot.Client.SendPacket(
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
        var collector = (LastSeenMessageCollector1193)messageCollector!;
        var acknowledgedBitfield = collector.Collect(out var count, out var acknowledged);

        var timestamp = DateTimeOffset.UtcNow;

        if (!Bot.Session.OnlineSession || Bot.Session.Certificate == null)
        {
            return Bot.Client.SendPacket(
                new ChatCommandPacket(
                    command,
                    timestamp.ToUnixTimeMilliseconds(),
                    0,
                    Array.Empty<ChatCommandPacket.ArgumentSignature>(),
                    count,
                    acknowledgedBitfield));
        }

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        var salt = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_3(
                Bot.Data,
                Bot.Session.Certificate.RsaPrivate,
                argument,
                Bot.Session.Uuid,
                chatSession,
                messageCount++,
                salt,
                timestamp,
                acknowledged);

            signatures.Add(new(name, signature));
        }

        return Bot.Client.SendPacket(
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
            commandTree = CommandTree.Parse(packet.RawBuffer, Bot.Data);
        }
        catch (Exception e)
        {
            Logger.Error($"Could not parse command tree: {e}");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method to receive chat messages after 1.19
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    private Task HandleChatMessagePacket(PlayerChatPacket packet)
    {
        try
        {
            // TODO: Advanced chat stuff like filtering and signature verification.

            if (packet.Body is PlayerChatPacket.V11923Body body && body.Signature != null)
            {
                if (messageCollector is LastSeenMessageCollector1192 collector1192)
                {
                    collector1192.Push(new(true, body.Sender, body.Signature));
                    if (collector1192.PendingAcknowledgements++ > 64)
                    {
                        var messages = collector1192.AcknowledgeMessages().Select(x => x.ToProtocolMessage()).ToArray();
                        Bot.Client.SendPacket(new MessageAcknowledgementPacket(messages, null));
                    }
                }
                else if (messageCollector is LastSeenMessageCollector1193 collector1193)
                {
                    var count = collector1193.ResetCount();
                    if (count > 0)
                    {
                        Bot.Client.SendPacket(new MessageAcknowledgementPacket(count));
                    }
                }
            }

            (var uuid,
                var messageType,
                var sender,
                var content,
                var target) = packet.Body switch
                {
                    PlayerChatPacket.V119Body v19 => (
                        v19.Sender,
                        v19.MessageType,
                        v19.SenderName,
                        v19.UnsignedChat ?? v19.SignedChat,
                        null),

                    PlayerChatPacket.V11923Body v192 => (
                        v192.Sender,
                        v192.Type,
                        v192.NetworkName,
                        v192.FormattedMessage ?? v192.UnsignedContent ?? new TextComponent(v192.PlainMessage),
                        v192.NetworkTargetName),

                    _ => throw new UnreachableException()
                };

            (var chat, var type)
                = GetChatMessageTypeFromRegistry(messageType, sender, content, target);

            HandleChatInternal(uuid, chat, type);
        }
        catch (Exception e)
        {
            Logger.Error("Error in chat message: " + e);
        }

        return Task.CompletedTask;
    }

    private Task HandleSystemChatMessage(SystemChatMessagePacket packet)
    {
        return packet switch
        {
            SystemChatMessagePacket.Before192 before192
                => HandleChatInternal(null, before192.Message, (ChatMessageType)before192.ChatType),

            SystemChatMessagePacket.Since192 since192
                => HandleChatInternal(null,
                                      since192.Message,
                                      since192.IsOverlay ? ChatMessageType.GameInfo : ChatMessageType.SystemMessage),

            _ => throw new UnreachableException()
        };
    }

    private Task HandleDisguisedChatMessage(DisguisedChatMessagePacket packet)
    {
        (var chat, var type)
            = GetChatMessageTypeFromRegistry(packet.ChatType, packet.Name, packet.Message, packet.Target);

        return HandleChatInternal(
            null,
            chat,
            type);
    }

    /// <summary>
    ///     Method to receive chat messages before 1.19
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    private Task HandleChat(ChatPacket packet)
    {
        // TODO: packet.Message can be a JSON formatted text component
        return HandleChatInternal(packet.Sender, packet.Message, (ChatMessageType)packet.Position);
    }

    private Task HandleChatInternal(Uuid? sender, string message, ChatMessageType type)
    {
        return OnChatMessageReceived.Dispatch(Bot, sender, new TextComponent(message), type);
    }

    private Task HandleChatInternal(Uuid? sender, ChatComponent.Chat message, ChatMessageType type)
    {
        return OnChatMessageReceived.Dispatch(Bot, sender, message, type);
    }

    private (ChatComponent.Chat, ChatMessageType) GetChatMessageTypeFromRegistry(
        int index,
        ChatComponent.Chat? sender,
        ChatComponent.Chat? content,
        ChatComponent.Chat? target)
    {
        var entry = Bot.Registry["minecraft:chat_type"]["value"][index];
        var name = Identifier.Parse(entry["name"]!.StringValue!);
        var element = entry["element"];
        var styleCompound = element["chat"]["style"];
        var translation = element["chat"]["translation_key"].StringValue;
        var parameters = (element["chat"]["parameters"] as NbtList)!.Select(x => x.StringValue).ToArray();

        var with = parameters.Select(paramName =>
        {
            var param = paramName switch
            {
                "sender" => sender,
                "content" => content,
                "target" => target,
                _ => throw new InvalidOperationException(
                    $"Unknown parameter: '{paramName}'") // TODO: There are also team_name, etc...
            };

            if (param is null)
            {
                Logger.Warn("Chat processing: Parameter {name} is null for translation {translation}", paramName,
                            translation);
            }

            return param ?? new TextComponent("");
        });

        var style = styleCompound is not null
            ? Style.Parse(styleCompound)
            : Style.DefaultStyle;

        var translatable = new TranslatableComponent(
            translation,
            with.ToArray(),
            style);

        return (translatable, GetChatMessageType(name));
    }

    private static ChatMessageType GetChatMessageType(Identifier message)
    {
        return message.ToString() switch
        {
            "minecraft:chat" => ChatMessageType.Chat,
            "minecraft:emote_command" => ChatMessageType.Emote,
            "minecraft:say_command" => ChatMessageType.SayCommand,
            "minecraft:team_msg_command_incoming" => ChatMessageType.TeamMessage,
            "minecraft:msg_command_incoming" => ChatMessageType.SayCommand,
            _ => UnknownChatMessage(message)
        };
    }

    private static ChatMessageType UnknownChatMessage(Identifier msg)
    {
        Logger.Debug("Unknown chat message type {message}.", msg);
        return ChatMessageType.Raw;
    }
}
