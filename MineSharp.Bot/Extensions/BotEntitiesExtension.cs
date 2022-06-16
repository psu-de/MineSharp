using MineSharp.Core.Types;
using MineSharp.Data.Entities;
using System.Collections.Concurrent;

namespace MineSharp.Bot {

    /// <summary>
    /// This partial class keeps track of the Entities in the world
    /// </summary>
    public partial class MinecraftBot {



        /// <summary>
        /// All living Entities in range
        /// </summary>
        public ConcurrentDictionary<int, Entity> Entities => EntityModule.Entities;

        public ConcurrentDictionary<UUID, MinecraftPlayer> PlayerMapping => PlayerModule.PlayerMapping;
        public List<MinecraftPlayer> PlayerList => PlayerMapping.Values.ToList();
    }
}
