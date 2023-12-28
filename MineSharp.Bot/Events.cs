using MineSharp.Bot.Chat;
using MineSharp.Chat;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Windows;

namespace MineSharp.Bot;

/// <summary>
/// Event delegates used by MineSharp.Bot
/// </summary>
public static class Events
{
    #pragma warning disable CS1591
    
    public delegate void BotEvent(MineSharpBot sender);
    
    public delegate void BotStringEvent(MineSharpBot sender, string message);

    public delegate void BotChatEvent(MineSharpBot sender, ChatComponent message);

    public delegate void BotChatMessageEvent(MineSharpBot sender, UUID? player, ChatComponent message, ChatMessageType chatPosition, string? senderName);

    public delegate void EntityEvent(MineSharpBot sender, Entity entity);
    
    public delegate void PlayerEvent(MineSharpBot sender, MinecraftPlayer player);

    public delegate void WindowEvent(MineSharpBot sender, Window window);
    
    public delegate void ItemEvent(MineSharpBot sender, Item? item);
    
    #pragma warning restore CS1591
}
