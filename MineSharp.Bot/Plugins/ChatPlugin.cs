using MineSharp.Bot.Chat;
using MineSharp.Commands;
using MineSharp.Commands.Parser;
using MineSharp.Core.Common;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol;

using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;
using CBChatMessagePacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatMessagePacket;

namespace MineSharp.Bot.Plugins;

public class ChatPlugin : Plugin
{
    private readonly LastSeenMessageCollector? _messageCollector;
    private readonly UUID _chatSession = new Guid();
    private int _messageCount = 0;
    private byte[]? _precedingSignature = null; // only used for 1.19.2 chat signing

    private CommandTree? _commandTree = null;

    public event Events.BotChatMessageEvent? OnChatMessageReceived;

    public ChatPlugin(MinecraftBot bot) : base(bot)
    {
        this._messageCollector = this.Bot.Data.Protocol.Version switch {
            >= ProtocolVersion.V_1_19_3 => new LastSeenMessageCollector1_19_3(),
            >= ProtocolVersion.V_1_19_2 => new LastSeenMessageCollector1_19_2(),
            _ => null
        };
        
        this.Bot.Client.On<DeclareCommandsPacket>(this.HandleDeclareCommandsPacket);
        this.Bot.Client.On<CBChatMessagePacket>(this.HandleChatMessagePacket);
    }

    protected override async Task Init()
    {
        if (this.Bot.Data.Features.Supports("useChatSessions"))
        {
            await this.Bot.Client.SendPacket(
                new PlayerSessionPacket(
                    this._chatSession,
                    new DateTimeOffset(this.Bot.Client.Session.Certificate!.ExpiresAt).ToUnixTimeMilliseconds(),
                    this.Bot.Client.Session.Certificate.Keys.PublicKey,
                    this.Bot.Client.Session.Certificate.PublicKeySignatureV2
                ));
        }
    }

    public Task SendChat(string message)
    {
        if (message.StartsWith('/') 
            && this.Bot.Data.Protocol.Version >= ProtocolVersion.V_1_19)
        {
            return SendCommand(message.Substring(1));
        }
        
        // signed chat messages since 1.19
        if (this.Bot.Data.Features.Supports("useChatSessions")) // 1.19.3
        {
            return SendChat1_19_3(message);
        } 
        
        if (this.Bot.Data.Features.Supports("chainedChatWithHashing")) // 1.19.2
        {
            return SendChat1_19_2(message);
        }

        if (this.Bot.Data.Features.Supports("signedChat")) // 1.19.1
        {
            return SendChat1_19_1(message);
        }
        
        // Literally every version before.
        return this.Bot.Client.SendPacket(new SBChatMessagePacket(message));
    }
    
