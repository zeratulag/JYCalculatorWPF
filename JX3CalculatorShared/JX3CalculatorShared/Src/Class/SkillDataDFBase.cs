using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class SkillDataDFBase : ObservableObject
    {
        public Dictionary<string, List<Recipe>> Skill2Recipe; // 每个技能应用的秘籍;
        public Dictionary<string, SkillRecipeGroup> Skill2RecipeGroup; // 每个技能应用的秘籍组;

        /// <summary>
        /// 清除已经应用的所有秘籍效果，避免重复叠加秘籍
        /// </summary>
        public virtual void Clear()
        {
            Skill2Recipe?.Clear();
            Skill2RecipeGroup?.Clear();
        }

        public void Update(IEnumerable<Recipe> recipes, IEnumerable<SkillRecipeGroup> recipeGroups,
            IEnumerable<SkillModifier> modifiers = null)
        {
            Clear();
            GetSkill2Recipe(recipes);
            GetRecipeGroup(recipeGroups);
            ApplyRecipes();
            ApplySkillModifiers(modifiers);
            PostProceed();
        }


        /// <summary>
        /// 寻找每个技能分别被应用了哪些秘籍
        /// </summary>
        /// <param name="recipes"></param>
        public void GetSkill2Recipe(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                foreach (var skillName in recipe.EffectSkillName)
                {
                    if (!Skill2Recipe.ContainsKey(skillName))
                    {
                        Skill2Recipe.Add(skillName, new List<Recipe>());
                    }

                    Skill2Recipe[skillName].Add(recipe);
                }
            }
        }

        public void GetRecipeGroup(IEnumerable<SkillRecipeGroup> recipeGroups)
        {
            foreach (var recipeGroup in recipeGroups)
            {
                if (recipeGroup.Recipes != null)
                {
                    foreach (var skillName in recipeGroup.EffectSkillName)
                    {
                        Skill2RecipeGroup.Add(skillName, recipeGroup);
                    }
                }
            }
        }

        public virtual void ApplyRecipes()
        {
        }

        public virtual void ApplySkillModifier(SkillModifier modifier)
        {
        }

        public void ApplySkillModifiers(IEnumerable<SkillModifier> modifiers)
        {
            // 应用技能修饰效果（非秘籍部分），例如雷甲
            if (modifiers == null) return;
            foreach (var mod in modifiers)
            {
                ApplySkillModifier(mod);
            }
        }

        // 后处理（可选）
        public virtual void PostProceed()
        {
        }
    }
}