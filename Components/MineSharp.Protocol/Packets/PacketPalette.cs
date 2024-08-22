using System.Collections.Frozen;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.NetworkTypes;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Status;
using NLog;
using CBChatPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChatPacket;
using CBCloseWindowPacket = MineSharp.Protocol.Packets.Clientbound.Play.CloseWindowPacket;
using CBConfigurationAddResourcePackPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.AddResourcePackPacket;
using CBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.KeepAlivePacket;
using CBConfigurationPluginMessagePacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PluginMessagePacket;
using CBConfigurationRemoveResourcePackPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.RemoveResourcePackPacket;
using CBConfigurationUpdateTagsPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.UpdateTagsPacket;
using CBFinishConfigurationPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.FinishConfigurationPacket;
using CBKeepAlivePacket = MineSharp.Protocol.Packets.Clientbound.Play.KeepAlivePacket;
using CBPlayAddResourcePackPacket = MineSharp.Protocol.Packets.Clientbound.Play.AddResourcePackPacket;
using CBPlayChangeDifficultyPacket = MineSharp.Protocol.Packets.Clientbound.Play.ChangeDifficultyPacket;
using CBPlayMoveVehiclePacket = MineSharp.Protocol.Packets.Clientbound.Play.MoveVehiclePacket;
using CBPlayPingResponsePacket = MineSharp.Protocol.Packets.Clientbound.Play.PingResponsePacket;
using CBPlayPlayerAbilitiesPacket = MineSharp.Protocol.Packets.Clientbound.Play.PlayerAbilitiesPacket;
using CBPlayPluginMessagePacket = MineSharp.Protocol.Packets.Clientbound.Play.PluginMessagePacket;
using CBPlayRemoveResourcePackPacket = MineSharp.Protocol.Packets.Clientbound.Play.RemoveResourcePackPacket;
using CBPlayUpdateTagsPacket = MineSharp.Protocol.Packets.Clientbound.Play.UpdateTagsPacket;
using CBSetHeldItemPacket = MineSharp.Protocol.Packets.Clientbound.Play.SetHeldItemPacket;
using CBStatusPingResponsePacket = MineSharp.Protocol.Packets.Clientbound.Status.PingResponsePacket;
using ConfClientInformationPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.ClientInformationPacket;
using ConfigurationDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.DisconnectPacket;
using ConfPingPacket = MineSharp.Protocol.Packets.Clientbound.Configuration.PingPacket;
using ConfPongPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PongPacket;
using LoginDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Login.DisconnectPacket;
using PlayClientInformationPacket = MineSharp.Protocol.Packets.Serverbound.Play.ClientInformationPacket;
using PlayDisconnectPacket = MineSharp.Protocol.Packets.Clientbound.Play.DisconnectPacket;
using PlayPingPacket = MineSharp.Protocol.Packets.Clientbound.Play.PingPacket;
using PlayPongPacket = MineSharp.Protocol.Packets.Serverbound.Play.PongPacket;
using SBChatMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatMessagePacket;
using SBChatPacket = MineSharp.Protocol.Packets.Serverbound.Play.ChatPacket;
using SBCloseWindowPacket = MineSharp.Protocol.Packets.Serverbound.Play.CloseWindowPacket;
using SBConfigurationKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.KeepAlivePacket;
using SBConfigurationPluginMessagePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.PluginMessagePacket;
using SBConfigurationResourcePackResponsePacket = MineSharp.Protocol.Packets.Serverbound.Configuration.ResourcePackResponsePacket;
using SBFinishConfigurationPacket = MineSharp.Protocol.Packets.Serverbound.Configuration.FinishConfigurationPacket;
using SBKeepAlivePacket = MineSharp.Protocol.Packets.Serverbound.Play.KeepAlivePacket;
using SBPlayChangeDifficultyPacket = MineSharp.Protocol.Packets.Serverbound.Play.ChangeDifficultyPacket;
using SBPlayMoveVehiclePacket = MineSharp.Protocol.Packets.Serverbound.Play.MoveVehiclePacket;
using SBPlayPingRequestPacket = MineSharp.Protocol.Packets.Serverbound.Play.PingRequestPacket;
using SBPlayPlayerAbilitiesPacket = MineSharp.Protocol.Packets.Serverbound.Play.PlayerAbilitiesPacket;
using SBPlayPluginMessagePacket = MineSharp.Protocol.Packets.Serverbound.Play.PluginMessagePacket;
using SBSetHeldItemPacket = MineSharp.Protocol.Packets.Serverbound.Play.SetHeldItemPacket;
using StatusPingRequestPacket = MineSharp.Protocol.Packets.Serverbound.Status.PingRequestPacket;

