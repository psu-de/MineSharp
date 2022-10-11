using MineSharp.Core.Types;
using System.Collections.Concurrent;

namespace MineSharp.Bot
{
    public partial class MinecraftBot
    {
        public ConcurrentDictionary<UUID, MinecraftPlayer> PlayerMapping => this.PlayerModule!.PlayerMapping;
        public List<MinecraftPlayer> PlayerList => this.PlayerMapping.Values.ToList();

        /// <summary>
        /// Whether it is raining in the minecraft world
        /// </summary>
        public bool IsRaining => this.PlayerModule!.IsRaining;

        /// <summary>
        /// Rain level in the minecraft world
        /// </summary>
        public float RainLevel => this.PlayerModule!.RainLevel;

        /// <summary>
        /// Thunder level in the minecraft world
        /// </summary>
        public float ThunderLevel => this.PlayerModule!.ThunderLevel;
        
        public event BotEmptyEvent? WeatherChanged {
            add => this.PlayerModule!.WeatherChanged += value;
            remove => this.PlayerModule!.WeatherChanged -= value;
        }
        
        public event BotPlayerEvent? PlayerJoined {
            add => this.PlayerModule!.PlayerJoined += value;
            remove => this.PlayerModule!.PlayerJoined -= value;
        }
        
        public event BotPlayerEvent? PlayerLeft {
            add => this.PlayerModule!.PlayerLeft += value;
            remove => this.PlayerModule!.PlayerLeft -= value;
        }
        
        public event BotPlayerEvent? PlayerLoaded {
            add => this.PlayerModule!.PlayerLoaded += value;
            remove => this.PlayerModule!.PlayerLoaded -= value;
        }
    }
}
