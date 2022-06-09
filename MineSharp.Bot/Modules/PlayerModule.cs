using MineSharp.Data.Entities;
using MineSharp.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineSharp.Protocol.Packets.Clientbound.Play;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoPacket;
using MineSharp.Core.Types.Enums;
using static MineSharp.Bot.MinecraftBot;

namespace MineSharp.Bot.Modules {
    public class PlayerModule : Module {

        public PlayerModule(MinecraftBot bot) : base(bot) { }

        public ConcurrentDictionary<UUID, Player> PlayerMapping = new ConcurrentDictionary<UUID, Player>();

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
                    WeatherChanged?.Invoke();
                    break;
                case GameStateReason.EndRaining:
                    IsRaining = false;
                    WeatherChanged?.Invoke();
                    break;
                case GameStateReason.RainLevelChanged:
                    RainLevel = packet.Value;
                    WeatherChanged?.Invoke();
                    break;
                case GameStateReason.ThunderLevelChanged:
                    ThunderLevel = packet.Value;
                    WeatherChanged?.Invoke();
                    break;
                case GameStateReason.ChangeGameMode:
                    var gameMode = (GameMode)packet.Value;
                    this.Bot.BotEntity.GameMode = gameMode;
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

                        Player newPlayer = new Player(name, uuid, ping, gm, -1, new Vector3(double.NaN, double.NaN, double.NaN), float.NaN, float.NaN);
                        if (!PlayerMapping.TryAdd(uuid, newPlayer)) { Logger.Debug("Could not add player"); return Task.CompletedTask; }
                        PlayerJoined?.Invoke(newPlayer);
                    }
                    break;
                case PlayerInfoAction.RemovePlayer:
                    foreach ((UUID uuid, object? data) in packet.Players) {
                        Player player;
                        if (!PlayerMapping.TryRemove(uuid, out player)) continue;
                        PlayerLeft?.Invoke(player);
                    }
                    break;
            }
            return Task.CompletedTask;
        }

        private Task handleSpawnPlayer(SpawnPlayerPacket packet) {
            if (!PlayerMapping.TryGetValue(packet.PlayerUUID, out var player)) {
                return Task.CompletedTask;
            }

            player.Position = new Vector3(packet.X, packet.Y, packet.Z);
            player.Yaw = packet.Yaw;
            player.Pitch = packet.Pitch;
            player.Id = packet.EntityID;

            this.Bot.EntityModule.AddEntity(player);
            PlayerLoaded?.Invoke(player);
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
