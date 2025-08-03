using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using System.Collections.Generic;

namespace JYCalculator.ViewModels
{

    /// <summary>
    /// 技能信息集合
    /// </summary>
    public class SkillDataDFViewModel : AbsDependViewModel<AbsViewModel>
    {
        #region 成员

        public readonly AllSkillMiJiConfigViewModel SkillMiJi;
        public readonly QiXueConfigViewModel QiXue;

        public readonly List<Recipe> Recipes;
        public readonly Dictionary<string, SkillRecipeGroup> RecipeGroups;

        public readonly SkillDataDF Model;

        public IEnumerable<SkillData> Data => Model.Data.Values; // 存储数据

        #endregion

        #region 构建

        public SkillDataDFViewModel(AllSkillMiJiConfigViewModel skill, QiXueConfigViewModel qixue) :
            base(InputPropertyNameType.None, skill, qixue)
        {
            SkillMiJi = skill;
            QiXue = qixue;

            Recipes = new List<Recipe>();
            RecipeGroups = skill.SkillRecipeGroups;

            Model = new SkillDataDF();
            PostConstructor();
        }

        #endregion

        protected override void _Update()
        {
            RefreshRecipes();
            ApplyRecipes();
        }

        protected void RefreshRecipes()
        {
            Recipes.Clear();
            Recipes.AddRange(QiXue.Model.OtherRecipes);
            Recipes.AddRange(SkillMiJi.OtherRecipes);
        }

        protected void ApplyRecipes()
        {
            Model.Update(this);
        }

        protected override void _RefreshCommands()
        {
        }
    }
}