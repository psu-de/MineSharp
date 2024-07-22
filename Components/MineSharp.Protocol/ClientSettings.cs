using MineSharp.Core.Common;
using NLog;

namespace MineSharp.Protocol;

/// <summary>
///     Describes client settings
/// </summary>
public record ClientSettings
{
    /// <summary>
    ///     Default client settings
    /// </summary>
    public static ClientSettings Default { get; } = new(
        "en_GB",
        24,
        ChatMode.Enabled,
        true,
        0x7F,
        PlayerHand.MainHand,
        false,
        true);

    /// <summary>
    ///     Constructor
    /// </summary>
    public ClientSettings(string locale, byte viewDistance, ChatMode chatMode, bool coloredChat,
                          byte displayedSkinParts,
                          PlayerHand mainHand, bool enableTextFiltering, bool allowServerListings)
    {
        Locale = locale;
        ViewDistance = viewDistance;
        ChatMode = chatMode;
        ColoredChat = coloredChat;
        DisplayedSkinParts = displayedSkinParts;
        MainHand = mainHand;
        EnableTextFiltering = enableTextFiltering;
        AllowServerListings = allowServerListings;
    }

    /// <summary>
    ///     Locale (e.g. 'en_GB')
    /// </summary>
    public string Locale { get; }

    /// <summary>
    ///     The client's view distance
    /// </summary>
    public byte ViewDistance { get; }

    /// <summary>
    ///     What the client want's to see in chat (currently ignored)
    /// </summary>
    public ChatMode ChatMode { get; } // TODO: #31

    /// <summary>
    ///     Whether the client allows colored chat (currently ignored)
    /// </summary>
    public bool ColoredChat { get; } // TODO: #31

    /// <summary>
    ///     Bitmask of skin parts displayed by the client (not used)
    /// </summary>
    public byte DisplayedSkinParts { get; }

    /// <summary>
    ///     The clients main hand
    /// </summary>
    public PlayerHand MainHand { get; }

    /// <summary>
    ///     Whether to filter chat messages (currently ignored)
    /// </summary>
    public bool EnableTextFiltering { get; } // TODO: #31

    /// <summary>
    ///     Whether you want to show up in a server's online players list
    /// </summary>
    public bool AllowServerListings { get; }
}

/// <summary>
///     Specifies the chat mode
/// </summary>
public enum ChatMode
{
    /// <summary>
    ///     Show all chat messages
    /// </summary>
    Enabled = 0,

    /// <summary>
    ///     Only command messages
    /// </summary>
    CommandsOnly = 1,

    /// <summary>
    ///     No player messages nor command messages
    /// </summary>
    Hidden = 2
}
