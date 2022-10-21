namespace MineSharp.Data.Generator.Recipes
{
    internal class RecipeGenerator : Generator
    {
        public RecipeGenerator(MinecraftDataHelper wrapper, string version) : base(wrapper, version) {}
        public override void WriteCode(CodeGenerator codeGenerator)
        {
            var recipeData = this.Wrapper.LoadJson<Dictionary<int, RecipeJsonInfo[]>>(Version, "recipes");
            
            codeGenerator.CommentBlock($"Generated Recipe Data for Minecraft Version {this.Version}");
            
            codeGenerator.WriteLine("using MineSharp.Core.Types;");
            
            codeGenerator.Begin("namespace MineSharp.Data.Recipes");
            codeGenerator.Begin("public static class RecipePalette");
            codeGenerator.Begin("public static readonly Dictionary<int, Recipe[]> Recipes = new Dictionary<int, Recipe[]>()");

            foreach (var kvp in recipeData)
            {
                codeGenerator.WriteLine($"{{ {kvp.Key}, new [] {{ \n{codeGenerator.CurrentIndent + CodeGenerator.Indent}{string.Join($", \n{codeGenerator.CurrentIndent + CodeGenerator.Indent}", kvp.Value.Select(StringifyRecipe))} }} }},");
            }
            
            codeGenerator.Finish(semicolon: true);
            codeGenerator.Finish();
            codeGenerator.Finish();
        }
        
        private string StringifyRecipe(RecipeJsonInfo recipe)
        {
            int?[] ingredients;
            var requiresTable = false;
            
            if (recipe.Ingredients != null)
            {
                ingredients = recipe.Ingredients
                    .Select(x => (int?)x)    
                    .ToArray();
                requiresTable = recipe.Ingredients.Length > 4;
            } 
            else
            {
                if (recipe.InShape == null)
                {
                    throw new Exception("Ingredients and inShape are null");
                }
                ingredients = ConvertShape(recipe.InShape!);
                ingredients = RemoveTrailingNulls(ingredients);
                requiresTable = recipe.InShape!.Length > 2 || recipe.InShape!.Any(x => x.Length > 2);
                if (!requiresTable)
                {
                    if (ingredients.Length > 3)
                    {
                        if (ingredients[2] != null)
                        {
                            throw new Exception($"[{string.Join(", ", ingredients)}], [{string.Join(", ", ConvertShape(recipe.InShape!))}], {recipe.InShape.Length}, [{string.Join(", ", recipe.InShape.Select(x => x.Length))}]");
                        }
                    
                        var ing = ingredients.ToList();
                        ing.RemoveAt(2);
                        ingredients = ing.ToArray();   
                    }
                    
                    if (ingredients.Length > 4)
                    {
                        throw new Exception($"[{string.Join(", ", ingredients)}], [{string.Join(", ", ConvertShape(recipe.InShape!))}], {recipe.InShape.Length}, [{string.Join(", ", recipe.InShape.Select(x => x.Length))}]");
                    }   
                }
            }

            int?[]? outShape = null;
            
            if (recipe.OutShape != null)
            {
                outShape = ConvertShape(recipe.OutShape!);
            }

            return $"new Recipe({StringifyArray(ingredients)}, {StringifyArray(outShape)}, {requiresTable.ToString().ToLower()}, {recipe.Result.Id}, {recipe.Result.Count})";
        }

        private string StringifyArray(int?[]? arr)
        {
            if (arr == null)
            {
                return "null";
            }
            
            return $"new int?[] {{ {string.Join(", ", arr.Select(x => x == null ? "null" : x.ToString()))} }}";
        }

        private int?[] ConvertShape(int?[][] shape)
        {
            var @out = new int?[9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var idx = (i * 3) + j;
                    int? id = null;
                    if (shape.Length > i && shape[i].Length > j)
                    {
                        id = shape[i][j];
                    }
                    @out[idx] = id;
                }
            }
            return @out;
        }
        
        private int?[] RemoveTrailingNulls(int?[] arr)
        {
            int nullIndex = -1;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != null)
                {
                    nullIndex = -1;
                    continue;
                }

                if (nullIndex > 0)
                {
                    continue;
                }

                nullIndex = i;
            }

            if (nullIndex == -1)
            {
                return arr;
            }

            return arr.Take(nullIndex).ToArray();
        }
    }
}
