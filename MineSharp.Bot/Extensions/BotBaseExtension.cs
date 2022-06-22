
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Entities;

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

        [BotFunction("Basic", "Waits until the bot entity has loaded. BotEntity has been set at this point.")]
        public Task WaitForBot() => this.BaseModule.WaitForBot();

    }
}
