using Newtonsoft.Json;

namespace MineSharp.Data.Generator.Recipes
{
    internal class RecipeJsonInfo
    {
        [JsonProperty("ingredients")]
        public int[]? Ingredients;

        [JsonProperty("inShape")]
        public int?[][]? InShape;

        [JsonProperty("OutShape")]
        public int?[][]? OutShape;

        [JsonProperty("result")]
        public RecipeResult Result;
        
        public struct RecipeResult
        {
            [JsonProperty("count")]
            public int Count;
            [JsonProperty("id")]
            public int Id;
        }
    }
}
