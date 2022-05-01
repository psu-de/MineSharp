using MineSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.Protocol.Packets.Clientbound.Play {
    public class DeclareRecipesPacket : Packet {

        public Recipe[] Recipes { get; private set; }

        public DeclareRecipesPacket() { }
        public DeclareRecipesPacket(Recipe[] recipes) { this.Recipes = recipes; }

        public override void Read(PacketBuffer buffer) {
            int length = buffer.ReadVarInt();
            this.Recipes = new Recipe[length];
            for (int i = 0; i < length; i++) this.Recipes[i] = ReadRecipe(buffer);
        }

        public override void Write(PacketBuffer buffer) {
            throw new NotImplementedException();
        }

        private Recipe ReadRecipe(PacketBuffer buffer) {
            Identifier type = buffer.ReadIdentifier();
            Identifier recipeId = buffer.ReadIdentifier();
            object? data = null;
            Slot[][]? ingredients = null;
            Slot? result = null;
            Slot[]? ingredient = null;
            string? group = null;
            switch (type.Value) {
                case "crafting_shapeless":
                    group = buffer.ReadString();
                    ingredients = new Slot[buffer.ReadVarInt()][];
                    for (int i = 0; i < ingredients.Length; i++) ingredients[i] = buffer.ReadSlotArray();
                    result = buffer.ReadSlot();
                    data = new { Group = group, Ingredients = ingredients, Result = result };
                    break;
                case "crafting_shaped":
                    int width = buffer.ReadVarInt();
                    int height = buffer.ReadVarInt();
                    group = buffer.ReadString();
                    ingredients = new Slot[width * height][];
                    for (int i = 0; i < ingredients.Length; i++) ingredients[i] = buffer.ReadSlotArray();
                    result = buffer.ReadSlot();
                    data = new { Width = width, Height = height, Group = group, Ingredients = ingredients, Result = result };
                    break;
                case "smelting":
                case "blasting":
                case "smoking":
                case "campfire_cooking":
                    group  = buffer.ReadString();
                    ingredient = buffer.ReadSlotArray();
                    result = buffer.ReadSlot();
                    float experience = buffer.ReadFloat();
                    int cookingTime = buffer.ReadVarInt();
                    data = new { Group = group, Ingredient = ingredient, Result = result, Experience = experience, CookingTime = cookingTime };
                    break;
                case "stonecutting":
                    group = buffer.ReadString();
                    ingredient = buffer.ReadSlotArray();
                    result = buffer.ReadSlot();
                    data = new { Group = group, Ingredient = ingredient, Result = result };
                    break;
                case "smithing":
                    Slot[] @base = buffer.ReadSlotArray();
                    Slot[] addition = buffer.ReadSlotArray();
                    result = buffer.ReadSlot();
                    data = new { Base = @base, Addition = addition, Result = result };
                    break;
            }

            return new Recipe(type, recipeId, data);
        }
    }
}
