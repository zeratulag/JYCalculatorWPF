using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class SkillFreqCTDF
    {
        #region 成员

        public readonly Dictionary<string, SkillFreqCT> Data;
        public readonly FullCharacter FChar;
        public readonly SkillDataDF SkillDatas;
        #endregion

        #region 构建

        public SkillFreqCTDF(SkillFreqCTDF old)
        {
            FChar = old.FChar.Copy();
            Data = old.Data.ToDictionary(_ => _.Key, _ => _.Value.Copy());
            SkillDatas = old.SkillDatas.Copy();
        }

        public SkillFreqCTDF Copy()
        {
            return new SkillFreqCTDF(this);
        }


        public SkillFreqCTDF(SkillDataDF df, FullCharacter fchar, SkillFreqDict freq)
        {
            Data = new Dictionary<string, SkillFreqCT>(freq.Data.Count);
            FChar = fchar;
            SkillDatas = df;
            UpdateInput(df, fchar, freq);
        }

        public void UpdateInput(SkillDataDF df, FullCharacter fchar, SkillFreqDict freq)
        {
            foreach (var KVP in freq.Data)
            {
                var key = KVP.Key;
                if (df.Data.ContainsKey(key))
                {
                    var value = new SkillFreqCT(df.Data[key], fchar, KVP.Value);
                    Data.SetKeyValue(key, value);
                }
            }
        }

        public void UpdateFChar()
        {
            foreach (var V in Data.Values)
            {
                V.UpdateFChar(FChar);
            }
        }

        /// <summary>
        /// 以特定频率增加技能
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void AddSkillFreq(string key, double freq)
        {
            if (SkillDatas.Data.ContainsKey(key))
            {
                var newFreqCT = new SkillFreqCT(SkillDatas.Data[key], FChar, freq);
                Data.Add(key, newFreqCT);
            }
        }

        /// <summary>
        /// 修正特定技能频率
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void SetSkillFreq(string key, double freq)
        {
            if (Data.TryGetValue(key, out var value))
            {
                value.Freq = freq;
            }
        }

        /// <summary>
        /// 增加特定技能频率
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="dfreq">delta技能频率，增量</param>
        public void ModifySkillFreq(string key, double dfreq)
        {
            if (Data.TryGetValue(key, out var value))
            {
                value.Freq += dfreq;
            }
        }

        /// <summary>
        /// 从技能1转移频率给技能2
        /// </summary>
        /// <param name="srcKey">源技能key</param>
        /// <param name="dstKey">目标技能key</param>
        /// <param name="dfreq">频率值</param>
        public void TransSkillFreq(string srcKey, string dstKey, double dfreq)
        {
            ModifySkillFreq(srcKey, -dfreq);
            ModifySkillFreq(dstKey, dfreq);
        }


        public SkillFreqCT[] ToArr()
        {
            var res = Data.Values.Where(_ => _.Freq > 0);
            return res.ToArray();
        }

        public string[] GetValidNames()
        {
            var valids = Data.Where(_ => _.Value.Freq > 0);
            var res = from _ in valids select _.Key;
            return res.ToArray();
        }

        #endregion

        // 获取给定技能的总频率和平均会心率
        public (double SumFreq, double MeanCT) GetMeanCT(ISet<string> names)
        {
            var items = from _ in Data where names.Contains(_.Key) where _.Value.Freq > 0 select _.Value;

            double sumct = 0;
            double sumfreq = 0;
            foreach (var item in items)
            {
                sumct += item.CT * item.Freq;
                sumfreq += item.Freq;
            }

            var meanct = sumfreq > 0 ? sumct / sumfreq : 0;
            var res = (sumfreq, meanct);
            return res;
        }


        // 获取给定技能的 频率*会心 之和
        public double GetSumCTFreq(ISet<string> names)
        {
            var items = from _ in Data where names.Contains(_.Key) where _.Value.Freq > 0 select _.Value;
            var res = items.Sum(item => item.CT * item.Freq);
            return res;
        }

    }
}