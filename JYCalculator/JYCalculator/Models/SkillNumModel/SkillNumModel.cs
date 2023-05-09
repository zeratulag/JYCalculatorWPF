using JX3CalculatorShared.Common;

namespace JYCalculator.Models
{
    // 技能数模型
    public partial class SkillNumModel : IModel
    {
        /*public void CalcZXXWCD()
        {
            const string zx = "ZX";
            var normalfreq = Normal.FinalSkillFreq[zx];
            var xwfreq = XW.FinalSkillFreq[zx];
            XWCD = QiXue.SetZXXWCD(normalfreq, xwfreq);
        }*/

        public void CalcJYEnergyInjection()
        {
            // 计算最终心无CD
            Normal.FinalSkillFreq.CalcJYEnergyInjection();
            XW.FinalSkillFreq.CalcJYEnergyInjection();
            XWCD = QiXue.SetHJXWCD(Normal.FinalSkillFreq.EnergyInjectionFreq / 3, XW.FinalSkillFreq.EnergyInjectionFreq / 3);
        }

        public void CalcBaiYu()
        {
            Normal.FinalSkillFreq.CalcJYBaiYuDuoPo();
            XW.FinalSkillFreq.CalcJYBaiYuDuoPo();
        }
    }
}