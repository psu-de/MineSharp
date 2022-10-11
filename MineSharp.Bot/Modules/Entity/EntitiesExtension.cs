using MineSharp.Core.Types;
using System.Collections.Concurrent;

namespace MineSharp.Bot
{

    /// <summary>
    ///     This partial class keeps track of the Entities in the world
    /// </summary>
    public partial class MinecraftBot
    {


        /// <summary>
        ///     All living Entities in range
        /// </summary>
        public ConcurrentDictionary<int, Entity> Entities => this.EntityModule!.Entities;


        /// <summary>
        ///     Fires when an entity spawned in the players View Distance or when a player walks into View Distance
        /// </summary>
        public event BotEntityEvent EntitySpawned {
            add => this.EntityModule!.EntitySpawned += value;
            remove => this.EntityModule!.EntitySpawned -= value;
        }

        /// <summary>
        ///     Fires when an entity despawned in the players View Distance
        /// </summary>
        public event BotEntityEvent EntityDespawned {
            add => this.EntityModule!.EntityDespawned += value;
            remove => this.EntityModule!.EntityDespawned -= value;
        }

        /// <summary>
        ///     Fires when an entity moved
        /// </summary>
        public event BotEntityEvent EntityMoved {
            add => this.EntityModule!.EntityMoved += value;
            remove => this.EntityModule!.EntityMoved -= value;
        }

        /// <summary>
        ///     Fires when an entity's effect is added / removed / updated
        /// </summary>
        public event BotEntityEvent EntityEffectChanged {
            add => this.EntityModule!.EntityEffectChanged += value;
            remove => this.EntityModule!.EntityEffectChanged -= value;
        }
    }
}
