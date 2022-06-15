using MineSharp.Core.Types;
using MineSharp.Data.T4.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Data.T4.Blocks {
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

	}
}
