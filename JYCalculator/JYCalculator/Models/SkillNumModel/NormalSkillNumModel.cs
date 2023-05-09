using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;

namespace JYCalculator.Models
{
    public class NormalSkillNumModel : CommonSkillNumModel
    {
        #region 成员

        public RestNum Rest; // 剩余技能数，表示一次爆发中没打完，留给常规阶段的技能次数

        #endregion

        #region 构造

        public NormalSkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste,
            AbilitySkillNumItem abilityitem,
            EquipOptionConfigModel equip, BigFMConfigModel bigfm,
            SkillNumModelArg arg) :
            base(qixue, skillhaste, abilityitem, equip, bigfm, arg)
        {
            Rest = new RestNum();
        }

        public NormalSkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste, AbilitySkillNumItem abilityitem,
            EquipOptionConfigModel equip,
            BigFMConfigModel bigfm,
            SkillNumModelArg arg,
            RestNum rest) : this(qixue, skillhaste, abilityitem, equip, bigfm, arg)
        {
            Rest = rest;
        }

        #endregion

        public new void Calc()
        {
            CommonCalcBefore();
            CalcNormalSkillNum();
        }

        /// <summary>
        /// 计算常规状态下的技能数
        /// </summary>
        public void CalcNormalSkillNum()
        {
            CalcBL();
        }

    }

}