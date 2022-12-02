using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.Src.Data
{
    public partial class SkillInfoItem
    {
        public bool IsP => Type == SkillDataTypeEnum.PZ; // 是否为类破招技能

        #region 方法

        /// <summary>
        /// 判断技能能否触发事件
        /// </summary>
        /// <param name="skillInfoItem">技能</param>
        /// <param name="eventItem">技能事件</param>
        /// <returns>能否触发</returns>
        public static bool CanTrigger(SkillInfoItem skillInfoItem, SkillEventItem eventItem)
        {
            if (skillInfoItem.Type == SkillDataTypeEnum.DOT || skillInfoItem.Type == SkillDataTypeEnum.PZ)
            {
                // DOT和破招无法触发任何事件
                return false;
            }

            bool skill1 = (skillInfoItem.SkillEventMask1 & eventItem.EventMask1) > 0;
            bool skill2 = (skillInfoItem.SkillEventMask2 & eventItem.EventMask2) > 0;
            bool cast1 = (skillInfoItem.Cast_SkillEventMask1 & eventItem.EventMask1) > 0;
            bool cast2 = (skillInfoItem.Cast_SkillEventMask2 & eventItem.EventMask2) > 0;
            bool eventskill = (skillInfoItem.SkillID > 0) && (skillInfoItem.SkillID == eventItem.EventSkillID);
            bool res = skill1 || skill2 || cast1 || cast2 || eventskill;
            return res;
        }

        public bool CanTrigger(SkillEventItem eventItem)
        {
            return CanTrigger(this, eventItem);
        }

        public IEnumerable<string> TriggerEvent(IEnumerable<SkillEventItem> eventItems)
        {
            var res = from item in eventItems
                where CanTrigger(item)
                select item.Name;
            return res.ToImmutableArray();
        }


        /// <summary>
        /// 判断技能是否吃秘籍
        /// </summary>
        /// <param name="info">技能</param>
        /// <param name="recipe">秘籍</param>
        /// <returns></returns>
        public static bool CanEffectRecipe(SkillInfoItem info, Recipe recipe)
        {
            bool res = false;

            if (recipe.EffectSkillName.Count > 0)
            {
                res = recipe.EffectSkillName.Contains(info.Name);
            }
            else
            {
                bool skillrecipe = (info.SkillID > 0) && (info.SkillID == recipe.SkillID);
                bool skillrecipeType =
                    (info.RecipeType > 0) && (info.RecipeType == recipe.SkillRecipeType);
                res = skillrecipe || skillrecipeType;
            }
            return res;
        }

        public bool CanEffectRecipe(Recipe recipe)
        {
            return CanEffectRecipe(this, recipe);
        }

        public IEnumerable<string> EffectRecipe(IEnumerable<Recipe> recipes)
        {
            var res = from item in recipes
                where CanEffectRecipe(item)
                select item.Name;
            return res.ToImmutableArray();
        }

        #endregion
    }
}