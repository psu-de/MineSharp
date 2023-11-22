using MineSharp.Bot.Chat;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Entities;
using MineSharp.Core.Common.Items;
using MineSharp.Windows;

namespace MineSharp.Bot;

public static class Events
{
    public delegate void BotEvent(MinecraftBot sender);

    public delegate void BotStringEvent(MinecraftBot sender, string message);

    public delegate void BotChatEvent(MinecraftBot sender, Core.Common.Chat message);

    public delegate void BotChatMessageEvent(MinecraftBot sender, UUID player, string message, ChatMessageType chatPosition);

    public delegate void EntityEvent(MinecraftBot sender, Entity entity);
    
    public delegate void PlayerEvent(MinecraftBot sender, MinecraftPlayer player);

    public delegate void WindowEvent(MinecraftBot sender, Window window);
    
    public delegate void ItemEvent(MinecraftBot sender, Item? item);
}
