namespace MineSharp.Windows.Clicks;

public abstract class WindowClick
{
    public const int OutsideClick = -999;
        
    public Window Window { get; }
    public byte Button { get; }
    public short Slot { get; }

    public abstract ClickMode ClickMode { get; }

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
