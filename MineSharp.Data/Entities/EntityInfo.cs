using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Entities {
    public struct EntityInfo {

        public EntityType Type { get; set; }
        public EntityCategory Category { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

    }
}
