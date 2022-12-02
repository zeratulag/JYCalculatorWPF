using JX3CalculatorShared.Src.Data;
using System;

namespace JX3CalculatorShared.Src.Class
{
    public class Ability
    {
        #region 成员

        public readonly int Rank; // 手法水平，0~3

        public readonly string Desc;

        public readonly string Name;

        public readonly string Genre; // 流派

        public string ItemName { get; }
        public string ToolTip { get; }

        public readonly AbilitySkillNumItemBase Normal;
        public readonly AbilitySkillNumItemBase BigXW;

        #endregion

        #region 构造

        public Ability(AbilityItem abilityitem, AbilitySkillNumItemBase normal, AbilitySkillNumItemBase bigXW)
        {
            if (abilityitem.Rank != normal.Rank || abilityitem.Rank != bigXW.Rank)
            {
                throw new ArgumentException("Rank必须配对！");
            }

            Rank = abilityitem.Rank;
            ItemName = abilityitem.ItemName;
            Desc = abilityitem.Desc;
            ToolTip = abilityitem.ToolTip;

            Normal = normal;
            BigXW = bigXW;

            Genre = normal.Genre;

        }

        #endregion

        #region 方法


        #endregion

    }
}