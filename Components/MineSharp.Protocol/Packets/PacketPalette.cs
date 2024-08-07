using System.Collections.Frozen;
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
using CBChatPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatPacket;
using CBCloseWindowPacket = MineSharp.Protocol.Packets.Clientbound.Play.CloseWindowPacket;
using CBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.KeepAlivePacket;
using CBFinishConfigurationPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.FinishConfigurationPacket;
using CBKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;
using CBPluginMessagePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PluginMessagePacket;
using CBSetHeldItemPacket = MineSharp.Protocol.Packets.Clientbound.Play.SetHeldItemPacket;
using ConfClientInformation = MineSharp.Protocol.Packets.Serverbound.Configuration.ClientInformationPacket;
using ConfigurationDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.DisconnectPacket;
using ConfPingPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PingPacket;
using ConfPongPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PongPacket;
using LoginDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Login.DisconnectPacket;
using PlayClientInformation = MineSharp.Protocol.Packets.Serverbound.Play.ClientInformationPacket;
using PlayDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Play.DisconnectPacket;
using PlayPingPacket = MineSharp.Protocol.Packets.Clientbound.Play.PingPacket;
using PlayPongPacket = MineSharp.Protocol.Packets.Serverbound.Play.PongPacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;
using SBChatPacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatPacket;
using SBCloseWindowPacket = MineSharp.Protocol.Packets.Serverbound.Play.CloseWindowPacket;
using SBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.KeepAlivePacket;
using SBFinishConfigurationPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.FinishConfigurationPacket;
using SBKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Play.KeepAlivePacket;
using SBPluginMessagePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PluginMessagePacket;
using SBSetHeldItemPacket = MineSharp.Protocol.Packets.Serverbound.Play.SetHeldItemPacket;

namespace MineSharp.Protocol.Packets;

internal static class PacketPalette
{
    public delegate IPacket PacketFactory(PacketBuffer buffer, MinecraftData version);
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly FrozenDictionary<PacketType, PacketFactory> PacketFactories;

