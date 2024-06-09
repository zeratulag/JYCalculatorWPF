using JX3CalculatorShared.Common;

namespace JYCalculator.Models
{
    // 技能数模型
    public partial class SkillNumModel : IModel
    {
        /*public void CalcZXXWCD()
        {
            const string zx = SkillKeyConst.逐星箭;
            var normalfreq = Normal.FinalSkillFreq[zx];
            var xwfreq = XW.FinalSkillFreq[zx];
            XWCD = QiXue.SetZXXWCD(normalfreq, xwfreq);
        }*/

        public void CalcJYEnergyInjection()
        {
            // 计算最终心无CD
            Normal.FinalSkillFreq.CalcJYEnergyInjection();
            XW.FinalSkillFreq.CalcJYEnergyInjection();
            var normal = Normal.FinalSkillFreq.GetJYHanJiangFreq();
            var xw = XW.FinalSkillFreq.GetJYHanJiangFreq();
            XWCD = QiXue.SetHJXWCD(normal, xw);
        }

        public void CalcBaiYu()
        {
            if (QiXue.白雨跳珠)
            {
                Normal.FinalSkillFreq.CalcJYBaiYuTiaoZhu();
                XW.FinalSkillFreq.CalcJYBaiYuTiaoZhu();
            }
        }
    }
}