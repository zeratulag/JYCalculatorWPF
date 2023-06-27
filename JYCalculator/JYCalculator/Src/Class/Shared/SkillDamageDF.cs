using System;
using JX3CalculatorShared.Class;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;

namespace JYCalculator.Class
{
    public class SkillDamageDF : AbsDataFrame<string, SkillDamage>
    {
        public new Dictionary<string, SkillDamage> Data;
        public FullCharacter FChar; // 人物属性
        public FullCharacter SnapFChar; // DOT快照的人物属性
        public Target CTarget;
        public SkillDataDF DataDf;
        public string BaseLineSkillName { get; protected set; }

        public SkillDamageDF(SkillDataDF datadf, FullCharacter fchar, FullCharacter snapfchar,
            Target target)
        {
            FChar = fchar;
            SnapFChar = snapfchar;
            CTarget = target;
            DataDf = datadf;

            Data = DataDf.Data.ToDictionary(_ => _.Key, _ => new SkillDamage(_.Value, FChar, CTarget));

            switch (AppStatic.XinFaTag)
            {
                case "JY":
                {
                    BaseLineSkillName = "DP";
                    break;
                }
                case "TL":
                {
                    BaseLineSkillName = "LN";
                    Data["HX_XW"].SetFChar(SnapFChar); // 修复心无化血快照的面板
                    break;
                }
            }
        }

        public void GetDamage()
        {
            Data[BaseLineSkillName].GetDamage();
            var baseline = Data[BaseLineSkillName].FinalEDamage;
            foreach (var _ in Data.Values)
            {
                _.GetDamage();
                _.CalcRelativeDamage(baseline);
            }
        }

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
    }
}