    static PacketPalette()
    {
        PacketFactories = InitializePackets();
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

    private static FrozenDictionary<PacketType, PacketFactory> InitializePackets()
    {
        Dictionary<PacketType, PacketFactory> packetFactories = new();

        void RegisterPacket<TPacket>()
            where TPacket : IPacket
        {
            packetFactories.Add(TPacket.StaticType, TPacket.Read);
        }

        // Handshaking
        RegisterPacket<HandshakePacket>();

        // Login
        RegisterPacket<LoginDisconnectPacket>();
        RegisterPacket<EncryptionRequestPacket>();
        RegisterPacket<LoginSuccessPacket>();
        RegisterPacket<SetCompressionPacket>();
        RegisterPacket<LoginPluginRequestPacket>();

        RegisterPacket<LoginStartPacket>();
        RegisterPacket<EncryptionResponsePacket>();
        RegisterPacket<LoginPluginResponsePacket>();
        RegisterPacket<AcknowledgeLoginPacket>();

        // Status
        RegisterPacket<StatusResponsePacket>();
        RegisterPacket<PongResponsePacket>();

        RegisterPacket<StatusRequestPacket>();
        RegisterPacket<PingRequestPacket>();

        // Configuration
        RegisterPacket<CBPluginMessagePacket>();
        RegisterPacket<ConfigurationDisconnectPacket>();
        RegisterPacket<CBFinishConfigurationPacket>();
        RegisterPacket<CBConfigurationKeepAlivePacket>();
        RegisterPacket<ConfPingPacket>();
        RegisterPacket<RegistryDataPacket>();
        RegisterPacket<FeatureFlagsPacket>();

        RegisterPacket<ConfClientInformation>();
        RegisterPacket<SBPluginMessagePacket>();
        RegisterPacket<SBFinishConfigurationPacket>();
        RegisterPacket<SBConfigurationKeepAlivePacket>();
        RegisterPacket<ConfPongPacket>();
        RegisterPacket<ResourcePackResponsePacket>();

        // Play
        RegisterPacket<SpawnPaintingPacket>();
        RegisterPacket<SpawnLivingEntityPacket>();
        RegisterPacket<SpawnEntityPacket>();
        RegisterPacket<CBKeepAlivePacket>();
        RegisterPacket<ChunkDataAndUpdateLightPacket>();
        RegisterPacket<ParticlePacket>();
        RegisterPacket<UnloadChunkPacket>();
        RegisterPacket<BlockUpdatePacket>();
        RegisterPacket<MultiBlockUpdatePacket>();
        RegisterPacket<LoginPacket>();
        RegisterPacket<PlayerPositionPacket>();
        RegisterPacket<SetHealthPacket>();
        RegisterPacket<CombatDeathPacket>();
        RegisterPacket<RespawnPacket>();
        RegisterPacket<RemoveEntitiesPacket>();
        RegisterPacket<SetEntityVelocityPacket>();
        RegisterPacket<EntityPositionPacket>();
        RegisterPacket<EntityPositionAndRotationPacket>();
        RegisterPacket<EntityRotationPacket>();
        RegisterPacket<TeleportEntityPacket>();
        RegisterPacket<UpdateAttributesPacket>();
        RegisterPacket<DeclareCommandsPacket>();
        RegisterPacket<CBChatPacket>();
        RegisterPacket<PlayerChatPacket>();
        RegisterPacket<SpawnPlayerPacket>();
        RegisterPacket<PlayerInfoUpdatePacket>();
        RegisterPacket<PlayerInfoRemovePacket>();
        RegisterPacket<GameEventPacket>();
        RegisterPacket<AcknowledgeBlockChangePacket>();
        RegisterPacket<WindowItemsPacket>();
        RegisterPacket<WindowSetSlotPacket>();
        RegisterPacket<OpenWindowPacket>();
        RegisterPacket<CBCloseWindowPacket>();
        RegisterPacket<CBSetHeldItemPacket>();
        RegisterPacket<EntitySoundEffectPacket>();
        RegisterPacket<SoundEffectPacket>();
        RegisterPacket<SystemChatMessagePacket>();
        RegisterPacket<DisguisedChatMessagePacket>();
        RegisterPacket<EntityStatusPacket>();
        RegisterPacket<ChunkBatchStartPacket>();
        RegisterPacket<ChunkBatchFinishedPacket>();
        RegisterPacket<PlayPingPacket>();
        RegisterPacket<PlayDisconnectPacket>();
        RegisterPacket<SetPassengersPacket>();

        RegisterPacket<SBKeepAlivePacket>();
        RegisterPacket<SetPlayerPositionPacket>();
        RegisterPacket<SetPlayerPositionAndRotationPacket>();
        RegisterPacket<ClientCommandPacket>();
        RegisterPacket<SBChatPacket>();
        RegisterPacket<SBChatMessagePacket>();
        RegisterPacket<ChatCommandPacket>();
        RegisterPacket<MessageAcknowledgementPacket>();
        RegisterPacket<PlayerSessionPacket>();
        RegisterPacket<ConfirmTeleportPacket>();
        RegisterPacket<UpdateCommandBlock>();
        RegisterPacket<WindowClickPacket>();
        RegisterPacket<PlaceBlockPacket>();
        RegisterPacket<PlayerActionPacket>();
        RegisterPacket<SwingArmPacket>();
        RegisterPacket<InteractPacket>();
        RegisterPacket<SBCloseWindowPacket>();
        RegisterPacket<EntityActionPacket>();
        RegisterPacket<UseItemPacket>();
        RegisterPacket<SBSetHeldItemPacket>();
        RegisterPacket<ChunkBatchReceivedPacket>();
        RegisterPacket<SetCreativeSlotPacket>();
        RegisterPacket<PlayPongPacket>();
        RegisterPacket<PlayClientInformation>();

        return packetFactories.ToFrozenDictionary();
    }

}
