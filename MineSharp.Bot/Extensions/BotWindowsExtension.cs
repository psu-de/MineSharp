using MineSharp.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Bot {
    public partial class MinecraftBot {

        public Window? Inventory => WindowsModule.Inventory;
        public Dictionary<int, Window> OpenedWindows => WindowsModule.OpenedWindows;

    }
}
