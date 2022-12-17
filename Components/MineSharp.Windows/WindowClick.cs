using MineSharp.Core.Types.Enums;

namespace MineSharp.Windows
{
    /// <summary>
    /// Represents a operation on a <see cref="Window"/>
    /// To perform the action call window.Click(windowClick)
    /// </summary>
    public abstract class WindowClick
    {
        public const int OutsideClick = -999;
        
        public Window Window { get; }
        public byte Button { get; }
        public short Slot { get; }

        public abstract WindowOperationMode ClickMode { get; }

        protected WindowClick(Window window, short slot, byte button)
        {
            this.Window = window;
            this.Slot = slot;
            this.Button = button;
        }
        
        /// <summary>
        /// Performs the click on <see cref="Window"/>
        /// </summary>
        public abstract void PerformClick();
    }
}
