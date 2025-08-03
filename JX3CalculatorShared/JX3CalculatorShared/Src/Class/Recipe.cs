using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Class
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

        public readonly string SkillNameTag;

        public HashSet<string> EffectSkillName; // 可以作用的技能key

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
            SkillNameTag = item.SkillNameTag;


            if (item.RawSkillNames != "")
            {
                EffectSkillName = StringTool.ParseStringList(item.RawSkillNames).ToHashSet();
            }
            else
            {
                EffectSkillName = (new HashSet<string>()).ToHashSet();
            }

        }

        /// <summary>
        /// 按照倍数修改强度，并且生成新的虚拟秘籍
        /// </summary>
        /// <param name="old">旧的秘籍</param>
        /// <param name="k">效果倍数</param>
        public Recipe(Recipe old, double k = 1)
        {
            Name = old.Name;
            string descName = old.DescName;
            DescName = old.DescName;
            RecipeID = old.RecipeID;
            RecipeLevel = old.RecipeLevel;

            SkillRecipeType = old.SkillRecipeType;
            SkillID = old.SkillID;
            SkillKey = old.SkillKey;

            RecipeName = old.RecipeName;

            SkillAttrs = old.SkillAttrs.Copy();
            SSkillAttrs = old.SSkillAttrs.Copy();

            IconID = old.IconID;
            IsExclude = old.IsExclude;

            ToolTip = old.ToolTip;
            Type = old.Type;
            SkillNameTag = old.SkillNameTag;

            EffectSkillName = old.EffectSkillName;


            if (k != 1)
            {
                descName += $"[x{k:F2}]";
                MultiplyEffect(k);
            }

            DescName = descName;

        }

        /// <summary>
        /// 设定此秘籍可以修饰的技能名
        /// </summary>
        /// <param name="skillNames">技能名列表</param>
        public void SetEffectSkills(IEnumerable<string> skillNames)
        {
            EffectSkillName = skillNames.ToHashSet();
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


        /// <summary>
        /// 就地按照倍数修改强度，用于模拟多层效果叠加
        /// </summary>
        /// <param name="k">效果倍数</param>
        public void MultiplyEffect(double k)
        {
            SkillAttrs.MultiplyEffect(k);
            SSkillAttrs.MultiplyEffect(k);
        }

        /// <summary>
        /// 按照倍数修改强度，并且生成新的虚拟秘籍
        /// </summary>
        /// <param name="k">效果倍数</param>

        public Recipe Emit(double k)
        {
            var res = new Recipe(this, k);
            return res;
        }

        #endregion
    }
}