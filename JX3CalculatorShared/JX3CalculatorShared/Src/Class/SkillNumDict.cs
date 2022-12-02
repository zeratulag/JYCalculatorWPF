using JX3CalculatorShared.Utils;
using System.Collections;
using System.Collections.Generic;

namespace JX3CalculatorShared.Src
{
    public class SkillNumDict
    {
        #region 成员

        public readonly Dictionary<string, double> Data;

        public double Time;

        #endregion

        #region 构建

        public SkillNumDict(Dictionary<string, double> data, double time)
        {
            Data = data;
            Time = time;
        }

        public SkillNumDict(IDictionary<string, double> data, double time) : this(data.ToDict(), time)
        {
        }

        #endregion

        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }


        double this[string index]
        {
            set => Data.SetKeyValue(index, value);
            get => Data.GetValueOrUseDefault(index, 0);
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }


        // 技能数到频率的转化
        public SkillFreqDict ToFreq()
        {
            var freq = Data.ValueDictMultiply(1 / Time);
            return new SkillFreqDict(freq);
        }
    }
}