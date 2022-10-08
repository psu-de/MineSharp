using MineSharp.Core.Types.Enums;

namespace MineSharp.Core.Types
{
    public class MinecraftPlayer
    {

        public MinecraftPlayer(string username, UUID uuid, int ping, GameMode gamemode, Entity entity)
        {
            this.Username = username;
            this.UUID = uuid;
            this.Ping = ping;
            this.GameMode = gamemode;
            this.Entity = entity;
        }

        public string Username { get; set; }
        public UUID UUID { get; set; }
        public int Ping { get; set; }
        public GameMode GameMode { get; set; }
        public Entity Entity { get; set; }

        public Vector3 GetHeadPosition() => this.Entity.Position.Plus(Vector3.Up);
    }
}
