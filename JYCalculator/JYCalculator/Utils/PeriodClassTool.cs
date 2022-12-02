using JX3CalculatorShared.Common;
using JYCalculator.Class;

namespace JYCalculator.Utils
{
    public static class PeriodClassTool
    {
        public static SkillFreqCTs[] GetSkillFreqCTTable(this Period<SkillFreqCTDF> SkillFreqCTDf)
        {
            return SkillFreqCTs.GetSkillFreqCTTable(SkillFreqCTDf);
        }
    }
}