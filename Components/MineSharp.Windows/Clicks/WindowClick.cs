using MineSharp.Core.Common;

namespace MineSharp.Windows.Clicks;

/// <summary>
///     Base class for implementing Window clicks
/// </summary>
public abstract class WindowClick
{
    /// <summary>
    ///     Slot index when clicking outside of the window
    /// </summary>
    public const int OutsideClick = -999;

    /// <summary>
    ///     Create a new instance
    /// </summary>
    /// <param name="window"></param>
    /// <param name="slot"></param>
    /// <param name="button"></param>
    protected WindowClick(Window window, short slot, byte button)
    {
        Window = window;
        Slot = slot;
        Button = button;
    }

    /// <summary>
    ///     The clicked window
    /// </summary>
    public Window Window { get; }

    /// <summary>
    ///     Number identifying the clicked button
    /// </summary>
    public byte Button { get; }

    /// <summary>
    ///     Slot index of clicked slot
    /// </summary>
    public short Slot { get; }

    /// <summary>
    ///     The click mode
    /// </summary>
    public abstract ClickMode ClickMode { get; }

    /// <summary>
    ///     The slots affected by the click
    /// </summary>
    /// <returns></returns>
    public abstract Slot[] GetChangedSlots();

    /// <summary>
    ///     Performs the click on <see cref="Window" />
    /// </summary>
    public abstract void PerformClick();
}
