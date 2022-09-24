using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Core.Logging
{
    internal struct LogScope
    {
        public LogLevel Threshold { get; set; }
        public Action<string> WriteLine { get; set; }

        public LogScope(LogLevel threshold, Action<string> writeLine)
        {
            Threshold = threshold;
            WriteLine = writeLine;
        }
    }
}
