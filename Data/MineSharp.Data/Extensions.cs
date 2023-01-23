using MineSharp.Core.Types;
using MineSharp.Data.Blocks;
using MineSharp.Data.Effects;
using MineSharp.Data.Items;
using MineSharp.Data.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot = MineSharp.Data.Protocol.Slot;

namespace MineSharp.Data
{
    public static class Extensions
    {
        public static bool IsSolid(this Block block)
        {
            var id = block.Info.Id;
            return !(id == BlockPalette.AirInfo.Id || id == BlockPalette.CaveAirInfo.Id || id == BlockPalette.VoidAirInfo.Id);
        }

        public static int CalculateBrakingTime(this Block block, Item? heldItem, Entity miner) => block.Info.CalculateBreakingTime(heldItem, miner);

        public static int CalculateBreakingTime(this BlockInfo block, Item? heldItem, Entity miner)
        {

            //TODO: Gamemode creative
            //if (miner.GameMode == Core.Types.Enums.GameMode.Creative) return 0;

            float toolMultiplier = heldItem?.GetToolMultiplier(block) ?? 1;
            float efficiencyLevel = 0; // TODO: Efficiency level
            float hasteLevel = miner.GetEffectLevel(EffectPalette.HasteEffectInfo.Id) ?? 0;
            float miningFatiqueLevel = miner.GetEffectLevel(EffectPalette.MiningfatigueEffectInfo.Id) ?? 0;

            toolMultiplier /= MathF.Pow(1.3f, efficiencyLevel);
            toolMultiplier /= MathF.Pow(1.2f, hasteLevel);
            toolMultiplier *= MathF.Pow(0.3f, miningFatiqueLevel);

            var damage = toolMultiplier / block.Hardness;

            var canHarvest = block.CanBeHarvested(heldItem);
            if (canHarvest)
            {
                damage /= 30f;
            }
            else
            {
                damage /= 100f;
            }

            if (damage > 1) return 0;

            var ticks = MathF.Ceiling(1 / damage);
            return (int)(ticks / 20 * 1000);
        }

        public static bool CanBeHarvested(this Block block, Item? item) => block.Info.CanBeHarvested(item);

        public static bool CanBeHarvested(this BlockInfo block, Item? item)
        {
            if (block.HarvestTools == null) return true;

            if (item == null) return false;

            return block.HarvestTools.Contains(item!.Info.Id);
        }


        public static BlockShape[] GetBlockShape(this Block block)
        {
            var shapeIndices = block.Info.BlockShapeIndices;
            var idx = 0;
            if (shapeIndices.Length > 1)
            {
                idx = block.Metadata;
            }
            var shapeData = BlockShapePalette.AllBlockShapes[shapeIndices[idx]];

            return shapeData.Select(x => new BlockShape(x)).ToArray();
        }

        public static AABB[] GetBoundingBoxes(this Block block)
        {
            var shapeIndices = block.Info.BlockShapeIndices;
            var idx = shapeIndices.Length > 1 ? block.Metadata : 0;
            var shapeData = BlockShapePalette.AllBlockShapes[shapeIndices[idx]];
            return shapeData.Select(x => new BlockShape(x).ToBoundingBox().Offset(block.Position!.X, block.Position.Y, block.Position.Z)).ToArray();
        }




        public static Slot ToProtocolSlot(this Core.Types.Slot slot) => new Slot(!slot.IsEmpty(), new Slot.AnonSwitch(slot.IsEmpty() ? null : new Slot.AnonSwitch.AnonSwitchStatetrueContainer(slot.Item!.Info.Id, (sbyte)slot.Item!.Count, slot.Item!.Metadata)));

        public static Core.Types.Slot ToSlot(this Slot slot)
        {

            if (slot.Present)
            {
                var anon = (Slot.AnonSwitch.AnonSwitchStatetrueContainer)slot.Anon!;
                return new Core.Types.Slot(
                    new Item(ItemPalette.GetItemInfoById(anon.ItemId), (byte)anon.ItemCount, null, anon.NbtData), -1);
            }
            return new Core.Types.Slot(null, -1);

        }

        public static PositionBitfield ToProtocolPosition(this Position pos) => new PositionBitfield(pos.ToULong());


    }
}