namespace MineSharp.Protocol.Packets;

internal static class PacketPalette
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly FrozenDictionary<PacketType, PacketFactory<IPacketClientbound>> ClientboundPacketFactories;
    private static readonly FrozenDictionary<PacketType, PacketFactory<IPacketServerbound>> ServerboundPacketFactories;

    static PacketPalette()
    {
        (ClientboundPacketFactories, ServerboundPacketFactories) = InitializePackets();
    }

    public static PacketFactory<IPacketClientbound>? GetClientboundFactory(PacketType packetType)
    {
        if (!ClientboundPacketFactories.TryGetValue(packetType, out var packet))
        {
            // Logger.Trace($"Unknown packet for state {state}, direction {direction} and id {id}.");
            return null;
        }

        return packet;
    }

    private static (FrozenDictionary<PacketType, PacketFactory<IPacketClientbound>> Clientbound, FrozenDictionary<PacketType, PacketFactory<IPacketServerbound>> Serverbound) InitializePackets()
    {
        Dictionary<PacketType, PacketFactory<IPacketClientbound>> clientboundPacketFactories = new();
        Dictionary<PacketType, PacketFactory<IPacketServerbound>> serverboundPacketFactories = new();

        void RegisterPacketClientbound<TPacket>()
            where TPacket : IPacketStatic<TPacket>, IPacketClientbound
        {
            PacketFactory<IPacketClientbound> readDelegate = (buffer, data) => GetPacketFactory<TPacket>()(buffer, data);
            clientboundPacketFactories.Add(TPacket.StaticType, readDelegate);
        }

        void RegisterPacketServerbound<TPacket>()
            where TPacket : IPacketStatic<TPacket>, IPacketServerbound
        {
            PacketFactory<IPacketServerbound> readDelegate = (buffer, data) => GetPacketFactory<TPacket>()(buffer, data);
            serverboundPacketFactories.Add(TPacket.StaticType, readDelegate);
        }

        PacketFactory<TPacket> GetPacketFactory<TPacket>()
            where TPacket : IPacket, ISerializableWithMinecraftData<TPacket>
        {
            return TPacket.Read;
        }

        // Handshaking
        // SB
        RegisterPacketServerbound<HandshakePacket>();

        // Login
        // CB
        RegisterPacketClientbound<LoginDisconnectPacket>();
        RegisterPacketClientbound<EncryptionRequestPacket>();
        RegisterPacketClientbound<LoginSuccessPacket>();
        RegisterPacketClientbound<SetCompressionPacket>();
        RegisterPacketClientbound<LoginPluginRequestPacket>();

        // SB
        RegisterPacketServerbound<LoginStartPacket>();
        RegisterPacketServerbound<EncryptionResponsePacket>();
        RegisterPacketServerbound<LoginPluginResponsePacket>();
        RegisterPacketServerbound<LoginAcknowledgedPacket>();

        // Status
        // CB
        RegisterPacketClientbound<StatusResponsePacket>();
        RegisterPacketClientbound<CBStatusPingResponsePacket>();

        // SB
        RegisterPacketServerbound<StatusRequestPacket>();
        RegisterPacketServerbound<StatusPingRequestPacket>();

        // Configuration
        // CB
        RegisterPacketClientbound<CBConfigurationPluginMessagePacket>();
        RegisterPacketClientbound<ConfigurationDisconnectPacket>();
        RegisterPacketClientbound<CBFinishConfigurationPacket>();
        RegisterPacketClientbound<CBConfigurationKeepAlivePacket>();
        RegisterPacketClientbound<ConfPingPacket>();
        RegisterPacketClientbound<RegistryDataPacket>();
        RegisterPacketClientbound<FeatureFlagsPacket>();
        RegisterPacketClientbound<CBConfigurationAddResourcePackPacket>();
        RegisterPacketClientbound<CBConfigurationRemoveResourcePackPacket>();
        RegisterPacketClientbound<CBConfigurationUpdateTagsPacket>();

        // SB
        RegisterPacketServerbound<ConfClientInformationPacket>();
        RegisterPacketServerbound<SBConfigurationPluginMessagePacket>();
        RegisterPacketServerbound<SBFinishConfigurationPacket>();
        RegisterPacketServerbound<SBConfigurationKeepAlivePacket>();
        RegisterPacketServerbound<ConfPongPacket>();
        RegisterPacketServerbound<SBConfigurationResourcePackResponsePacket>();

        // Play
        // CB
        RegisterPacketClientbound<SpawnPaintingPacket>();
        RegisterPacketClientbound<SpawnLivingEntityPacket>();
        RegisterPacketClientbound<SpawnEntityPacket>();
        RegisterPacketClientbound<CBKeepAlivePacket>();
        RegisterPacketClientbound<ChunkDataAndUpdateLightPacket>();
        RegisterPacketClientbound<ParticlePacket>();
        RegisterPacketClientbound<UnloadChunkPacket>();
        RegisterPacketClientbound<BlockUpdatePacket>();
        RegisterPacketClientbound<MultiBlockUpdatePacket>();
        RegisterPacketClientbound<LoginPacket>();
        RegisterPacketClientbound<PlayerPositionPacket>();
        RegisterPacketClientbound<SetHealthPacket>();
        RegisterPacketClientbound<CombatDeathPacket>();
        RegisterPacketClientbound<RespawnPacket>();
        RegisterPacketClientbound<RemoveEntitiesPacket>();
        RegisterPacketClientbound<SetEntityVelocityPacket>();
        RegisterPacketClientbound<EntityPositionPacket>();
        RegisterPacketClientbound<EntityPositionAndRotationPacket>();
        RegisterPacketClientbound<EntityRotationPacket>();
        RegisterPacketClientbound<TeleportEntityPacket>();
        RegisterPacketClientbound<UpdateAttributesPacket>();
        RegisterPacketClientbound<DeclareCommandsPacket>();
        RegisterPacketClientbound<CBChatPacket>();
        RegisterPacketClientbound<PlayerChatPacket>();
        RegisterPacketClientbound<SpawnPlayerPacket>();
        RegisterPacketClientbound<PlayerInfoUpdatePacket>();
        RegisterPacketClientbound<PlayerInfoRemovePacket>();
        RegisterPacketClientbound<GameEventPacket>();
        RegisterPacketClientbound<AcknowledgeBlockChangePacket>();
        RegisterPacketClientbound<WindowItemsPacket>();
        RegisterPacketClientbound<WindowSetSlotPacket>();
        RegisterPacketClientbound<OpenWindowPacket>();
        RegisterPacketClientbound<CBCloseWindowPacket>();
        RegisterPacketClientbound<CBSetHeldItemPacket>();
        RegisterPacketClientbound<EntitySoundEffectPacket>();
        RegisterPacketClientbound<SoundEffectPacket>();
        RegisterPacketClientbound<SystemChatMessagePacket>();
        RegisterPacketClientbound<DisguisedChatMessagePacket>();
        RegisterPacketClientbound<EntityStatusPacket>();
        RegisterPacketClientbound<ChunkBatchStartPacket>();
        RegisterPacketClientbound<ChunkBatchFinishedPacket>();
        RegisterPacketClientbound<PlayPingPacket>();
        RegisterPacketClientbound<PlayDisconnectPacket>();
        RegisterPacketClientbound<SetPassengersPacket>();
        RegisterPacketClientbound<AwardStatisticsPacket>();
        RegisterPacketClientbound<BlockActionPacket>();
        RegisterPacketClientbound<BlockEntityDataPacket>();
        RegisterPacketClientbound<BossBarPacket>();
        RegisterPacketClientbound<CBPlayChangeDifficultyPacket>();
        RegisterPacketClientbound<ChatSuggestionsPacket>();
        RegisterPacketClientbound<ChunkBiomesPacket>();
        RegisterPacketClientbound<ClearTitlesPacket>();
        RegisterPacketClientbound<CBPlayPluginMessagePacket>();
        RegisterPacketClientbound<CommandSuggestionsResponsePacket>();
        RegisterPacketClientbound<DamageEventPacket>();
        RegisterPacketClientbound<DeleteMessagePacket>();
        RegisterPacketClientbound<EntityAnimationPacket>();
        RegisterPacketClientbound<SetBlockDestroyStagePacket>();
        RegisterPacketClientbound<SetCooldownPacket>();
        RegisterPacketClientbound<SpawnExperienceOrbPacket>();
        RegisterPacketClientbound<HurtAnimationPacket>();
        RegisterPacketClientbound<InitializeWorldBorderPacket>();
        RegisterPacketClientbound<MapDataPacket>();
        RegisterPacketClientbound<OpenHorseScreenPacket>();
        RegisterPacketClientbound<UpdateLightPacket>();
        RegisterPacketClientbound<WorldEventPacket>();
        RegisterPacketClientbound<CBPlayAddResourcePackPacket>();
        RegisterPacketClientbound<CBPlayRemoveResourcePackPacket>();
        RegisterPacketClientbound<DisplayObjectivePacket>();
        RegisterPacketClientbound<EndCombatPacket>();
        RegisterPacketClientbound<EnterCombatPacket>();
        RegisterPacketClientbound<LookAtPacket>();
        RegisterPacketClientbound<MerchantOffersPacket>();
        RegisterPacketClientbound<CBPlayMoveVehiclePacket>();
        RegisterPacketClientbound<OpenBookPacket>();
        RegisterPacketClientbound<OpenSignEditorPacket>();
        RegisterPacketClientbound<PlaceGhostRecipePacket>();
        RegisterPacketClientbound<CBPlayPlayerAbilitiesPacket>();
        RegisterPacketClientbound<RemoveEntityEffectPacket>();
        RegisterPacketClientbound<ResetScorePacket>();
        RegisterPacketClientbound<SelectAdvancementTabPacket>();
        RegisterPacketClientbound<ServerDataPacket>();
        RegisterPacketClientbound<SetActionBarTextPacket>();
        RegisterPacketClientbound<SetBorderCenterPacket>();
        RegisterPacketClientbound<SetBorderLerpSizePacket>();
        RegisterPacketClientbound<SetBorderSizePacket>();
        RegisterPacketClientbound<SetBorderWarningDelayPacket>();
        RegisterPacketClientbound<SetBorderWarningDistancePacket>();
        RegisterPacketClientbound<SetCameraPacket>();
        RegisterPacketClientbound<SetCenterChunkPacket>();
        RegisterPacketClientbound<SetDefaultSpawnPositionPacket>();
        RegisterPacketClientbound<SetHeadRotationPacket>();
        RegisterPacketClientbound<SetRenderDistancePacket>();
        RegisterPacketClientbound<UpdateRecipeBookPacket>();
        RegisterPacketClientbound<SetEntityMetadataPacket>();
        RegisterPacketClientbound<PickupItemPacket>();
        RegisterPacketClientbound<SetEquipmentPacket>();
        RegisterPacketClientbound<SetExperiencePacket>();
        RegisterPacketClientbound<SetSimulationDistancePacket>();
        RegisterPacketClientbound<SetSubtitleTextPacket>();
        RegisterPacketClientbound<SetTabListHeaderFooterPacket>();
        RegisterPacketClientbound<SetTickingStatePacket>();
        RegisterPacketClientbound<SetTitleAnimationTimesPacket>();
        RegisterPacketClientbound<SetTitleTextPacket>();
        RegisterPacketClientbound<StartConfigurationPacket>();
        RegisterPacketClientbound<StepTickPacket>();
        RegisterPacketClientbound<StopSoundPacket>();
        RegisterPacketClientbound<TagQueryResponsePacket>();
        RegisterPacketClientbound<UpdateObjectivesPacket>();
        RegisterPacketClientbound<UpdateScorePacket>();
        RegisterPacketClientbound<UpdateTeamsPacket>();
        RegisterPacketClientbound<UpdateTimePacket>();
        RegisterPacketClientbound<EntityEffectPacket>();
        RegisterPacketClientbound<UpdateAdvancementsPacket>();
        RegisterPacketClientbound<UpdateRecipesPacket>();
        RegisterPacketClientbound<CBPlayUpdateTagsPacket>();
        RegisterPacketClientbound<ExplosionPacket>();
        RegisterPacketClientbound<LinkEntitiesPacket>();
        RegisterPacketClientbound<SetContainerPropertyPacket>();
        RegisterPacketClientbound<CBPlayPingResponsePacket>();

        // SB
        RegisterPacketServerbound<SBKeepAlivePacket>();
        RegisterPacketServerbound<SetPlayerPositionPacket>();
        RegisterPacketServerbound<SetPlayerPositionAndRotationPacket>();
        RegisterPacketServerbound<ClientCommandPacket>();
        RegisterPacketServerbound<SBChatPacket>();
        RegisterPacketServerbound<SBChatMessagePacket>();
        RegisterPacketServerbound<ChatCommandPacket>();
        RegisterPacketServerbound<MessageAcknowledgementPacket>();
        RegisterPacketServerbound<PlayerSessionPacket>();
        RegisterPacketServerbound<ConfirmTeleportPacket>();
        RegisterPacketServerbound<CommandBlockUpdatePacket>();
        RegisterPacketServerbound<WindowClickPacket>();
        RegisterPacketServerbound<PlaceBlockPacket>();
        RegisterPacketServerbound<PlayerActionPacket>();
        RegisterPacketServerbound<SwingArmPacket>();
        RegisterPacketServerbound<InteractPacket>();
        RegisterPacketServerbound<SBCloseWindowPacket>();
        RegisterPacketServerbound<EntityActionPacket>();
        RegisterPacketServerbound<UseItemPacket>();
        RegisterPacketServerbound<SBSetHeldItemPacket>();
        RegisterPacketServerbound<ChunkBatchReceivedPacket>();
        RegisterPacketServerbound<SetCreativeSlotPacket>();
        RegisterPacketServerbound<PlayPongPacket>();
        RegisterPacketServerbound<PlayClientInformationPacket>();
        RegisterPacketServerbound<AcknowledgeConfigurationPacket>();
        RegisterPacketServerbound<ChangeContainerSlotStatePacket>();
        RegisterPacketServerbound<SBPlayChangeDifficultyPacket>();
        RegisterPacketServerbound<ChangeRecipeBookSettingsPacket>();
        RegisterPacketServerbound<CommandSuggestionsRequestPacket>();
        RegisterPacketServerbound<EditBookPacket>();
        RegisterPacketServerbound<JigsawGeneratePacket>();
        RegisterPacketServerbound<LockDifficultyPacket>();
        RegisterPacketServerbound<SBPlayMoveVehiclePacket>();
        RegisterPacketServerbound<PaddleBoatPacket>();
        RegisterPacketServerbound<PickItemPacket>();
        RegisterPacketServerbound<SBPlayPingRequestPacket>();
        RegisterPacketServerbound<PlaceRecipePacket>();
        RegisterPacketServerbound<SBPlayPlayerAbilitiesPacket>();
        RegisterPacketServerbound<PlayerInputPacket>();
        RegisterPacketServerbound<SBPlayPluginMessagePacket>();
        RegisterPacketServerbound<ProgramJigsawBlockPacket>();
        RegisterPacketServerbound<ProgramStructureBlockPacket>();
        RegisterPacketServerbound<QueryBlockEntityTagPacket>();
        RegisterPacketServerbound<QueryEntityTagPacket>();
        RegisterPacketServerbound<RenameItemPacket>();
        RegisterPacketServerbound<ResourcePackResponsePacket>();
        RegisterPacketServerbound<SeenAdvancementsPacket>();
        RegisterPacketServerbound<SelectTradePacket>();
        RegisterPacketServerbound<SetBeaconEffectPacket>();
        RegisterPacketServerbound<SetPlayerOnGroundPacket>();
        RegisterPacketServerbound<SetPlayerRotationPacket>();
        RegisterPacketServerbound<SetSeenRecipePacket>();
        RegisterPacketServerbound<TeleportToEntityPacket>();
        RegisterPacketServerbound<UpdateCommandBlockMinecartPacket>();
        RegisterPacketServerbound<UpdateSignPacket>();
        RegisterPacketServerbound<ClickContainerButtonPacket>();

        return (clientboundPacketFactories.ToFrozenDictionary(), serverboundPacketFactories.ToFrozenDictionary());
    }

}
