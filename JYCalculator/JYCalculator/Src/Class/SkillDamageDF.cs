using System.Collections.Generic;
using System.Linq;
using JYCalculator.Src;
using JYCalculator.Src.Class;

namespace JYCalculator.Class
{
    public class SkillDamageDF
    {
        #region 成员
        
        public readonly Dictionary<string, SkillDamage> Data;

        public readonly FullCharacter FChar; // 人物属性
        public readonly FullCharacter SnapFChar; // DOT快照的人物属性
        public readonly Target CTarget;
        public readonly SkillDataDF DataDf;


        #endregion

        #region 构造

        public SkillDamageDF(SkillDataDF datadf, FullCharacter fchar, FullCharacter snapfchar,
            Target target)
        {
            FChar = fchar;
            SnapFChar = snapfchar;
            CTarget = target;
            DataDf = datadf;

            Data = DataDf.Data.ToDictionary(_ => _.Key, _ => new SkillDamage(_.Value, FChar, CTarget));
            
        }

        #endregion

        #region 方法

        public void GetDamage()
        {
            Data["DP"].GetDamage();
            var baseline = Data["DP"].FinalEDamage;
            foreach (var _ in Data.Values)
            {
                _.GetDamage();
                _.CalcRelativeDamage(baseline);
            }
        }

        // 增加频率信息
        public void AttachFreq(SkillFreqCTDF freqdf)
        {
            foreach (var KVP in freqdf.Data)
            {
                var key = KVP.Key;
                var freq = KVP.Value.Freq;
                Data[key].SetFreq(freq);
                Data[key].GetDPS();

            }
        }

        public void ClearFreq()
        {
            foreach (var _ in Data.Values)
            {
                _.SetFreq(0);
            }
        }

        public void GetDeriv()
        {
            foreach (var _ in Data.Values)
            {
                _.GetDeriv();
            }
        }


        #endregion

    }
}