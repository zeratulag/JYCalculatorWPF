using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using System.Collections.Generic;

namespace JX3CalculatorShared.Models
{
    public class EquipOptionConfigModelBase : IModel
    {
        public WPOption WP;
        public YZOption YZ;
        public bool JN;
        public bool SL;
        public List<Recipe> OtherRecipes; // 装备选项附带的秘籍
        public List<string> AssociateKeys;
        public List<SkillEventItem> SkillEvents;

        public EquipOptionConfigModelBase()
        {
            OtherRecipes = new List<Recipe>(8);
            AssociateKeys = new List<string>(4);
            SkillEvents = new List<SkillEventItem>(4);
        }

        public EquipOptionConfigModelBase(WPOption wp, YZOption yz, bool jn, bool sl) : this()
        {
            UpdateInput(wp, yz, jn, sl);
        }

        public virtual void UpdateInput(WPOption wp, YZOption yz, bool jn, bool sl)
        {
            WP = wp;
            YZ = yz;
            JN = jn;
            SL = sl;
            Calc();
        }


        public virtual void Calc()
        {

        }

    }
}