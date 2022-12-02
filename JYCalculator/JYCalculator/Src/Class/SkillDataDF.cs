using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Force.DeepCloner;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;
using JYCalculator.ViewModels;

namespace JYCalculator.Class
{
    public class SkillDataDF : ObservableObject
    {
        #region 成员

        public Dictionary<string, SkillData> Data;

        public readonly Dictionary<string, List<Recipe>> Skill2Recipe; // 每个技能应用的秘籍;
        public readonly Dictionary<string, SkillRecipeGroup> Skill2RecipeGroup; // 每个技能应用的秘籍组;

        #endregion

        #region 构建
        public SkillDataDF(SkillInfoDB db)
        {
            Data = db.Skills.ToDictionary(_ => _.Key, _ => new SkillData(_.Value));
            Skill2Recipe = Data.ToDictionary(_ => _.Key, _ => new List<Recipe>());
            Skill2RecipeGroup = Data.ToDictionary(_ => _.Key, _ => new SkillRecipeGroup());
        }

        public SkillDataDF() : this(StaticJYData.DB.SkillInfo)
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

        #endregion

        /// <summary>
        /// 清除已经应用的所有秘籍效果，避免重复叠加秘籍
        /// </summary>
        public void Clear()
        {
            foreach (var _ in Data.Values)
            {
                _.Reset();
            }
            Skill2Recipe.Clear();
            Skill2RecipeGroup.Clear();
        }

        public void Update(IEnumerable<Recipe> recipes, IEnumerable<SkillRecipeGroup> recipeGroups, IEnumerable<SkillModifier> modifiers = null)
        {
            Clear();
            GetSkill2Recipe(recipes);
            GetRecipeGroup(recipeGroups);
            ApplyRecipes();
            ApplySkillModifiers(modifiers);
            PostProceed();
        }

        public void Update(SkillDataDFViewModel vm)
        {
            Update(vm.Recipes, vm.RecipeGroups.Values, vm.QiXue.Model.SkillModifiers);
        }

        /// <summary>
        /// 寻找每个技能分别被应用了哪些秘籍
        /// </summary>
        /// <param name="recipes"></param>
        public void GetSkill2Recipe(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                foreach (var skillname in recipe.EffectSkillName)
                {
                    if (!Skill2Recipe.ContainsKey(skillname))
                    {
                        Skill2Recipe.Add(skillname, new List<Recipe>());
                    }

                    Skill2Recipe[skillname].Add(recipe);
                }
            }
        }

        public void GetRecipeGroup(IEnumerable<SkillRecipeGroup> recipeGroups)
        {
            foreach (var recipeGroup in recipeGroups)
            {
                if (recipeGroup.Recipes != null)
                {
                    foreach (var skillname in recipeGroup.EffectSkillName)
                    {
                        Skill2RecipeGroup.Add(skillname, recipeGroup);
                    }
                }
            }
        }

        public void ApplyRecipes()
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
                skill.UpdateCoef();

            }
        }

        /// <summary>
        /// 在现有秘籍基础上追加新秘籍
        /// </summary>
        /// <param name="recipes"></param>
        public void AddRecipesAndApply(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
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
        }


        // 应用技能修饰效果（非秘籍部分），例如雷甲
        public void ApplySkillModifier(SkillModifier modifier)
        {
            foreach (var name in modifier.SkillNames)
            {
                Data[name].ApplySkillModifier(modifier);
                Data[name].UpdateCoef();
            }
        }

        // 应用技能修饰效果（非秘籍部分），例如雷甲
        public void ApplySkillModifiers(IEnumerable<SkillModifier> modifiers)
        {
            if (modifiers == null) return;
            foreach (var mod in modifiers)
            {
                ApplySkillModifier(mod);
            }
        }

        // 后处理
        protected void PostProceed()
        {
            Data["PZ_ZM"].IgnoreB = Data["ZM"].IgnoreB; // 破招也能吃到无视防御
        }

    }
}