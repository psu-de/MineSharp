namespace MineSharp.Core.Types {
    public class VillagerTrade {

        public Slot InputItem { get; set; }
        public Slot OutputItem { get; set; }
        public bool HasSecondItem { get; set; }
        public Slot? InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int TradeUses { get; set; }
        public int MaxUses { get; set; }
        public int XP { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }

        public VillagerTrade(Slot inputItem, Slot outputItem, bool hasSecondItem, Slot? inputItem2, bool tradeDisabled, int tradeUses, int maxUses, int xP, int specialPrice, float priceMultiplier, int demand) {
            InputItem = inputItem;
            OutputItem = outputItem;
            HasSecondItem = hasSecondItem;
            InputItem2 = inputItem2;
            TradeDisabled = tradeDisabled;
            TradeUses = tradeUses;
            MaxUses = maxUses;
            XP = xP;
            SpecialPrice = specialPrice;
            PriceMultiplier = priceMultiplier;
            Demand = demand;
        }
    }
}
