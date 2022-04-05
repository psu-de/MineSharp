using MineSharp.Core.Logging;
using MineSharp.Protocol.Events;
using MineSharp.Protocol.Packets.Clientbound.Login;
using MineSharp.Protocol.Packets.Clientbound.Play;
using MineSharp.Protocol.Packets.Clientbound.Status;
using MineSharp.Protocol.Packets.Serverbound.Handshaking;
using MineSharp.Protocol.Packets.Serverbound.Login;
using MineSharp.Protocol.Packets.Serverbound.Play;
using MineSharp.Protocol.Packets.Serverbound.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets {
    public abstract class Packet {
        public Packet() {

        }

        public abstract void Read(PacketBuffer buffer);
        public abstract void Write(PacketBuffer buffer);
        public virtual async Task Handle(MinecraftClient client) {
            //Logger.Debug("Received packet: " + this.GetType().Name);
            client.Events.InvokePacketReceived(client, this);
        }

        public virtual async Task Sent(MinecraftClient client) {
            client.Events.InvokePacketSent(client, this);
        }


        protected static Logger Logger = Logger.GetLogger();

        private static Dictionary<GameState, Dictionary<PacketFlow, Dictionary<int, Type?>>> IdToPacketType = new Dictionary<GameState, Dictionary<PacketFlow, Dictionary<int, Type?>>>();
        private static Dictionary<Type, int> PacketTypeToId = new Dictionary<Type, int>();

        private static void InitPacket(GameState state, PacketFlow flow, int id, Type type) {
            IdToPacketType[state][flow].Add(id, type);
            PacketTypeToId.Add(type, id);
        }


        public static void Initialize() {

            if (IdToPacketType.Count > 0) return;


            foreach (GameState state in Enum.GetValues(typeof(GameState))) {
                IdToPacketType.Add(state, new Dictionary<PacketFlow, Dictionary<int, Type?>>());

                IdToPacketType[state].Add(PacketFlow.SERVERBOUND, new Dictionary<int, Type?>());
                IdToPacketType[state].Add(PacketFlow.CLIENTBOUND, new Dictionary<int, Type?>());
            }

            // SERVERBOUND

            InitPacket(GameState.HANDSHAKING, PacketFlow.SERVERBOUND, 0x00, typeof(HandshakePacket)); // HANDSHAKE

            InitPacket(GameState.STATUS, PacketFlow.SERVERBOUND, 0x00, typeof(RequestPacket)); // STATUS
            InitPacket(GameState.STATUS, PacketFlow.SERVERBOUND, 0x01, typeof(Serverbound.Status.PingPacket));

            InitPacket(GameState.LOGIN, PacketFlow.SERVERBOUND, 0x00, typeof(LoginStartPacket)); // LOGIN
            InitPacket(GameState.LOGIN, PacketFlow.SERVERBOUND, 0x01, typeof(EncryptionResponsePacket));

            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x00, typeof(TeleportConfirmPacket));// PLAY
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x01, typeof(QueryBlockNBTPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x02, typeof(SetDifficultyPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x03, typeof(Serverbound.Play.ChatMessagePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x04, typeof(ClientStatusPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x05, typeof(ClientSettingsPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x06, typeof(Serverbound.Play.TabCompletePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x07, typeof(ClickWindowButtonPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x08, typeof(ClickWindowPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x09, typeof(Serverbound.Play.CloseWindowPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0A, typeof(Serverbound.Play.PluginMessagePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0B, typeof(EditBookPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0C, typeof(QueryEntityNBTPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0D, typeof(InteractEntityPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0E, typeof(GenerateStructurePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x0F, typeof(Serverbound.Play.KeepAlivePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x10, typeof(LockDifficultyPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x11, typeof(PlayerPositionPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x12, typeof(PlayerPositionAndRotationPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x13, typeof(PlayerRotationPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x14, typeof(PlayerMovementPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x15, typeof(Serverbound.Play.VehicleMovePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x16, typeof(SteerBoatPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x17, typeof(PickItemPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x18, typeof(CraftRecipeRequestPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x19, typeof(Serverbound.Play.PlayerAbilitiesPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1A, typeof(PlayerDiggingPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1B, typeof(EntityActionPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1C, typeof(SteerVehiclePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1D, typeof(Serverbound.Play.PongPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1E, typeof(SetRecipeBookStatePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x1F, typeof(SetDisplayedRecipePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x20, typeof(NameItemPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x21, typeof(ResourcePackStatusPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x22, typeof(AdvancementTabPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x23, typeof(SelectTradePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x24, typeof(SetBeaconEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x25, typeof(Serverbound.Play.HeldItemChangePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x27, typeof(UpdateCommandBlockMinecartPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x28, typeof(CreativeInventoryActionPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x29, typeof(UpdateJigsawBlockPacket));

            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x2B, typeof(UpdateSignPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x2C, typeof(AnimationPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x2D, typeof(SpectatePacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x2E, typeof(PlayerBlockPlacementPacket));
            InitPacket(GameState.PLAY, PacketFlow.SERVERBOUND, 0x2F, typeof(UseItemPacket));


            // CLIENTBOUND

            InitPacket(GameState.STATUS, PacketFlow.CLIENTBOUND, 0x00, typeof(ResponsePacket)); // STATUS
            InitPacket(GameState.STATUS, PacketFlow.CLIENTBOUND, 0x01, typeof(Clientbound.Status.PongPacket));

            InitPacket(GameState.LOGIN, PacketFlow.CLIENTBOUND, 0x00, typeof(Clientbound.Login.DisconnectPacket)); // LOGIN
            InitPacket(GameState.LOGIN, PacketFlow.CLIENTBOUND, 0x01, typeof(EncryptionRequestPacket));
            InitPacket(GameState.LOGIN, PacketFlow.CLIENTBOUND, 0x02, typeof(LoginSuccessPacket));
            InitPacket(GameState.LOGIN, PacketFlow.CLIENTBOUND, 0x03, typeof(SetCompressionPacket));

            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x00, typeof(SpawnEntityPacket)); // PLAY
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x01, typeof(SpawnExperienceOrbPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x02, typeof(SpawnLivingEntityPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x03, typeof(SpawnPaintingPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x04, typeof(SpawnPlayerPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x05, typeof(SculkVibrationSignalPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x06, typeof(EntityAnimationPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x07, typeof(StatisticsPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x08, typeof(AcknowledgePlayerDiggingPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x09, typeof(BlockBreakAnimationPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0A, typeof(BlockEntityDataPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0B, typeof(BlockActionPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0C, typeof(BlockChangePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0D, typeof(BossBarPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0E, typeof(ServerDifficultyPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x0F, typeof(Clientbound.Play.ChatMessagePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x10, typeof(ClearTitlesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x11, typeof(Clientbound.Play.TabCompletePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x12, typeof(DeclareCommandsPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x13, typeof(Clientbound.Play.CloseWindowPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x14, typeof(WindowItemsPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x15, typeof(WindowPropertyPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x16, typeof(SetSlotPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x17, typeof(SetCooldownPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x18, typeof(Clientbound.Play.PluginMessagePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x19, typeof(NamedSoundEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1A, typeof(Clientbound.Play.DisconnectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1B, typeof(EntityStatusPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1C, typeof(ExplosionPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1D, typeof(UnloadChunkPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1E, typeof(ChangeGameStatePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x1F, typeof(OpenHorseWindowPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x20, typeof(InitializeWorldBorderPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x21, typeof(Clientbound.Play.KeepAlivePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x22, typeof(ChunkDataAndLightUpdatePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x23, typeof(EffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x24, typeof(ParticlePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x25, typeof(UpdateLightPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x26, typeof(JoinGamePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x27, typeof(MapDataPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x28, typeof(TradeListPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x29, typeof(EntityPositionPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2A, typeof(EntityPositionandRotationPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2B, typeof(EntityRotationPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2C, typeof(Clientbound.Play.VehicleMovePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2D, typeof(OpenBookPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2E, typeof(OpenWindowPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x2F, typeof(OpenSignEditorPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x30, typeof(Clientbound.Play.PingPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x31, typeof(CraftRecipeResponsePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x32, typeof(Clientbound.Play.PlayerAbilitiesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x33, typeof(EndCombatEventPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x34, typeof(EnterCombatEventPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x35, typeof(DeathCombatEventPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x36, typeof(PlayerInfoPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x37, typeof(FacePlayerPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x38, typeof(PlayerPositionAndLookPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x39, typeof(UnlockRecipesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3A, typeof(DestroyEntitiesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3B, typeof(RemoveEntityEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3C, typeof(ResourcePackSendPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3D, typeof(RespawnPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3E, typeof(EntityHeadLookPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x3F, typeof(MultiBlockChangePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x40, typeof(SelectAdvancementTabPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x41, typeof(ActionBarPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x42, typeof(WorldBorderCenterPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x43, typeof(WorldBorderLerpSizePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x44, typeof(WorldBorderSizePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x45, typeof(WorldBorderWarningDelayPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x46, typeof(WorldBorderWarningReachPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x47, typeof(CameraPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x48, typeof(Clientbound.Play.HeldItemChangePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x49, typeof(UpdateViewPositionPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4A, typeof(UpdateViewDistancePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4B, typeof(SpawnPositionPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4C, typeof(DisplayScoreboardPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4D, typeof(EntityMetadataPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4E, typeof(AttachEntityPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x4F, typeof(EntityVelocityPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x50, typeof(EntityEquipmentPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x51, typeof(SetExperiencePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x52, typeof(UpdateHealthPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x53, typeof(ScoreboardObjectivePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x54, typeof(SetPassengersPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x55, typeof(TeamsPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x56, typeof(UpdateScorePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x57, typeof(UpdateSimulationDistancePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x58, typeof(SetTitleSubTitlePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x59, typeof(TimeUpdatePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5A, typeof(SetTitleTextPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5B, typeof(SetTitleTimesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5C, typeof(EntitySoundEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5D, typeof(SoundEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5E, typeof(StopSoundPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x5F, typeof(PlayerListHeaderAndFooterPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x60, typeof(NBTQueryResponsePacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x61, typeof(CollectItemPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x62, typeof(EntityTeleportPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x63, typeof(AdvancementsPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x64, typeof(EntityPropertiesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x65, typeof(EntityEffectPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x66, typeof(DeclareRecipesPacket));
            InitPacket(GameState.PLAY, PacketFlow.CLIENTBOUND, 0x67, typeof(TagsPacket));



            // Fill rest with unknown packets

            for (int i = 0; i < 0x68; i++) {
                if (!IdToPacketType[GameState.PLAY][PacketFlow.CLIENTBOUND].ContainsKey(i)) {
                    Logger.Warning($"Clientbound Packet 0x{i.ToString("X2")} is not implemented!");
                    IdToPacketType[GameState.PLAY][PacketFlow.CLIENTBOUND][i] = null;
                }
            }
        }

        public static Type? GetPacketType(GameState state, PacketFlow direction, int id) {
            try {
                return IdToPacketType[state][direction][id];
            } catch {
                Console.WriteLine("Unknown packet id: 0x" + id.ToString("X2"));
                //Logger.Error($"Unknown packet: State={Enum.GetName(typeof(GameState), state)}, Direction={Enum.GetName(typeof(PacketFlow), direction)}, Id={id.ToString("X2")}");
                return null;
            }
        }

        public static int GetPacketId(Type packetType) {
            try {
                return PacketTypeToId[packetType];
            } catch (Exception ex) {
                Logger.Error($"Unkown packet type: Type={packetType.FullName}");
                throw new Exception("Unknown packet type", ex);
            }
        }
    }
}
