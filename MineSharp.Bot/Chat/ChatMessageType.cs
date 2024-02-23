namespace MineSharp.Bot.Chat;

/// <summary>
/// Specifies the type of a message
/// </summary>
public enum ChatMessageType
{
    /// <summary>
    /// Chat message
    /// </summary>
    Chat = 0,

    /// <summary>
    /// System message
    /// </summary>
    SystemMessage = 1,

    /// <summary>
    /// Game info 
    /// </summary>
    GameInfo = 2,

    /// <summary>
    /// Message was sent using /say or /tell
    /// </summary>
    SayCommand = 3,

    /// <summary>
    /// 
    /// </summary>
    Message = 4,

    /// <summary>
    /// Team message
    /// </summary>
    TeamMessage = 5,

    /// <summary>
    /// Not used in minecraft java
    /// </summary>
    Emote = 6,

    /// <summary>
    /// 
    /// </summary>
    Raw = 7,
}
