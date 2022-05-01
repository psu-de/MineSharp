using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Physics {
    public class PlayerControls {

        public bool Forward { get; set; } = false;
        public bool Back { get; set; } = false;
        public bool Left { get; set; } = false;
        public bool Right { get; set; } = false;
        public bool Jump { get; set; } = false;
        public bool Sprint { get; set; } = false;
        public bool Sneak { get; set; } = false;

    }
}
