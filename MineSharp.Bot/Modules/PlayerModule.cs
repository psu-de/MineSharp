using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.T4.Entities;
using MineSharp.Protocol.Packets.Clientbound.Play;
using System.Collections.Concurrent;
using static MineSharp.Bot.MinecraftBot;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoPacket;

namespace MineSharp.Bot.Modules {
    public class PlayerModule : Module {

        public PlayerModule(MinecraftBot bot) : base(bot) { }

        public ConcurrentDictionary<UUID, MinecraftPlayer> PlayerMapping = new ConcurrentDictionary<UUID, MinecraftPlayer>();

        public bool IsRaining { get; private set; }
        public float RainLevel { get; private set; }
        public float ThunderLevel { get; private set; }

        /// <summary>
        /// Fires whenever the weather has changed
        /// </summary>
        public event BotEmptyEvent WeatherChanged;

        /// <summary>
        /// Fires when a player joins the server (also fires when the bot itself joins the server). Warning: Entity is probably not loaded when this event fires!
        /// </summary>
        public event BotPlayerEvent PlayerJoined;

        /// <summary>
        /// Fires when a players leaves the server. 
        /// </summary>
        public event BotPlayerEvent PlayerLeft;

        /// <summary>
        /// Fires when a player comes into View Distance of the bot and (Position, etc...) has been loaded (<see cref="Protocol.Packets.Clientbound.Play.SpawnPlayerPacket"/>)
        /// </summary>
        public event BotPlayerEvent PlayerLoaded;

        protected override async Task Load() {

            this.Bot.On<PlayerInfoPacket>(handlePlayerInfo);
            this.Bot.On<SpawnPlayerPacket>(handleSpawnPlayer);
            this.Bot.On<PlayerPositionAndLookPacket>(handlePlayerPositionAndLook);
            this.Bot.On<ChangeGameStatePacket>(handleChangeGameState);

            await this.SetEnabled(true);
        }

        private Task handleChangeGameState(ChangeGameStatePacket packet) {

            //TODO: Implement all GameState Reasons
            switch (packet.Reason) {
                case GameStateReason.BeginRaining:
                    IsRaining = true;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.EndRaining:
                    IsRaining = false;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.RainLevelChanged:
                    RainLevel = packet.Value;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.ThunderLevelChanged:
                    ThunderLevel = packet.Value;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.ChangeGameMode:
                    var gameMode = (GameMode)packet.Value;
                    this.Bot.Player!.GameMode = gameMode;
                    break;
            }

            return Task.CompletedTask;
        }

        private Task handlePlayerInfo(PlayerInfoPacket packet) {
            switch (packet.Action) {
                case PlayerInfoAction.AddPlayer: //TODO: Nich ganz korrekt, wird auch gesendet wenn davor schon spieler aufm server waren
                    foreach ((UUID uuid, object? data) in packet.Players) {

                        PlayerInfo dat = (PlayerInfo)data;
                        string name = dat.Name;
                        if (name == Bot.Session.Username) continue;

                        GameMode gm = dat.GameMode;
                        int ping = dat.Ping;
                        if (this.PlayerMapping.ContainsKey(uuid)) {
                            this.PlayerMapping[uuid].Username = name;
                            this.PlayerMapping[uuid].Ping = ping;
                            this.PlayerMapping[uuid].GameMode = gm;
                            continue;
                        }

                        MinecraftPlayer newPlayer = new MinecraftPlayer(name, uuid, ping, gm, new Player(-1, new Vector3(double.NaN, double.NaN, double.NaN), float.NaN, float.NaN, Vector3.Zero, true, new Dictionary<int, Effect?>()));
                        if (!PlayerMapping.TryAdd(uuid, newPlayer)) { Logger.Debug("Could not add player"); return Task.CompletedTask; }
                        PlayerJoined?.Invoke(this.Bot, newPlayer);
                    }
                    break;
                case PlayerInfoAction.RemovePlayer:
                    foreach ((UUID uuid, object? data) in packet.Players) {
                        if (!PlayerMapping.TryRemove(uuid, out var player)) continue;
                        PlayerLeft?.Invoke(this.Bot, player);
                    }
                    break;
            }
            return Task.CompletedTask;
        }

        private Task handleSpawnPlayer(SpawnPlayerPacket packet) {
            if (!PlayerMapping.TryGetValue(packet.PlayerUUID, out var player)) {
                return Task.CompletedTask;
            }

            player.Entity.Position = new Vector3(packet.X, packet.Y, packet.Z);
            player.Entity.Yaw = packet.Yaw;
            player.Entity.Pitch = packet.Pitch;
            player.Entity.ServerId = packet.EntityID;

            this.Bot.EntityModule.AddEntity(player.Entity);
            PlayerLoaded?.Invoke(this.Bot, player);
            return Task.CompletedTask;
        }

        private async Task handlePlayerPositionAndLook(PlayerPositionAndLookPacket packet) {

            if ((packet.Flags & 0x01) == 0x01) this.Bot.BotEntity.Position.X += packet.X;
            else this.Bot.BotEntity.Position.X = packet.X;

            if ((packet.Flags & 0x02) == 0x02) this.Bot.BotEntity.Position.Y += packet.Y;
            else this.Bot.BotEntity.Position.Y = packet.Y;

            if ((packet.Flags & 0x04) == 0x04) this.Bot.BotEntity.Position.Z += packet.Z;
            else this.Bot.BotEntity.Position.Z = packet.Z;

            if ((packet.Flags & 0x08) == 0x08) this.Bot.BotEntity.Pitch += packet.Pitch;
            else this.Bot.BotEntity.Pitch = packet.Pitch;

            if ((packet.Flags & 0x10) == 0x10) this.Bot.BotEntity.Yaw += packet.Yaw;
            else this.Bot.BotEntity.Yaw = packet.Yaw;

            // TODO: Dismount Vehicle

            await this.Bot.Client.SendPacket(new Protocol.Packets.Serverbound.Play.TeleportConfirmPacket(packet.TeleportID));
        }
    }
}
