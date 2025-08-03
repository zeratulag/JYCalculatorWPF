using System.CodeDom;
using JX3CalculatorShared.Common;
using JYCalculator.Data;
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
            SkillName = StaticXFData.DB.AllSkillInfo.Get(Name).SkillName;
        }

        public SkillFreqCTs(SkillFreqCriticalStrike data) : this(data.Name)
        {
        }

        #endregion

        // 录入常规信息
        public void SetNormal(SkillFreqCriticalStrike data)
        {
            NormalCT = data.CriticalStrikeValue;
            NormalFreq = data.Freq;
        }

        // 录入心无信息
        public void SetXW(SkillFreqCriticalStrike data)
        {
            XWCT = data.CriticalStrikeValue;
            XWFreq = data.Freq;
        }

        // 统计所有出现的技能中的常规信息和心无信息
        public static SkillFreqCTs[] GetSkillFreqCTTable(Period<SkillFreqCTDF> SkillFreqCTDf)
        {
            var dict = SkillFreqCTDf.Normal.Data.ToDictionary(_ => _.Key, _ => new SkillFreqCTs(_.Value));
            var dict2 = SkillFreqCTDf.XinWu.Data.ToDictionary(_ => _.Key, _ => new SkillFreqCTs(_.Value));
            foreach (var item in dict2.Where(item => !dict.ContainsKey(item.Key)))
            {
                dict.Add(item.Key, item.Value);
            }

            var res = new List<SkillFreqCTs>(dict.Count);
            foreach (var KVP in dict)
            {
                var key = KVP.Key;
                if (SkillFreqCTDf.Normal.Data.TryGetValue(key, out var normal_value))
                {
                    KVP.Value.SetNormal(normal_value);
                }

                if (SkillFreqCTDf.XinWu.Data.TryGetValue(key, out var xinwu_value))
                {
                    KVP.Value.SetXW(xinwu_value);
                }

                if (KVP.Value.FreqGreaterThanZero)
                {
                    res.Add(KVP.Value);
                }
            }
            return res.ToArray();
        }
    }
}