using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Effects {
    public class EffectInfo {

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public EffectType Id { get; set; }
        public bool IsGood { get; set; }

        public EffectInfo(string name, string displayName, EffectType id, bool isGood) {
            this.Name = name;
            this.DisplayName = displayName;
            this.Id = id;
            this.IsGood = isGood;
        }

    }
}
