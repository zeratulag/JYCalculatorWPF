using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JX3CalculatorShared.Src
{
    public class SkillFreqDict : IEnumerable
    {
        #region 成员

        public readonly Dictionary<string, double> Data;

        #endregion

        #region 构建

        /// <summary>
        /// 直接指向data，不复制
        /// </summary>
        /// <param name="data"></param>
        public SkillFreqDict(Dictionary<string, double> data)
        {
            Data = data;
        }

        public SkillFreqDict(IDictionary<string, double> data, double time)
        {
            var dict = data.ToDictionary(_ => _.Key, _ => _.Value / time);
            Data = dict;
        }

        public SkillFreqDict(IDictionary<string, double> data) : this(data.ToDict())
        {
        }

        // 复制构造
        public SkillFreqDict(SkillFreqDict other) : this(other.Data.ToDict())
        {
        }

        #endregion

        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }

        public double this[string index]
        {
            set => Data.SetKeyValue(index, value);
            get => Data.GetValueOrUseDefault(index, 0);
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        // 频率到技能数的转化
        public SkillNumDict ToNum(double time)
        {
            var num = Data.ValueDictMultiply(time);
            return new SkillNumDict(num, time);
        }


        /// <summary>
        /// 按照一定频率增加技能
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void AddByFreq(string key, double freq)
        {
            Data.Add(key, freq);
        }

        /// <summary>
        /// 按照间隔增加技能
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="interval">技能间隔</param>
        public void AddByInterval(string key, double interval)
        {
            AddByFreq(key, 1 / interval);
        }

        #region 方法

        /// <summary>
        /// 对于一个触发事件，获取总触发频率（Hit次数）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public double GetEventHitFreq(SkillEventItem item)
        {
            var validNames = item.TriggerSkillNames.Intersect(Data.Keys); // 能够触发的技能名称
            double res = validNames.Sum(name => Data[name]);
            return res;
        }

        public Dictionary<string, double> GetEventsHitFreq(IEnumerable<SkillEventItem> items)
        {
            var res = new Dictionary<string, double>();
            foreach (var _ in items)
            {
                res.Add(_.Name, GetEventHitFreq(_));
            }

            return res;
        }

        #endregion

        /// <summary>
        /// 由于插入技能导致的技能频率变化
        /// </summary>
        /// <param name="coef">频率变化系数</param>
        public void AppplyFreqChange(double coef)
        {
            var keys = Data.Keys.ToArray();
            foreach (var key in keys)
            {
                if (!IsImmutableSkill(key))
                {
                    this[key] *= coef;
                }
            }
        }

        /// <summary>
        /// 判断一个技能是否不受频率损失影响（例如DOT）
        /// </summary>
        /// <param name="key">技能key</param>
        protected static bool IsImmutableSkill(string key)
        {
            var res = key.EndsWith("_DOT") || key.EndsWith("DOT_Hit");
            return res;
        }

        /// <summary>
        /// 刷新惊羽诀罡风频率，应该与Num.CalcGF保持同步
        /// </summary>
        /// <returns>更新后的罡风频率</returns>
        public double RefreshJYGFFreq()
        {
            var GF = this["DP"] + this["ZM"] + this["BL"] + this["_BYCast"] +
                     this["ZM_SF"] * 1 + this["ZX"] * 1.5;
            this["GF"] = GF;
            return GF;
        }
    }
}