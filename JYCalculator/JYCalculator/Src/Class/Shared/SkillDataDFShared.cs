using Force.DeepCloner;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public partial class SkillDataDF : SkillDataDFBase
    {
        #region 共用区域

        public Dictionary<string, SkillData> Data;

        public SkillDataDF(SkillInfoDB db)
        {
            Data = db.Skills.ToDictionary(_ => _.Key, _ => new SkillData(_.Value));
            Skill2Recipe = Data.ToDictionary(_ => _.Key, _ => new List<Recipe>());
            Skill2RecipeGroup = Data.ToDictionary(_ => _.Key, _ => new SkillRecipeGroup());
        }

        public SkillDataDF() : this(StaticXFData.DB.SkillInfo)
        {
        }

        public SkillDataDF(SkillDataDF old)
        {
            Data = old.Data.ToDictionary(_ => _.Key, _ => _.Value.Copy());
            Skill2Recipe = old.Skill2Recipe.ToDictionary(_ => _.Key, _ => _.Value.ToList());
            Skill2RecipeGroup = old.Skill2RecipeGroup.ToDictionary(_ => _.Key, _ => _.Value.DeepClone());
        }

        public SkillDataDF Copy()
        {
            return new SkillDataDF(this);
        }

        public override void Clear()
        {
            base.Clear();
            foreach (var _ in Data.Values)
            {
                _.Reset();
            }
        }


        public void Update(SkillDataDFViewModel vm)
        {
            Update(vm.Recipes, vm.RecipeGroups.Values, vm.QiXue.Model.SkillModifiers);
        }

        public override void ApplyRecipes()
        {
            foreach (var KVP in Data)
            {
                var key = KVP.Key;
                var skill = KVP.Value;

                List<Recipe> recipes = null;
                SkillRecipeGroup recipeGroup = null;

                if (Skill2Recipe.ContainsKey(key))
                {
                    recipes = Skill2Recipe[key];
                }

                if (Skill2RecipeGroup.ContainsKey(key))
                {
                    recipeGroup = Skill2RecipeGroup[key];
                }

                skill.ApplySkillRecipeGroup(recipeGroup);
                skill.ApplyRecipes(recipes);
                skill.Update();
            }
        }

        /// <summary>
        /// 在现有秘籍基础上追加新秘籍
        /// </summary>
        /// <param name="recipe">目标秘籍</param>
        public void AddRecipeAndApply(Recipe recipe)
        {
            foreach (var skillname in recipe.EffectSkillName)
            {
                if (!Skill2Recipe.ContainsKey(skillname))
                {
                    Skill2Recipe.Add(skillname, new List<Recipe>());
                }

                Skill2Recipe[skillname].Add(recipe);
                Data[skillname].ApplyRecipe(recipe);
                Data[skillname].Update();
            }
        }


        /// <summary>
        /// 在现有秘籍基础上追加新秘籍
        /// </summary>
        /// <param name="recipes">很多秘籍</param>
        public void AddRecipesAndApply(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                AddRecipeAndApply(recipe);
            }
        }

        public override void ApplySkillModifier(SkillModifier modifier)
        {
            // 应用技能修饰效果（非秘籍部分），例如雷甲
            foreach (var name in modifier.SkillNames)
            {
                SkillData s;
                if (Data.TryGetValue(name, out s))
                {
                    s.ApplySkillModifier(modifier);
                    s.Update();
                }
            }
        }
    }

    #endregion
}