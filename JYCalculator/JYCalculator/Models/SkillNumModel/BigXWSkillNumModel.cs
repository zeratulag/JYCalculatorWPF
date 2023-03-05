using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;
using System;

namespace JYCalculator.Models
{
    /// <summary>
    /// 大心无专用
    /// </summary>
    public class BigXWSkillNumModel : CommonSkillNumModel
    {

        public RestNum Rest; // 剩余技能数，表示一次爆发中没打完，留给常规阶段的技能次数

        public BigXWSkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste,
            AbilitySkillNumItem abilityitem,
            EquipOptionConfigModel equip, BigFMConfigModel bigfm,
            SkillNumModelArg arg) : base(qixue, skillhaste, abilityitem, equip, bigfm, arg)
        {
            if (!IsXW)
            {
                throw new ArgumentException("必须为心无状态下！");
            }

            Rest = new RestNum();
        }

        public new void Calc()
        {
            CommonCalcBefore();
            CalcBigXWSkillNum();
            CommonCalcAfter();
        }


        // 计算大心无期间的技能数 [TODO] 天风天绝
        public void CalcBigXWSkillNum()
        {
            var bl = GetBLFreqTime();
            Num.BL = bl.Freq * Num._Time;
        }
    }

    // 剩余次数
    public struct RestNum
    {
        public double FinalBullets; // 剩余的强化子弹数
        public double TJ; // 剩余天绝次数
        public double TJ_TF; // 剩余天风天绝数
        public double HX_XW; // 心无快照化血数
    }
}