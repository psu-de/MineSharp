using MineSharp.Core;
using MineSharp.Core.Types;
using MineSharp.Core.Types.Enums;
using MineSharp.Data.Effects;
using MineSharp.Data.Entities;
using System.Collections.Concurrent;
using static MineSharp.Protocol.Packets.Clientbound.Play.PlayerInfoPacket;

namespace MineSharp.Bot {

    /// <summary>
    /// This partial class keeps track of the Entities in the world
    /// </summary>
    public partial class MinecraftBot {



        /// <summary>
        /// All living Entities in range
        /// </summary>
        public ConcurrentDictionary<int, Entity> Entities => EntityModule.Entities;

        public ConcurrentDictionary<UUID, Player> PlayerMapping => PlayerModule.PlayerMapping;
        public List<Player> PlayerList => PlayerMapping.Values.ToList();
    }
}
