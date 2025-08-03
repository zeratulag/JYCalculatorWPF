using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;

namespace JYCalculator.Src.Class
{
    public class BuffToRecipeItem
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Key { get; set; }
        public int BuffID { get; set; } = -1;
        public int BuffLevel { get; set; } = -1;
        public int RecipeID { get; set; }
        public int RecipeLevel { get; set; }
        public string SkillModifierName { get; set; }
        public string ScriptFile { get; set; }
        public string BuffRawID => $@"{BuffID}_{BuffLevel}";

        public Recipe CRecipe = null;

        public SkillModifier CModifier = null;

    }
}