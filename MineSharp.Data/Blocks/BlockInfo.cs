using MineSharp.Core.Logging;
using MineSharp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.Blocks {
    public class BlockInfo {

        public BlockType Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public float? Hardness { get; set; }
        public float Resistance { get; set; }
        public bool Diggable { get; set; }
        public bool Transparent { get; set; }
        public int FilterLight { get; set; }
        public int EmitLight { get; set; }
        public string BoundingBox { get; set; }
        public int StackSize { get; set; }
        public string Material { get; set; }
        public int DefaultState { get; set; }
        public int MinStateId { get; set; }
        public int MaxStateId { get; set; }
        public Items.ItemType[]? HarvestTools { get; set; }

        public BlockInfo(BlockType id, string displayName, string name, float? hardness, float resistance, bool diggable, bool transparent, int filterLight, int emitLight, string boundingBox, int stackSize, string material, int defaultState, int minStateId, int maxStateId, Items.ItemType[]? harvestTools) {
            Id = id;
            DisplayName = displayName;
            Name = name;
            Hardness = hardness;
            Resistance = resistance;
            Diggable = diggable;
            Transparent = transparent;
            FilterLight = filterLight;
            EmitLight = emitLight;
            BoundingBox = boundingBox;
            StackSize = stackSize;
            Material = material;
            DefaultState = defaultState;
            MinStateId = minStateId;
            MaxStateId = maxStateId;
            HarvestTools = harvestTools;
        }

        public bool CanBeHarvested(Items.ItemInfo? info) {
            if (this.HarvestTools == null) return true;

            if (info == null) return false;

            return this.HarvestTools.Contains(info.Id);
        }

        public int CalculateBreakingTime(Items.ItemInfo? info, Player miner) {

            if (this.Hardness == null) throw new InvalidDataException("Hardness is null");

            if (miner.GameMode == Core.Types.Enums.GameMode.Creative) return 0;

            float toolMultiplier = info?.GetToolMultiplier(this) ?? 1;
            float efficiencyLevel = 0; // TODO: Efficiency level
            float hasteLevel = miner.GetEffectLevel(Effects.EffectType.Haste) ?? 0;
            float miningFatiqueLevel = miner.GetEffectLevel(Effects.EffectType.MiningFatigue) ?? 0;

            toolMultiplier /= MathF.Pow(1.3f, efficiencyLevel);
            toolMultiplier /= MathF.Pow(1.2f, hasteLevel);
            toolMultiplier *= MathF.Pow(0.3f, miningFatiqueLevel);

            float damage = toolMultiplier / (float)this.Hardness;

            bool canHarvest = this.CanBeHarvested(info);
            if (canHarvest) {
                damage /= 30f;
            } else {
                damage /= 100f;
            }

            if (damage > 1) return 0;

            float ticks = MathF.Ceiling(1 / damage);
            return (int)((ticks / 20) * 1000);
        }
    }
}
