using MineSharp.Core.Common;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Serverbound.Configuration;
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
using LoginDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Login.DisconnectPacket;
using ConfigurationDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.DisconnectPacket;
using CBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.KeepAlivePacket;
using SBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.KeepAlivePacket;
using CBPluginMessagePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PluginMessagePacket;
using SBFinishConfigurationPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.FinishConfigurationPacket;
using CBFinishConfigurationPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.FinishConfigurationPacket;
using SBPluginMessagePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PluginMessagePacket;
using CBCloseWindowPacket = MineSharp.Protocol.Packets.Clientbound.Play.CloseWindowPacket;
using SBCloseWindowPacket = MineSharp.Protocol.Packets.Serverbound.Play.CloseWindowPacket;
using CBSetHeldItemPacket = MineSharp.Protocol.Packets.Clientbound.Play.SetHeldItemPacket;
using SBSetHeldItemPacket = MineSharp.Protocol.Packets.Serverbound.Play.SetHeldItemPacket;
using PlayPingPacket = MineSharp.Protocol.Packets.Clientbound.Play.PingPacket;
using PlayPongPacket = MineSharp.Protocol.Packets.Serverbound.Play.PongPacket;
using ConfPingPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PingPacket;
using ConfPongPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PongPacket;

namespace MineSharp.Protocol.Packets;

internal static class PacketPalette
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public delegate IPacket PacketFactory(PacketBuffer buffer, MinecraftData version);

    private static readonly IDictionary<PacketType, PacketFactory> PacketFactories;
    private static readonly IDictionary<Guid, PacketType>          ClassToTypeMap;

    static PacketPalette()
    {
        PacketFactories = new Dictionary<PacketType, PacketFactory>();
        ClassToTypeMap  = new Dictionary<Guid, PacketType>();

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

    public static PacketType GetPacketType<T>() where T : IPacket
    {
        var guid = typeof(T).GUID;
        return ClassToTypeMap[guid];
    }

    private static void InitializePackets()
    {
        // Handshaking
        RegisterPacket<HandshakePacket>(PacketType.SB_Handshake_SetProtocol);

        // Login
        RegisterPacket<LoginDisconnectPacket>(PacketType.CB_Login_Disconnect);
        RegisterPacket<EncryptionRequestPacket>(PacketType.CB_Login_EncryptionBegin);
        RegisterPacket<LoginSuccessPacket>(PacketType.CB_Login_Success);
        RegisterPacket<SetCompressionPacket>(PacketType.CB_Login_Compress);
        RegisterPacket<LoginPluginRequestPacket>(PacketType.CB_Login_LoginPluginRequest);

        RegisterPacket<LoginStartPacket>(PacketType.SB_Login_LoginStart);
        RegisterPacket<EncryptionResponsePacket>(PacketType.SB_Login_EncryptionBegin);
        RegisterPacket<LoginPluginResponsePacket>(PacketType.SB_Login_LoginPluginResponse);
        RegisterPacket<AcknowledgeLoginPacket>(PacketType.SB_Login_LoginAcknowledged);

        // Status
        RegisterPacket<StatusResponsePacket>(PacketType.CB_Status_ServerInfo);
        RegisterPacket<PongResponsePacket>(PacketType.CB_Status_Ping);

        RegisterPacket<StatusRequestPacket>(PacketType.SB_Status_PingStart);
        RegisterPacket<PingRequestPacket>(PacketType.SB_Status_Ping);

        // Configuration
        RegisterPacket<CBPluginMessagePacket>(PacketType.CB_Configuration_CustomPayload);
        RegisterPacket<ConfigurationDisconnectPacket>(PacketType.CB_Configuration_Disconnect);
        RegisterPacket<CBFinishConfigurationPacket>(PacketType.CB_Configuration_FinishConfiguration);
        RegisterPacket<CBConfigurationKeepAlivePacket>(PacketType.CB_Configuration_KeepAlive);
        RegisterPacket<ConfPingPacket>(PacketType.CB_Configuration_Ping);
        RegisterPacket<RegistryDataPacket>(PacketType.CB_Configuration_RegistryData);
        RegisterPacket<FeatureFlagsPacket>(PacketType.CB_Configuration_FeatureFlags);

        RegisterPacket<ClientInformationPacket>(PacketType.SB_Configuration_Settings);
        RegisterPacket<SBPluginMessagePacket>(PacketType.SB_Configuration_CustomPayload);
        RegisterPacket<SBFinishConfigurationPacket>(PacketType.SB_Configuration_FinishConfiguration);
        RegisterPacket<SBConfigurationKeepAlivePacket>(PacketType.SB_Configuration_KeepAlive);
        RegisterPacket<ConfPongPacket>(PacketType.SB_Configuration_Pong);
        RegisterPacket<ResourcePackResponsePacket>(PacketType.SB_Configuration_ResourcePackReceive);

        // Play
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
        RegisterPacket<AcknowledgeBlockChangePacket>(PacketType.CB_Play_AcknowledgePlayerDigging);
        RegisterPacket<WindowItemsPacket>(PacketType.CB_Play_WindowItems);
        RegisterPacket<WindowSetSlotPacket>(PacketType.CB_Play_SetSlot);
        RegisterPacket<OpenWindowPacket>(PacketType.CB_Play_OpenWindow);
        RegisterPacket<CBCloseWindowPacket>(PacketType.CB_Play_CloseWindow);
        RegisterPacket<CBSetHeldItemPacket>(PacketType.CB_Play_HeldItemSlot);
        RegisterPacket<SystemChatMessagePacket>(PacketType.CB_Play_SystemChat);
        RegisterPacket<DisguisedChatMessagePacket>(PacketType.CB_Play_ProfilelessChat);
        RegisterPacket<EntityStatusPacket>(PacketType.CB_Play_EntityStatus);
        RegisterPacket<ChunkBatchStartPacket>(PacketType.CB_Play_ChunkBatchStart);
        RegisterPacket<ChunkBatchFinishedPacket>(PacketType.CB_Play_ChunkBatchFinished);
        RegisterPacket<PlayPingPacket>(PacketType.CB_Play_Ping);

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
        RegisterPacket<WindowClickPacket>(PacketType.SB_Play_WindowClick);
        RegisterPacket<PlaceBlockPacket>(PacketType.SB_Play_BlockPlace);
        RegisterPacket<PlayerActionPacket>(PacketType.SB_Play_BlockDig);
        RegisterPacket<SwingArmPacket>(PacketType.SB_Play_ArmAnimation);
        RegisterPacket<InteractPacket>(PacketType.SB_Play_UseEntity);
        RegisterPacket<SBCloseWindowPacket>(PacketType.SB_Play_CloseWindow);
        RegisterPacket<EntityActionPacket>(PacketType.SB_Play_EntityAction);
        RegisterPacket<UseItemPacket>(PacketType.SB_Play_UseItem);
        RegisterPacket<SBSetHeldItemPacket>(PacketType.SB_Play_HeldItemSlot);
        RegisterPacket<ChunkBatchReceivedPacket>(PacketType.SB_Play_ChunkBatchReceived);
        RegisterPacket<SetCreativeSlotPacket>(PacketType.SB_Play_SetCreativeSlot);
        RegisterPacket<PlayPongPacket>(PacketType.SB_Play_Pong);
    }

    private static void RegisterPacket<TPacket>(PacketType type) where TPacket : IPacket
    {
        PacketFactories.Add(type, TPacket.Read);
        ClassToTypeMap.Add(typeof(TPacket).GUID, type);
    }
}
