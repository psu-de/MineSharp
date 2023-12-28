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
    
    public delegate void BotEvent(MinecraftBot sender);
    
    public delegate void BotStringEvent(MinecraftBot sender, string message);

    public delegate void BotChatEvent(MinecraftBot sender, ChatComponent message);

    public delegate void BotChatMessageEvent(MinecraftBot sender, UUID? player, ChatComponent message, ChatMessageType chatPosition, string? senderName);

    public delegate void EntityEvent(MinecraftBot sender, Entity entity);
    
    public delegate void PlayerEvent(MinecraftBot sender, MinecraftPlayer player);

    public delegate void WindowEvent(MinecraftBot sender, Window window);
    
    public delegate void ItemEvent(MinecraftBot sender, Item? item);
    
    #pragma warning restore CS1591
}
