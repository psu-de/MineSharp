using MineSharp.Core.Types;
using MineSharp.Data.Effects;

namespace MineSharp.Data.Blocks {
    public static class BlockExtensions {

		public static bool IsSolid(this Block block) {
			var id = block.Id;
			return id == Air.BlockId || id == CaveAir.BlockId || id == VoidAir.BlockId;
		}

		public static int CalculateBreakingTime(this Block block, Item? heldItem, Entity miner) {

			if (block.Hardness == null) throw new InvalidDataException("Hardness is null");

			//TODO: Gamemode creative
			//if (miner.GameMode == Core.Types.Enums.GameMode.Creative) return 0;

			float toolMultiplier = heldItem?.GetToolMultiplier(block) ?? 1;
			float efficiencyLevel = 0; // TODO: Efficiency level
			float hasteLevel = miner.GetEffectLevel(HasteEffect.EffectId) ?? 0;
			float miningFatiqueLevel = miner.GetEffectLevel(MiningfatigueEffect.EffectId) ?? 0;

			toolMultiplier /= MathF.Pow(1.3f, efficiencyLevel);
			toolMultiplier /= MathF.Pow(1.2f, hasteLevel);
			toolMultiplier *= MathF.Pow(0.3f, miningFatiqueLevel);

			float damage = toolMultiplier / (float)block.Hardness;

			bool canHarvest = block.CanBeHarvested(heldItem);
			if (canHarvest) {
				damage /= 30f;
			} else {
				damage /= 100f;
			}

			if (damage > 1) return 0;

			float ticks = MathF.Ceiling(1 / damage);
			return (int)((ticks / 20) * 1000);
		}

		public static bool CanBeHarvested(this Block block, Item? item) {
			if (block.HarvestTools == null) return true;

			if (item == null) return false;

			return block.HarvestTools.Contains(item!.Id);
		}


		public static BlockShape[] GetBlockShape(this Block block) {

			var blockType = block.GetType();
			var shapeIndices = (int[])blockType.GetProperty("", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!;
			float[][] shapeData = BlockShapePalette.AllBlockShapes[shapeIndices[0]];

			return shapeData.Select(x => new BlockShape(x)).ToArray();
		}
	}
}
