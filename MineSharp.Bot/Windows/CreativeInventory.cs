using MineSharp.Bot.Plugins;
using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Data;
using MineSharp.Protocol.Packets.Serverbound.Play;

namespace MineSharp.Bot.Windows;

/// <summary>
/// A creative inventory
/// </summary>
public class CreativeInventory
{
    private MineSharpBot    bot;
    private PlayerPlugin player;
    private MinecraftData   data;

    /// <summary>
    /// Indicates whether the creative inventory is available
    /// </summary>
    public bool Available => player.Self?.GameMode == GameMode.Creative;
    
    internal CreativeInventory(MineSharpBot bot)
    {
        this.bot    = bot;
        this.player = bot.GetPlugin<PlayerPlugin>();
        this.data   = bot.Data;
    }

    /// <summary>
    /// Get an item from the creative inventory and put it in the bots inventory at <paramref name="inventorySlotIndex"/>
    /// </summary>
    public Task GetItem(ItemType type, byte count, short inventorySlotIndex)
    {
        if (!this.Available)
        {
            throw new InvalidOperationException("creative inventory is only available in creative mode");
        }
        
        var info = this.data.Items.ByType(type);

        if (info is null)
        {
            throw new NullReferenceException($"item type {type} not found (this item probably doesn't exist in {this.data.Version})");
        }

        return bot.Client.SendPacket(new SetCreativeSlotPacket(
            inventorySlotIndex,
            new Item(info, count, null, null)));
    }
}
