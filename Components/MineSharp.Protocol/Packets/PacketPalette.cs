﻿using System.Collections.Frozen;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.Clientbound.Configuration;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
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
        // CB
        RegisterPacket<LoginDisconnectPacket>();
        RegisterPacket<EncryptionRequestPacket>();
        RegisterPacket<LoginSuccessPacket>();
        RegisterPacket<SetCompressionPacket>();
        RegisterPacket<LoginPluginRequestPacket>();

        // SB
        RegisterPacket<LoginStartPacket>();
        RegisterPacket<EncryptionResponsePacket>();
        RegisterPacket<LoginPluginResponsePacket>();
        RegisterPacket<LoginAcknowledgedPacket>();

        // Status
        // CB
        RegisterPacket<StatusResponsePacket>();
        RegisterPacket<CBStatusPingResponsePacket>();

        // SB
        RegisterPacket<StatusRequestPacket>();
        RegisterPacket<StatusPingRequestPacket>();

        // Configuration
        // CB
        RegisterPacket<CBConfigurationPluginMessagePacket>();
        RegisterPacket<ConfigurationDisconnectPacket>();
        RegisterPacket<CBFinishConfigurationPacket>();
        RegisterPacket<CBConfigurationKeepAlivePacket>();
        RegisterPacket<ConfPingPacket>();
        RegisterPacket<RegistryDataPacket>();
        RegisterPacket<FeatureFlagsPacket>();
        RegisterPacket<CBConfigurationAddResourcePackPacket>();
        RegisterPacket<CBConfigurationRemoveResourcePackPacket>();
        RegisterPacket<CBConfigurationUpdateTagsPacket>();

        // SB
        RegisterPacket<ConfClientInformationPacket>();
        RegisterPacket<SBConfigurationPluginMessagePacket>();
        RegisterPacket<SBFinishConfigurationPacket>();
        RegisterPacket<SBConfigurationKeepAlivePacket>();
        RegisterPacket<ConfPongPacket>();
        RegisterPacket<SBConfigurationResourcePackResponsePacket>();

        // Play
        // CB
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
        RegisterPacket<AwardStatisticsPacket>();
        RegisterPacket<BlockActionPacket>();
        RegisterPacket<BlockEntityDataPacket>();
        RegisterPacket<BossBarPacket>();
        RegisterPacket<CBPlayChangeDifficultyPacket>();
        RegisterPacket<ChatSuggestionsPacket>();
        RegisterPacket<ChunkBiomesPacket>();
        RegisterPacket<ClearTitlesPacket>();
        RegisterPacket<CBPlayPluginMessagePacket>();
        RegisterPacket<CommandSuggestionsResponsePacket>();
        RegisterPacket<DamageEventPacket>();
        RegisterPacket<DeleteMessagePacket>();
        RegisterPacket<EntityAnimationPacket>();
        RegisterPacket<SetBlockDestroyStagePacket>();
        RegisterPacket<SetCooldownPacket>();
        RegisterPacket<SpawnExperienceOrbPacket>();
        RegisterPacket<HurtAnimationPacket>();
        RegisterPacket<InitializeWorldBorderPacket>();
        RegisterPacket<MapDataPacket>();
        RegisterPacket<OpenHorseScreenPacket>();
        RegisterPacket<UpdateLightPacket>();
        RegisterPacket<WorldEventPacket>();
        RegisterPacket<CBPlayAddResourcePackPacket>();
        RegisterPacket<CBPlayRemoveResourcePackPacket>();
        RegisterPacket<DisplayObjectivePacket>();
        RegisterPacket<EndCombatPacket>();
        RegisterPacket<EnterCombatPacket>();
        RegisterPacket<LookAtPacket>();
        RegisterPacket<MerchantOffersPacket>();
        RegisterPacket<CBPlayMoveVehiclePacket>();
        RegisterPacket<OpenBookPacket>();
        RegisterPacket<OpenSignEditorPacket>();
        RegisterPacket<PlaceGhostRecipePacket>();
        RegisterPacket<CBPlayPlayerAbilitiesPacket>();
        RegisterPacket<RemoveEntityEffectPacket>();
        RegisterPacket<ResetScorePacket>();
        RegisterPacket<SelectAdvancementTabPacket>();
        RegisterPacket<ServerDataPacket>();
        RegisterPacket<SetActionBarTextPacket>();
        RegisterPacket<SetBorderCenterPacket>();
        RegisterPacket<SetBorderLerpSizePacket>();
        RegisterPacket<SetBorderSizePacket>();
        RegisterPacket<SetBorderWarningDelayPacket>();
        RegisterPacket<SetBorderWarningDistancePacket>();
        RegisterPacket<SetCameraPacket>();
        RegisterPacket<SetCenterChunkPacket>();
        RegisterPacket<SetDefaultSpawnPositionPacket>();
        RegisterPacket<SetHeadRotationPacket>();
        RegisterPacket<SetRenderDistancePacket>();
        RegisterPacket<UpdateRecipeBookPacket>();
        RegisterPacket<SetEntityMetadataPacket>();
        RegisterPacket<PickupItemPacket>();
        RegisterPacket<SetEquipmentPacket>();
        RegisterPacket<SetExperiencePacket>();
        RegisterPacket<SetSimulationDistancePacket>();
        RegisterPacket<SetSubtitleTextPacket>();
        RegisterPacket<SetTabListHeaderFooterPacket>();
        RegisterPacket<SetTickingStatePacket>();
        RegisterPacket<SetTitleAnimationTimesPacket>();
        RegisterPacket<SetTitleTextPacket>();
        RegisterPacket<StartConfigurationPacket>();
        RegisterPacket<StepTickPacket>();
        RegisterPacket<StopSoundPacket>();
        RegisterPacket<TagQueryResponsePacket>();
        RegisterPacket<UpdateObjectivesPacket>();
        RegisterPacket<UpdateScorePacket>();
        RegisterPacket<UpdateTeamsPacket>();
        RegisterPacket<UpdateTimePacket>();
        RegisterPacket<EntityEffectPacket>();
        RegisterPacket<UpdateAdvancementsPacket>();
        RegisterPacket<UpdateRecipesPacket>();
        RegisterPacket<CBPlayUpdateTagsPacket>();
        RegisterPacket<ExplosionPacket>();
        RegisterPacket<LinkEntitiesPacket>();
        RegisterPacket<SetContainerPropertyPacket>();
        RegisterPacket<CBPlayPingResponsePacket>();

        // SB
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
        RegisterPacket<CommandBlockUpdatePacket>();
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
        RegisterPacket<PlayClientInformationPacket>();
        RegisterPacket<AcknowledgeConfigurationPacket>();
        RegisterPacket<ChangeContainerSlotStatePacket>();
        RegisterPacket<SBPlayChangeDifficultyPacket>();
        RegisterPacket<ChangeRecipeBookSettingsPacket>();
        RegisterPacket<CommandSuggestionsRequestPacket>();
        RegisterPacket<EditBookPacket>();
        RegisterPacket<JigsawGeneratePacket>();
        RegisterPacket<LockDifficultyPacket>();
        RegisterPacket<SBPlayMoveVehiclePacket>();
        RegisterPacket<PaddleBoatPacket>();
        RegisterPacket<PickItemPacket>();
        RegisterPacket<SBPlayPingRequestPacket>();
        RegisterPacket<PlaceRecipePacket>();
        RegisterPacket<SBPlayPlayerAbilitiesPacket>();
        RegisterPacket<PlayerInputPacket>();
        RegisterPacket<SBPlayPluginMessagePacket>();
        RegisterPacket<ProgramJigsawBlockPacket>();
        RegisterPacket<ProgramStructureBlockPacket>();
        RegisterPacket<QueryBlockEntityTagPacket>();
        RegisterPacket<QueryEntityTagPacket>();
        RegisterPacket<RenameItemPacket>();
        RegisterPacket<ResourcePackResponsePacket>();
        RegisterPacket<SeenAdvancementsPacket>();
        RegisterPacket<SelectTradePacket>();
        RegisterPacket<SetBeaconEffectPacket>();
        RegisterPacket<SetPlayerOnGroundPacket>();
        RegisterPacket<SetPlayerRotationPacket>();
        RegisterPacket<SetSeenRecipePacket>();
        RegisterPacket<TeleportToEntityPacket>();
        RegisterPacket<UpdateCommandBlockMinecartPacket>();
        RegisterPacket<UpdateSignPacket>();
        RegisterPacket<ClickContainerButtonPacket>();

        return packetFactories.ToFrozenDictionary();
    }

}
