using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.Src.Class
{
    /// <summary>
    /// 用于描述一组相同技能修饰对象的组合，要求修饰的技能必须是相同的
    /// </summary>
    public class SkillRecipeGroup
    {
        #region 成员

        public string SkillKey;
        public int SkillRecipeType;
        public int SkillID;

        public Recipe[] Recipes;
        public string[] Names; // 激活的秘籍名

        public ImmutableHashSet<string> EffectSkillName;

        public SkillAttrCollection SkillAttrs;
        public SkillAttrCollection SSkillAttrs;

        #endregion

        #region 构造

        public SkillRecipeGroup()
        {
        }

        public SkillRecipeGroup(IEnumerable<Recipe> items)
        {
            _Update(items);
        }

        public void _Update(IEnumerable<Recipe> items)
        {
            var allIDs = from item in items select item.SkillID;
            var allRecipeTypes = from item in items select item.SkillRecipeType;
            var allIDArr = allIDs.ToArray();

            if (allIDArr.GetUniqueNum() > 1 || allRecipeTypes.GetUniqueNum() > 1)
            {
                throw new ArgumentException("存在多种不同类型的秘籍!");
            }

            Recipes = items.ToArray();

            if (allIDArr.Length > 0)
            {
                SkillID = allIDs.First();
                SkillRecipeType = allRecipeTypes.First();
                SkillKey = items.First().SkillKey;
                
                Names = (from _ in Recipes select _.Name).ToArray();
                EffectSkillName = Recipes[0].EffectSkillName;

                var attrslist = from _ in items select _.SkillAttrs;

                SkillAttrs = SkillAttrCollection.Sum(attrslist);
                SSkillAttrs = SkillAttrs.Simplify();
            }
            else
            {
                SkillID = -1;
                SkillKey = "___EMPTY__";
                SkillAttrs = SkillAttrCollection.Empty;
                SSkillAttrs = SkillAttrCollection.SimplifiedEmpty;
            }

        }

        #endregion

        #region 简化

        #endregion
    }
}