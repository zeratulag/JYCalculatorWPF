using System;
using JX3CalculatorShared.Class;
using JYCalculator.Data;
using JYCalculator.Globals;

namespace JYCalculator.Class
{
    public partial class SkillData : SkillDataBase
    {
        #region 成员

        public new SkillInfoItem Info;

        #endregion

        #region 构造

        public SkillData(SkillInfoItem item) : base(item)
        {
            Info = item;
        }

        /// <summary>
        /// 重置此技能信息
        /// </summary>
        /// <returns></returns>
        public SkillData GetReset()
        {
            return new SkillData(Info);
        }

        public SkillData Copy()
        {
            return new SkillData(this);
        }

        public override void Update()
        {
            UpdateCoef();
        }

        /// <summary>
        /// 计算技能系数
        /// </summary>
        /// <param name="finalG">最终郭氏系数（分子）</param>
        /// <returns></returns>
        public override double CalcAPCoef(double finalG)
        {
            var res = Math.Max(16, (int) finalG) / (16.0 * XFConsts.ChannelIntervalToAPFactor);
            return res;
        }

        #endregion
    }
}