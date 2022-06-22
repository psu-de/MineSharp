namespace MineSharp.Core.Types {
    public class Recipe {

        public Identifier Type { get; private set; }
        public Identifier RecipeId { get; private set; }
        public object? Data { get; private set; }

        public Recipe(Identifier type, Identifier recipeid, object? data) {
            Type = type;
            RecipeId = recipeid;
            Data = data;
        }
    }
}
