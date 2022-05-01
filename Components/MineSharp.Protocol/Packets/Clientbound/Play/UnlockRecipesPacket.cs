using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class UnlockRecipesPacket : Packet {

        public UnlockRecipesAction Action { get; private set; }
        public bool CraftingRecipeBookOpen { get; private set; }
        public bool CraftingRecipeBookFilterActive { get; private set; }
        public bool SmeltingRecipeBookOpen { get; private set; }
        public bool SmeltingRecipeBookFilterActive { get; private set; }
        public bool BlastFurnaceRecipeBookOpen { get; private set; }
        public bool BlastFurnaceRecipeBookFilterActive { get; private set; }
        public bool SmokerRecipeBookOpen { get; private set; }
        public bool SmokerRecipeBookFilterActive { get; private set; }
        public Identifier[] RecipeIDs { get; private set; }
        public Identifier[]? RecipeIDs2 { get; private set; }

        public UnlockRecipesPacket(UnlockRecipesAction action, bool craftingRecipeBookOpen, bool craftingRecipeBookFilterActive, bool smeltingRecipeBookOpen, bool smeltingRecipeBookFilterActive, bool blastFurnaceRecipeBookOpen, bool blastFurnaceRecipeBookFilterActive, bool smokerRecipeBookOpen, bool smokerRecipeBookFilterActive, Identifier[] recipeIDs, Identifier[]? recipeIDs2) {
            Action = action;
            CraftingRecipeBookOpen = craftingRecipeBookOpen;
            CraftingRecipeBookFilterActive = craftingRecipeBookFilterActive;
            SmeltingRecipeBookOpen = smeltingRecipeBookOpen;
            SmeltingRecipeBookFilterActive = smeltingRecipeBookFilterActive;
            BlastFurnaceRecipeBookOpen = blastFurnaceRecipeBookOpen;
            BlastFurnaceRecipeBookFilterActive = blastFurnaceRecipeBookFilterActive;
            SmokerRecipeBookOpen = smokerRecipeBookOpen;
            SmokerRecipeBookFilterActive = smokerRecipeBookFilterActive;
            RecipeIDs = recipeIDs;
            RecipeIDs2 = recipeIDs2;
        }

        public UnlockRecipesPacket() { }


        public override void Read(PacketBuffer buffer) {
            this.Action = (UnlockRecipesAction)buffer.ReadVarInt();
            this.CraftingRecipeBookOpen = buffer.ReadBoolean();
            this.CraftingRecipeBookFilterActive = buffer.ReadBoolean();
            this.SmeltingRecipeBookOpen = buffer.ReadBoolean();
            this.SmeltingRecipeBookFilterActive = buffer.ReadBoolean();
            this.BlastFurnaceRecipeBookOpen = buffer.ReadBoolean();
            this.BlastFurnaceRecipeBookFilterActive = buffer.ReadBoolean();
            this.SmokerRecipeBookOpen = buffer.ReadBoolean();
            this.SmokerRecipeBookFilterActive = buffer.ReadBoolean();
            this.RecipeIDs = buffer.ReadIdentifierArray();
            if (this.Action == UnlockRecipesAction.Init) this.RecipeIDs2 = buffer.ReadIdentifierArray();
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        public enum UnlockRecipesAction {
            Init = 0,
            Add = 1,
            Remove = 2,
        }
    }
}
