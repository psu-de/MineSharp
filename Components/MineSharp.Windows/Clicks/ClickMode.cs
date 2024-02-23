namespace MineSharp.Windows.Clicks;

/// <summary>
/// Specifies the type of a click
/// </summary>
public enum ClickMode
{
    /// <summary>
    /// Simple mouse click
    /// </summary>
    SimpleClick = 0,

    /// <summary>
    /// Mouse click while holding shift
    /// </summary>
    ShiftClick = 1,

    /// <summary>
    /// A hotkey click
    /// </summary>
    Hotkey = 2,

    /// <summary>
    /// Mouse middle click
    /// </summary>
    MouseMiddle = 3,

    /// <summary>
    /// Drop key
    /// </summary>
    DropKey = 4,

    /// <summary>
    /// Mouse drag while holding a mouse key
    /// </summary>
    MouseDrag = 5,

    /// <summary>
    /// Mouse double click
    /// </summary>
    DoubleClick = 6
}
