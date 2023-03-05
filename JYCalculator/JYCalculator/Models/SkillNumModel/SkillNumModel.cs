using JX3CalculatorShared.Common;

namespace JYCalculator.Models
{
    // 技能数模型
    public partial class SkillNumModel : IModel
    {
        public void CalcZXXWCD()
        {
            const string zx = "ZX";
            var normalfreq = Normal.FinalSkillFreq[zx];
            var xwfreq = XW.FinalSkillFreq[zx];
            XWCD = QiXue.SetZXXWCD(normalfreq, xwfreq);
        }
    }
}