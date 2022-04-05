using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Versions {
    public class ProtocolVersion {

        public static int GetVersionNumber(string version) {

            switch (version) {
                case "1.18.1": return 757;
                default: throw new NotSupportedException();
            }
        }

    }
}
