using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace JX3CalculatorShared.Src.Class
{
    /// <summary>
    /// 用于描述游戏内修饰技能的秘籍（包括秘籍，奇穴，装备效果）的类
    /// </summary>
    public class Recipe : IComparable<Recipe>
    {
        #region 成员

        public readonly string Name;

        public readonly string DescName;

        public readonly int RecipeID;

        public readonly int RecipeLevel;

        public readonly int SkillRecipeType;

        public readonly int SkillID;

        public readonly string SkillKey;

        public readonly string RecipeName;

        public readonly int IconID;

        public readonly string ToolTip;

        public readonly bool IsExclude = false;

        public readonly RecipeTypeEnum Type;

        public readonly SkillAttrCollection SkillAttrs;
        public readonly SkillAttrCollection SSkillAttrs;

        public ImmutableHashSet<string> EffectSkillName; // 可以作用的技能key

        #endregion

        #region 构造

        public Recipe(RecipeItem item)
        {
            Name = item.Name;
            DescName = item.DescName;
            RecipeID = item.RecipeID;
            RecipeLevel = item.RecipeLevel;

            SkillRecipeType = item.SkillRecipeType;
            SkillID = item.SkillID;
            SkillKey = item.Skill_key;

            RecipeName = item.RecipeName;

            var attrs = item.ParseItem();
            SkillAttrs = new SkillAttrCollection(attrs);
            SSkillAttrs = SkillAttrs.Simplify();

            IconID = item.IconID;
            IsExclude = item.IsExclude;

            ToolTip = item.ToolTip + item.GetToolTipTail();

            Type = item.Type;


            if (item.RawSkillNames != "")
            {
                EffectSkillName = StringTool.ParseStringList(item.RawSkillNames).ToImmutableHashSet();
            }
            else
            {
                EffectSkillName = (new HashSet<string>()).ToImmutableHashSet();
            }
            
        }


        /// <summary>
        /// 设定此秘籍可以修饰的技能名
        /// </summary>
        /// <param name="skillNames">技能名列表</param>
        public void SetEffectSkills(IEnumerable<string> skillNames)
        {
            EffectSkillName = skillNames.ToImmutableHashSet();
        }

        #endregion

        #region 显示

        #endregion

        #region 简化

        #endregion

        #region 方法

        public int CompareTo(Recipe other)
        {
            return String.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }

        #endregion
    }
}