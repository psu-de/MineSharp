using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using System.Collections.Concurrent;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoPacket;

namespace MineSharp.Bot {

    /// <summary>
    /// This partial class keeps track of the Entities in the world
    /// </summary>
    public partial class Bot {


        public delegate void BotPlayerEvent(Player entity);
        public delegate void BotEntityEvent(Entity entity);

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

        /// <summary>
        /// Fires when an entity spawned in the players View Distance or when a player walks into View Distance
        /// </summary>
        public event BotEntityEvent EntitySpawned;

        /// <summary>
        /// Fires when an entity despawned in the players View Distance
        /// </summary>
        public event BotEntityEvent EntityDespawned;

        public Player Player { get; private set; }
        private bool isPlayerLoaded = false;

        /// <summary>
        /// All living Entities in range
        /// </summary>
        public ConcurrentDictionary<int, Entity> EntitiesMapping { get; private set; }

        public ConcurrentDictionary<UUID, Player> PlayerMapping;
        public List<Player> PlayerList => PlayerMapping.Values.ToList();


        private partial void LoadEntities() {
            this.EntitiesMapping = new ConcurrentDictionary<int, Entity>();
            this.PlayerMapping = new ConcurrentDictionary<UUID, Player>();
        }

        private void InitPlayer(Protocol.Packets.Clientbound.Play.JoinGamePacket packet1, Protocol.Packets.Clientbound.Play.PlayerPositionAndLookPacket packet2) {
            this.Player = new Player(this.Options.UsernameOrEmail, this.Session.UUID, 0, packet1.Gamemode, packet1.EntityID, new Vector3( packet2.X, packet2.Y, packet2.Z), packet2.Pitch, packet2.Yaw);
            Logger.Info($"Initialized Player entity: Location=({packet2.X} / {packet2.Y} / {packet2.Z})");
            isPlayerLoaded = true;

            this.Client.SendPacket(new Protocol.Packets.Serverbound.Play.PlayerPositionAndRotationPacket(
                packet2.X, packet2.Y, packet2.Z, packet2.Yaw, packet2.Pitch, true)); //TODO: Change onGround=true
        }



        private void AddEntity(Entity entity) {
            if (!this.EntitiesMapping.TryAdd(entity.Id, entity)) { Logger.Debug("Could not add entity"); return; }
            this.EntitySpawned?.Invoke(entity);
        }

        private void handlePlayerInfo(Protocol.Packets.Clientbound.Play.PlayerInfoPacket packet) {

            switch (packet.Action) {
                case PlayerInfoAction.AddPlayer: //TODO: Nich ganz korrekt, wird auch gesendet wenn davor schon spieler aufm server waren
                    foreach ((UUID uuid, object? data) in packet.Players) {
                        PlayerInfo dat = (PlayerInfo)data;
                        string name = dat.Name;
                        GameMode gm = dat.GameMode;
                        int ping = dat.Ping;
                        Player newPlayer = new Player(name, uuid, ping, gm, -1, new Vector3( double.NaN, double.NaN, double.NaN), float.NaN, float.NaN);
                        if (!PlayerMapping.TryAdd(uuid, newPlayer)) { Logger.Debug("Could not add player"); return; }
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
        }


        private void handleSpawnPlayer(Protocol.Packets.Clientbound.Play.SpawnPlayerPacket packet) {
            Player player;
            if (!PlayerMapping.TryGetValue(packet.PlayerUUID, out player)) {
                return;
            }

            player.Position = new Vector3(packet.X, packet.Y, packet.Z);
            player.Yaw = packet.Yaw;
            player.Pitch = packet.Pitch;
            player.Id = packet.EntityID;

            this.AddEntity(player);
            PlayerLoaded?.Invoke(player);
        }

        private void handleSpawnLivingEntity(Protocol.Packets.Clientbound.Play.SpawnLivingEntityPacket packet) {
            Entity newEntity = new Entity(Entities.EntitiesByType[packet.Type], packet.EntityId, new Vector3(packet.X, packet.Y, packet.Z), packet.Pitch, packet.Yaw, new Vector3(packet.VelocityX, packet.VelocityY, packet.VelocityZ));
            AddEntity(newEntity);
        }

        private void handleDespawnEntity(Protocol.Packets.Clientbound.Play.DestroyEntitiesPacket packet) {
            for (int i = 0; i < packet.EntityIds.Length; i++) {
                Entity entity;
                if (!this.EntitiesMapping.TryRemove(packet.EntityIds[i], out entity)) { continue; }
                this.EntityDespawned?.Invoke(entity);

            }
        }
    }
}
