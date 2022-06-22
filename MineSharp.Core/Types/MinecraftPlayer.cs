using MineSharp.Core.Types.Enums;

namespace MineSharp.Core.Types {
    public class MinecraftPlayer {

        public string Username { get; set; }
        public UUID UUID { get; set; }
        public int Ping { get; set; }
        public GameMode GameMode { get; set; }
        public Entity Entity { get; set; }

        public MinecraftPlayer(string username, UUID uuid, int ping, GameMode gamemode, Entity entity) {
            this.Username = username;
            this.UUID = uuid;
            this.Ping = ping;
            this.GameMode = gamemode;
            this.Entity = entity;
        }

        public Vector3 GetHeadPosition() {
            return this.Entity.Position.Plus(Vector3.Up);
        }

    }
}
