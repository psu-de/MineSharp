
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;
using MineSharp.Protocol.Packets.Serverbound.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {


        //TODO: Events
        
        public Player? BotEntity => this.BaseModule?.BotEntity;
        public MinecraftPlayer? Player => this.BaseModule?.Player;

        public float Health => BaseModule.Health;
        public bool IsAlive => BaseModule.IsAlive;
        public float Food => BaseModule.Food;
        public float Saturation => BaseModule.Saturation;
        public Identifier CurrentDimension => BaseModule.CurrentDimension;
        public GameMode GameMode => BaseModule.GameMode;

        public Task WaitForBot() => this.BaseModule.WaitForBot();

    }
}
