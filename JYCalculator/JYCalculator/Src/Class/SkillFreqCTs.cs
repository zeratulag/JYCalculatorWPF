using JX3CalculatorShared.Common;
using JYCalculator.Src.Data;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class SkillFreqCTs
    {
        #region 成员

        public string Name { get; }
        public string SkillName { get; }
        public double NormalCT { get; set; } // 常规会心率
        public double NormalFreq { get; set; } // 常规频率
        public double XWCT { get; set; } // 心无会心率
        public double XWFreq { get; set; } // 心无频率

        public bool FreqGreaterThanZero => NormalFreq > 0 || XWFreq > 0; // 频率不为0

        #endregion

        #region 构造

        public SkillFreqCTs(string name)
        {
            Name = name;
            SkillName = StaticJYData.DB.SkillInfo.Get(Name).Skill_Name;
        }

        public SkillFreqCTs(SkillFreqCT data) : this(data.Name)
        {
        }

        #endregion

        // 录入常规信息
        public void SetNormal(SkillFreqCT data)
        {
            NormalCT = data.CT;
            NormalFreq = data.Freq;
        }

        // 录入心无信息
        public void SetXW(SkillFreqCT data)
        {
            XWCT = data.CT;
            XWFreq = data.Freq;
        }

        // 统计所有出现的技能中的常规信息和心无信息
        public static SkillFreqCTs[] GetSkillFreqCTTable(Period<SkillFreqCTDF> SkillFreqCTDf)
        {
            var dict = SkillFreqCTDf.Normal.Data.ToDictionary(_ => _.Key, _ => new SkillFreqCTs(_.Value));
            var res = new List<SkillFreqCTs>(dict.Count);
            foreach (var KVP in dict)
            {
                var key = KVP.Key;
                KVP.Value.SetNormal(SkillFreqCTDf.Normal.Data[key]);
                KVP.Value.SetXW(SkillFreqCTDf.XW.Data[key]);
                if (KVP.Value.FreqGreaterThanZero)
                {
                    res.Add(KVP.Value);
                }
            }
            return res.ToArray();
        }
    }
}