using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Exceptions;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Mappings;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Status;
using NLog;

using CBKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;
using SBKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Play.KeepAlivePacket;
using CBChatMessagePacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatMessagePacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;

namespace MineSharp.Protocol.Packets;

public static class PacketPalette
{
    public delegate IPacket PacketFactory(PacketBuffer buffer, MinecraftData version, string packetName);

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public static readonly MinecraftData ImplementedProtocol = MinecraftData.FromVersion("1.19.4");
    
    private static readonly IDictionary<int, PacketFactory> PacketFactories;
    private static readonly IDictionary<Guid, int> PacketToId;

    private static readonly IDictionary<int, IPacketMapping> Mappings;

    static PacketPalette()
    {
        PacketFactories = new Dictionary<int, PacketFactory>();
        PacketToId = new Dictionary<Guid, int>();
        Mappings = new Dictionary<int, IPacketMapping>();
        
        InitializePackets();
        InitializeMappers();
    }

    internal static IPacketMapping GetPacketMappingFromVersion(int protocolVersion)
    {
        if (!Mappings.TryGetValue(protocolVersion, out var mapping))
        {
            throw new MineSharpDataException($"Protocol version {protocolVersion} is currently not supported.");
        }

        return mapping;
    }

    public static PacketFactory? GetFactory(int id, GameState state, PacketFlow direction)
    {
        int key = GetPacketKey(id, state, direction);

        if (!PacketFactories.TryGetValue(key, out var packet))
        {
            // Logger.Trace($"Unknown packet for state {state}, direction {direction} and id {id}.");
            return null;
        }

        return packet;
    }

    public static int GetId(IPacket packet)
    {
        Guid key = packet.GetType().GUID!;

        if (!PacketToId.TryGetValue(key, out var id))
        {
            throw new MineSharpDataException($"Unknown packet: {key}");
        }

        return id;
    }

    private static void InitializeMappers()
    {
        RegisterMapper<MC_1_18_PacketMapping>();
        RegisterMapper<MC_1_19_PacketMapping>();
    }
    
    private static void InitializePackets()
    {
        RegisterPacket<HandshakePacket>(GameState.HANDSHAKING, PacketFlow.Serverbound);
        
        RegisterPacket<DisconnectPacket>(GameState.LOGIN, PacketFlow.Clientbound);
        RegisterPacket<EncryptionRequestPacket>(GameState.LOGIN, PacketFlow.Clientbound);
        RegisterPacket<LoginSuccessPacket>(GameState.LOGIN, PacketFlow.Clientbound);
        RegisterPacket<SetCompressionPacket>(GameState.LOGIN, PacketFlow.Clientbound);
        RegisterPacket<LoginPluginRequestPacket>(GameState.LOGIN, PacketFlow.Clientbound);
        
        RegisterPacket<LoginStartPacket>(GameState.LOGIN, PacketFlow.Serverbound);
        RegisterPacket<EncryptionResponsePacket>(GameState.LOGIN, PacketFlow.Serverbound);
        RegisterPacket<LoginPluginResponsePacket>(GameState.LOGIN, PacketFlow.Serverbound);
        
        RegisterPacket<StatusRequestPacket>(GameState.STATUS, PacketFlow.Serverbound);
        RegisterPacket<PingRequestPacket>(GameState.STATUS, PacketFlow.Serverbound);
        
        RegisterPacket<StatusResponsePacket>(GameState.STATUS, PacketFlow.Clientbound);
        RegisterPacket<PongResponsePacket>(GameState.STATUS, PacketFlow.Clientbound);
        
        RegisterPacket<CBKeepAlivePacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<ChunkDataAndUpdateLightPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<UnloadChunkPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<BlockUpdatePacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<LoginPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<SynchronizePlayerPositionPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<SetHealthPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<CombatDeathPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<RespawnPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<SpawnEntityPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<RemoveEntitiesPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<SetEntityVelocityPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<UpdateEntityPositionPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<UpdateEntityPositionAndRotationPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<UpdateEntityRotationPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<TeleportEntityPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<UpdateAttributesPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<DeclareCommandsPacket>(GameState.PLAY, PacketFlow.Clientbound);
        RegisterPacket<CBChatMessagePacket>(GameState.PLAY, PacketFlow.Clientbound);

        RegisterPacket<SBKeepAlivePacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<SetPlayerPositionPacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<SetPlayerPositionAndRotationPacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<ClientCommandPacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<SBChatMessagePacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<PlayerSessionPacket>(GameState.PLAY, PacketFlow.Serverbound);
        RegisterPacket<ChatCommandPacket>(GameState.PLAY, PacketFlow.Serverbound);
    }

    private static void RegisterMapper<TMapper>() where TMapper : IPacketMapping, new()
    {
        var mapping = new TMapper();
        
        foreach (var version in mapping.SupportedVersions)
        {
            Mappings.Add(version, mapping);
        }
    }
    
    private static int GetPacketKey(int id, GameState state, PacketFlow direction)
    {
        return id | (int)state << 16 | (int)direction << 24;
    }

    private static void RegisterPacket<TPacket>(GameState state, PacketFlow direction) where TPacket : IPacket
    {
        int key = GetPacketKey(TPacket.Id, state, direction);
        PacketFactories.Add(key, TPacket.Read);
        PacketToId.Add(typeof(TPacket).GUID!, TPacket.Id);
    }
}
