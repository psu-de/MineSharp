using MineSharp.Core.Common;
using MineSharp.Core.Common.Items;
using MineSharp.Core.Serialization;
using MineSharp.Data;
using MineSharp.Data.Protocol;
using MineSharp.Protocol.Packets.NetworkTypes;
using static MineSharp.Protocol.Packets.Clientbound.Play.MerchantOffersPacket;

namespace MineSharp.Protocol.Packets.Clientbound.Play;

/// <summary>
///     The list of trades a villager NPC is offering.
/// </summary>
/// <param name="WindowId">The ID of the window that is open; this is an int rather than a byte.</param>
/// <param name="Size">The number of trades in the following array.</param>
/// <param name="Trades">The trades offered by the villager.</param>
/// <param name="VillagerLevel">The level of the villager.</param>
/// <param name="Experience">Total experience for this villager (always 0 for the wandering trader).</param>
/// <param name="IsRegularVillager">True if this is a regular villager; false for the wandering trader.</param>
/// <param name="CanRestock">True for regular villagers and false for the wandering trader.</param>
public sealed record MerchantOffersPacket(int WindowId, int Size, Trade[] Trades, int VillagerLevel, int Experience, bool IsRegularVillager, bool CanRestock) : IPacket
{
    /// <inheritdoc />
    public PacketType Type => StaticType;
    /// <inheritdoc />
    public static PacketType StaticType => PacketType.CB_Play_TradeList;

    /// <inheritdoc />
    public void Write(PacketBuffer buffer, MinecraftData version)
    {
        buffer.WriteVarInt(WindowId);
        buffer.WriteVarInt(Size);
        foreach (var trade in Trades)
        {
            trade.Write(buffer, version);
        }
        buffer.WriteVarInt(VillagerLevel);
        buffer.WriteVarInt(Experience);
        buffer.WriteBool(IsRegularVillager);
        buffer.WriteBool(CanRestock);
    }

    /// <inheritdoc />
    public static IPacket Read(PacketBuffer buffer, MinecraftData version)
    {
        var windowId = buffer.ReadVarInt();
        var size = buffer.ReadVarInt();
        var trades = new Trade[size];
        for (int i = 0; i < size; i++)
        {
            trades[i] = Trade.Read(buffer, version);
        }
        var villagerLevel = buffer.ReadVarInt();
        var experience = buffer.ReadVarInt();
        var isRegularVillager = buffer.ReadBool();
        var canRestock = buffer.ReadBool();

        return new MerchantOffersPacket(windowId, size, trades, villagerLevel, experience, isRegularVillager, canRestock);
    }

    /// <summary>
    ///     Represents a trade offered by a villager.
    /// </summary>
    /// <param name="InputItem1">The first item the player has to supply for this villager trade.</param>
    /// <param name="OutputItem">The item the player will receive from this villager trade.</param>
    /// <param name="InputItem2">The second item the player has to supply for this villager trade. May be an empty slot.</param>
    /// <param name="TradeDisabled">True if the trade is disabled; false if the trade is enabled.</param>
    /// <param name="NumberOfTradeUses">Number of times the trade has been used so far.</param>
    /// <param name="MaximumNumberOfTradeUses">Number of times this trade can be used before it's exhausted.</param>
    /// <param name="XP">Amount of XP the villager will earn each time the trade is used.</param>
    /// <param name="SpecialPrice">The number added to the price when an item is discounted due to player reputation or other effects.</param>
    /// <param name="PriceMultiplier">Determines how much demand, player reputation, and temporary effects will adjust the price.</param>
    /// <param name="Demand">If positive, causes the price to increase. Negative values seem to be treated the same as zero.</param>
    public sealed record Trade(Item? InputItem1, Item? OutputItem, Item? InputItem2, bool TradeDisabled, int NumberOfTradeUses, int MaximumNumberOfTradeUses, int XP, int SpecialPrice, float PriceMultiplier, int Demand) : ISerializableWithMinecraftData<Trade>
    {
        /// <inheritdoc />
        public void Write(PacketBuffer buffer, MinecraftData data)
        {
            buffer.WriteOptionalItem(InputItem1);
            buffer.WriteOptionalItem(OutputItem);
            buffer.WriteOptionalItem(InputItem2);
            buffer.WriteBool(TradeDisabled);
            buffer.WriteInt(NumberOfTradeUses);
            buffer.WriteInt(MaximumNumberOfTradeUses);
            buffer.WriteInt(XP);
            buffer.WriteInt(SpecialPrice);
            buffer.WriteFloat(PriceMultiplier);
            buffer.WriteInt(Demand);
        }

        /// <inheritdoc />
        public static Trade Read(PacketBuffer buffer, MinecraftData data)
        {
            var inputItem1 = buffer.ReadOptionalItem(data);
            var outputItem = buffer.ReadOptionalItem(data);
            var inputItem2 = buffer.ReadOptionalItem(data);
            var tradeDisabled = buffer.ReadBool();
            var numberOfTradeUses = buffer.ReadInt();
            var maximumNumberOfTradeUses = buffer.ReadInt();
            var xp = buffer.ReadInt();
            var specialPrice = buffer.ReadInt();
            var priceMultiplier = buffer.ReadFloat();
            var demand = buffer.ReadInt();

            return new Trade(inputItem1, outputItem, inputItem2, tradeDisabled, numberOfTradeUses, maximumNumberOfTradeUses, xp, specialPrice, priceMultiplier, demand);
        }
    }
}
