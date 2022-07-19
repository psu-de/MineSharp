using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Data.Protocol.Play.Clientbound;
using System.Collections.Concurrent;
using static MineSharp.Bot.MinecraftBot;

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
        public event BotEmptyEvent? WeatherChanged;

        /// <summary>
        /// Fires when a player joins the server (also fires when the bot itself joins the server). Warning: Entity is probably not loaded when this event fires!
        /// </summary>
        public event BotPlayerEvent? PlayerJoined;

        /// <summary>
        /// Fires when a players leaves the server. 
        /// </summary>
        public event BotPlayerEvent? PlayerLeft;

        /// <summary>
        /// Fires when a player comes into View Distance of the bot and (Position, etc...) has been loaded (<see cref="Protocol.Packets.Clientbound.Play.SpawnPlayerPacket"/>)
        /// </summary>
        public event BotPlayerEvent? PlayerLoaded;

        protected override async Task Load() {

            this.Bot.On<PacketPlayerInfo>(handlePlayerInfo);
            this.Bot.On<PacketNamedEntitySpawn>(handleSpawnPlayer);
            this.Bot.On<PacketPosition>(handlePlayerPositionAndLook);
            this.Bot.On<PacketGameStateChange>(handleChangeGameState);

            await this.SetEnabled(true);
        }

        private Task handleChangeGameState(PacketGameStateChange packet) {

            //TODO: Implement all GameState Reasons
            switch ((GameStateReason)packet.Reason!) {
                case GameStateReason.BeginRaining:
                    IsRaining = true;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.EndRaining:
                    IsRaining = false;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.RainLevelChanged:
                    RainLevel = packet.Reason!;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.ThunderLevelChanged:
                    ThunderLevel = packet.Reason!;
                    WeatherChanged?.Invoke(this.Bot);
                    break;
                case GameStateReason.ChangeGameMode:
                    var gameMode = (GameMode)packet.Reason!;
                    this.Bot.Player!.GameMode = gameMode;
                    break;
            }

            return Task.CompletedTask;
        }

        private Task handlePlayerInfo(PacketPlayerInfo packet) {
            switch (packet.Action!.Value) {
                case 0: // Add player //TODO: Nich ganz korrekt, wird auch gesendet wenn davor schon spieler aufm server waren
                    foreach (var data in packet.Data!) {

                        
                        string name = (string)data.Name!.Value!;
                        if (name == Bot.Session.Username) continue;

                        GameMode gm = (GameMode)((Data.Protocol.VarInt)data.Gamemode!.Value!).Value;
                        int ping = (Data.Protocol.VarInt)data.Ping!.Value!;
                        if (this.PlayerMapping.ContainsKey(data.UUID!)) {
                            this.PlayerMapping[data.UUID!].Username = name;
                            this.PlayerMapping[data.UUID!].Ping = ping;
                            this.PlayerMapping[data.UUID!].GameMode = gm;
                            continue;
                        }

                        MinecraftPlayer newPlayer = new MinecraftPlayer(name, data.UUID!, ping, gm, new Player(-1, new Vector3(double.NaN, double.NaN, double.NaN), float.NaN, float.NaN, Vector3.Zero, true, new Dictionary<int, Effect?>()));
                        if (!PlayerMapping.TryAdd(data.UUID!, newPlayer)) { Logger.Debug("Could not add player"); return Task.CompletedTask; }
                        PlayerJoined?.Invoke(this.Bot, newPlayer);
                    }
                    break;
                case 4: // Remove player:
                    foreach (var data in packet.Data!) {
                        if (!PlayerMapping.TryRemove(data.UUID!, out var player)) continue;
                        PlayerLeft?.Invoke(this.Bot, player);
                    }
                    break;
            }
            return Task.CompletedTask;
        }

        private Task handleSpawnPlayer(PacketNamedEntitySpawn packet) {
            if (!PlayerMapping.TryGetValue(packet.PlayerUUID!, out var player)) {
                return Task.CompletedTask;
            }

            player.Entity.Position = new Vector3(packet.X!, packet.Y!, packet.Z!);
            player.Entity.Yaw = packet.Yaw!;
            player.Entity.Pitch = packet.Pitch!;
            player.Entity.ServerId = packet.EntityId!;

            this.Bot.EntityModule!.AddEntity(player.Entity);
            PlayerLoaded?.Invoke(this.Bot, player);
            return Task.CompletedTask;
        }

        private async Task handlePlayerPositionAndLook(PacketPosition packet) {

            if ((packet.Flags & 0x01) == 0x01) this.Bot.BotEntity!.Position.X += packet.X!;
            else this.Bot.BotEntity!.Position.X = packet.X!;

            if ((packet.Flags & 0x02) == 0x02) this.Bot.BotEntity.Position.Y += packet.Y!;
            else this.Bot.BotEntity.Position.Y = packet.Y!;

            if ((packet.Flags & 0x04) == 0x04) this.Bot.BotEntity.Position.Z += packet.Z!;
            else this.Bot.BotEntity.Position.Z = packet.Z!;

            if ((packet.Flags & 0x08) == 0x08) this.Bot.BotEntity.Pitch += packet.Pitch!;
            else this.Bot.BotEntity.Pitch = packet.Pitch!;

            if ((packet.Flags & 0x10) == 0x10) this.Bot.BotEntity.Yaw += packet.Yaw!;
            else this.Bot.BotEntity.Yaw = packet.Yaw!;

            // TODO: Dismount Vehicle

            await this.Bot.Client.SendPacket(new Data.Protocol.Play.Serverbound.PacketTeleportConfirm(packet.TeleportId!));
        }
    }
}
