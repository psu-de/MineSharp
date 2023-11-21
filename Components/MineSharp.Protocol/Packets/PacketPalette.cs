using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Status;
using NLog;

using CBKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;
using SBKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Play.KeepAlivePacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;
using CBChatPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatPacket;
using SBChatPacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatPacket;

namespace MineSharp.Protocol.Packets;

public static class PacketPalette
{
    public delegate IPacket PacketFactory(PacketBuffer buffer, MinecraftData version);

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private static readonly IDictionary<PacketType, PacketFactory> PacketFactories;

    static PacketPalette()
    {
        PacketFactories = new Dictionary<PacketType, PacketFactory>();
        
        InitializePackets();
    }

    public static PacketFactory? GetFactory(PacketType packetType)
    {
        if (!PacketFactories.TryGetValue(packetType, out var packet))
        {
            // Logger.Trace($"Unknown packet for state {state}, direction {direction} and id {id}.");
            return null;
        }

        return packet;
    }
    
    private static void InitializePackets()
    {
        RegisterPacket<HandshakePacket>(PacketType.SB_Handshake_SetProtocol);
        
        RegisterPacket<DisconnectPacket>(PacketType.CB_Login_Disconnect);
        RegisterPacket<EncryptionRequestPacket>(PacketType.CB_Login_EncryptionBegin);
        RegisterPacket<LoginSuccessPacket>(PacketType.CB_Login_Success);
        RegisterPacket<SetCompressionPacket>(PacketType.CB_Login_Compress);
        RegisterPacket<LoginPluginRequestPacket>(PacketType.CB_Login_LoginPluginRequest);

        RegisterPacket<LoginStartPacket>(PacketType.SB_Login_LoginStart);
        RegisterPacket<EncryptionResponsePacket>(PacketType.SB_Login_EncryptionBegin);
        RegisterPacket<LoginPluginResponsePacket>(PacketType.SB_Login_LoginPluginResponse);
        
        RegisterPacket<StatusResponsePacket>(PacketType.CB_Status_ServerInfo);
        RegisterPacket<PongResponsePacket>(PacketType.CB_Status_Ping);
        
        RegisterPacket<StatusRequestPacket>(PacketType.SB_Status_PingStart);
        RegisterPacket<PingRequestPacket>(PacketType.SB_Status_Ping);
        
        RegisterPacket<SpawnPaintingPacket>(PacketType.CB_Play_SpawnEntityPainting);
        RegisterPacket<SpawnLivingEntityPacket>(PacketType.CB_Play_SpawnEntityLiving);
        RegisterPacket<SpawnEntityPacket>(PacketType.CB_Play_SpawnEntity);
        RegisterPacket<CBKeepAlivePacket>(PacketType.CB_Play_KeepAlive);
        RegisterPacket<ChunkDataAndUpdateLightPacket>(PacketType.CB_Play_MapChunk);
        RegisterPacket<UnloadChunkPacket>(PacketType.CB_Play_UnloadChunk);
        RegisterPacket<BlockUpdatePacket>(PacketType.CB_Play_BlockChange);
        RegisterPacket<MultiBlockUpdatePacket>(PacketType.CB_Play_MultiBlockChange);
        RegisterPacket<LoginPacket>(PacketType.CB_Play_Login);
        RegisterPacket<PlayerPositionPacket>(PacketType.CB_Play_Position);
        RegisterPacket<SetHealthPacket>(PacketType.CB_Play_UpdateHealth);
        RegisterPacket<CombatDeathPacket>(PacketType.CB_Play_DeathCombatEvent);
        RegisterPacket<RespawnPacket>(PacketType.CB_Play_Respawn);
        RegisterPacket<RemoveEntitiesPacket>(PacketType.CB_Play_EntityDestroy);
        RegisterPacket<SetEntityVelocityPacket>(PacketType.CB_Play_EntityVelocity);
        RegisterPacket<EntityPositionPacket>(PacketType.CB_Play_RelEntityMove);
        RegisterPacket<EntityPositionAndRotationPacket>(PacketType.CB_Play_EntityMoveLook);
        RegisterPacket<EntityRotationPacket>(PacketType.CB_Play_EntityLook);
        RegisterPacket<TeleportEntityPacket>(PacketType.CB_Play_EntityTeleport);
        RegisterPacket<UpdateAttributesPacket>(PacketType.CB_Play_EntityUpdateAttributes);
        RegisterPacket<DeclareCommandsPacket>(PacketType.CB_Play_DeclareCommands);
        RegisterPacket<CBChatPacket>(PacketType.CB_Play_Chat);
        RegisterPacket<PlayerChatPacket>(PacketType.CB_Play_PlayerChat);
        RegisterPacket<SpawnPlayerPacket>(PacketType.CB_Play_NamedEntitySpawn);
        RegisterPacket<PlayerInfoUpdatePacket>(PacketType.CB_Play_PlayerInfo);
        RegisterPacket<PlayerInfoRemovePacket>(PacketType.CB_Play_PlayerRemove);
        RegisterPacket<GameEventPacket>(PacketType.CB_Play_GameStateChange);

        RegisterPacket<SBKeepAlivePacket>(PacketType.SB_Play_KeepAlive);
        RegisterPacket<SetPlayerPositionPacket>(PacketType.SB_Play_Position);
        RegisterPacket<SetPlayerPositionAndRotationPacket>(PacketType.SB_Play_PositionLook);
        RegisterPacket<ClientCommandPacket>(PacketType.SB_Play_ClientCommand);
        RegisterPacket<SBChatPacket>(PacketType.SB_Play_Chat);
        RegisterPacket<SBChatMessagePacket>(PacketType.SB_Play_ChatMessage);
        RegisterPacket<ChatCommandPacket>(PacketType.SB_Play_ChatCommand);
        RegisterPacket<MessageAcknowledgementPacket>(PacketType.SB_Play_MessageAcknowledgement);
        RegisterPacket<PlayerSessionPacket>(PacketType.SB_Play_ChatSessionUpdate);
        RegisterPacket<ConfirmTeleportPacket>(PacketType.SB_Play_TeleportConfirm);
        RegisterPacket<UpdateCommandBlock>(PacketType.SB_Play_UpdateCommandBlock);
    }

    private static void RegisterPacket<TPacket>(PacketType type) where TPacket : IPacket
    {
        PacketFactories.Add(type, TPacket.Read);
    }
}
