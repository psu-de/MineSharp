using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Entities {
    public class Player : Entity {

        public string Username { get; set; }
        public UUID UUID { get; set; }
        public int Ping { get; set; }
        public GameMode GameMode { get; set; }

        public Player(string username, UUID uuid, int ping, GameMode gamemode, int id, Vector3 pos, float pitch, float yaw) : base(Entities.PLAYER, id, pos, pitch, yaw, Vector3.Zero, true) {
            this.Username = username;
            this.UUID = uuid;
            this.Ping = ping;
            this.GameMode = gamemode;
        }

        public Vector3 GetHeadPosition() {
            return this.Position.Plus(Vector3.Up);
        }
    }
}