    public Task SendCommand(string command)
    {
        var arguments = new List<(string, string)>();
        if (this.Bot.Session.OnlineSession && this.Bot.Session.Certificate != null)
        {
            arguments = CollectCommandArgumentsRecursive(this._commandTree!.RootIndex, command);
        }

        if (this.Bot.Data.Features.Supports("useChatSessions")) // 1.19.3
        {
            return SendCommand1_19_3(command, arguments);
        } 
        
        if (this.Bot.Data.Features.Supports("chainedChatWithHashing")) // 1.19.2
        {
            return SendCommand1_19_2(command, arguments);
        }

        if (this.Bot.Data.Features.Supports("signedChat")) // 1.19.1
        {
            return SendCommand1_19_1(command, arguments);
        }

        return SendChat("/" + command);
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
        
        var node = this._commandTree.Nodes[nodeIdx];
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
                int argCnt = (node.Parser == null) ? 1 : node.Parser.GetArgumentCount();
                string[] arg = command.Split(' ', argCnt + 1);
                if ((node.Flags & 0x04) > 0)
                {
                    if (node.Parser != null && node.Parser.GetName() == "minecraft:message")
                        arguments.Add((node.Name!, command));
                }
                if (!(arg.Length == argCnt + 1))
                    return arguments;
                lastArg = arg[^1];
            }
                break;
        }

        while (this._commandTree.Nodes[nodeIdx].RedirectNode >= 0)
            nodeIdx = this._commandTree.Nodes[nodeIdx].RedirectNode;

        foreach (int childIdx in this._commandTree.Nodes[nodeIdx].Children)
            arguments = CollectCommandArgumentsRecursive(childIdx, lastArg, arguments);

        return arguments;
    }
    
    /*
     * Thanks to Minecraft-Console-Client
     * https://github.com/MCCTeam/Minecraft-Console-Client
     * 
     * This Method uses a lot of code from MinecraftClient/Protocol/Handlers/Packet/s2c/DeclareCommands.cs from MCC.
     */
    private Task HandleDeclareCommandsPacket(DeclareCommandsPacket packet)
    {
        var buffer = packet.RawBuffer;
        int nodeCount = buffer.ReadVarInt();
        var nodes = new CommandNode[nodeCount];

        for (int i = 0; i < nodes.Length; i++)
        {
            byte flags = buffer.ReadByte();
            int childCount = buffer.ReadVarInt();
            int[] children = new int[childCount];
            for (int j = 0; j < childCount; ++j)
                children[j] = buffer.ReadVarInt();

            int redirectNode = ((flags & 0x08) == 0x08) ? buffer.ReadVarInt() : -1;

            string? name = ((flags & 0x03) == 1 || (flags & 0x03) == 2) ? buffer.ReadString() : null;

            int parserId = ((flags & 0x03) == 2) ? buffer.ReadVarInt() : -1;
            IParser? parser = CommandParserFactory.ReadParser(parserId, this.Bot.Data, buffer);

            string? suggestionsType = ((flags & 0x10) == 0x10) ? buffer.ReadString() : null;
            nodes[i] = new CommandNode(flags, children, redirectNode, name, parser, suggestionsType);
        }
        var rootIndex = buffer.ReadVarInt();
        this._commandTree = new CommandTree(rootIndex, nodes);

        return Task.CompletedTask;
    }

    private Task HandleChatMessagePacket(CBChatMessagePacket packet)
    {
        // TODO: Advanced chat stuff like filtering and signature verification.

        if (packet.Body is CBChatMessagePacket.V1_19_2_3Body body && body.Signature != null)
        {
            if (this._messageCollector is LastSeenMessageCollector1_19_2 collector1_19_2)
            {
                collector1_19_2.Push(new AcknowledgedMessage(true, body.Sender, body.Signature));
                if (collector1_19_2.PendingAcknowledgements++ > 64)
                {
                    var messages = collector1_19_2.AcknowledgeMessages().Select(x => x.ToProtocolMessage()).ToArray();
                    this.Bot.Client.SendPacket(new MessageAcknowledgementPacket(messages, null));
                }
            } else if (this._messageCollector is LastSeenMessageCollector1_19_3 collector1_19_3)
            {
                int count = collector1_19_3.ResetCount();
                if (count > 0)
                    this.Bot.Client.SendPacket(new MessageAcknowledgementPacket(count));
            }
        }
        
        (UUID sender, string message, ChatMessageType type) = packet.Body switch {
            CBChatMessagePacket.V1_18Body v18 => (v18.Sender, v18.Message, (ChatMessageType)v18.Position),
            CBChatMessagePacket.V1_19Body v19 => (v19.Sender, v19.SignedChat, (ChatMessageType)v19.MessageType),
            CBChatMessagePacket.V1_19_2_3Body v19_2 => (v19_2.Sender, v19_2.PlainMessage, (ChatMessageType)v19_2.Type)
        };

        this.OnChatMessageReceived?.Invoke(this.Bot, sender, message, type);
        
        return Task.CompletedTask;
    }
    

    private Task SendChat1_19_1(string message)
    {
        byte[] signature;
        var timestamp = DateTimeOffset.UtcNow;
        var salt = ChatSignature.GenerateSalt();

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
                new SBChatMessagePacket.V1_19(
                    timestamp.ToUnixTimeMilliseconds(),
                    salt,
                    signature,
                    false)));
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

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt = ChatSignature.GenerateSalt();
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
        var collector = (LastSeenMessageCollector1_19_2)this._messageCollector!;
        var messages = collector.AcknowledgeMessages();
        var timestamp = DateTimeOffset.UtcNow;
        byte[] signature;
        long salt = ChatSignature.GenerateSalt();

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            signature = Array.Empty<byte>();
            salt = 0;
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
            new SBChatMessagePacket.V1_19(
                timestamp.ToUnixTimeMilliseconds(),
                salt,
                signature,
                false,
                messages.Select(x => x.ToProtocolMessage()).ToArray(),
                null
            )));
    }
    
    private Task SendCommand1_19_2(string command, List<(string, string)> arguments)
    {
        var collector = (LastSeenMessageCollector1_19_2)this._messageCollector!;
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

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt = ChatSignature.GenerateSalt();
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
        var collector = (LastSeenMessageCollector1_19_3)this._messageCollector!;
        byte[] acknowledgedBitfield = collector.Collect(out var count, out var acknowledged);
        
        var timestamp = DateTimeOffset.UtcNow;

        if (!this.Bot.Session.OnlineSession || this.Bot.Session.Certificate == null)
        {
            return this.Bot.Client.SendPacket(
                new SBChatMessagePacket(
                    message,
                    new SBChatMessagePacket.V1_19(
                        timestamp.ToUnixTimeMilliseconds(),
                        0,
                        null,
                        count, acknowledgedBitfield)));
        }

        var salt = ChatSignature.GenerateSalt();
        byte[] signature = ChatSignature.SignChat1_19_3(
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
                new SBChatMessagePacket.V1_19(
                    timestamp.ToUnixTimeMilliseconds(),
                    salt,
                    signature,
                    count,
                    acknowledgedBitfield)));
    }

    private Task SendCommand1_19_3(string command, List<(string, string)> arguments)
    {       
        var collector = (LastSeenMessageCollector1_19_3)this._messageCollector!;
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

        var signatures = new List<ChatCommandPacket.ArgumentSignature>();
        long salt = ChatSignature.GenerateSalt();
        foreach ((var name, var argument) in arguments)
        {
            var signature = ChatSignature.SignChat1_19_3(
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
}
