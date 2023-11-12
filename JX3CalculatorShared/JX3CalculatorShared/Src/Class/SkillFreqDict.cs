using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.Globals;
using JYCalculator.Class;
using HandyControl.Controls;

namespace JX3CalculatorShared.Class
{
    public class SkillFreqDict : IEnumerable
    {
        #region 成员

        public readonly Dictionary<string, double> Data;

        public double EnergyInjectionFreq; // 注能频率
        public double PZEnergyInjectionFreq; // 破招注能频率（逐星注能不触发破招）
        public double HanJiangFreq; // 寒江触发频率

        public readonly Dictionary<string, double> GFDict = new Dictionary<string, double>()
        {
            {"DP", 1}, {"ZM", 1}, {"BL", 1}, {"_BYCast", 1}, {"ZM_SF", 1}, {"ZX", 1}, {"CXL", 1},
        }; // 每个技能附带的罡风数

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
        /// 为已有技能增加频率
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void AddSkillFreq(string key, double freq)
        {
            Data[key] += freq;
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


        #region 惊羽专用

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

        public void CalcJYBaiYuTiaoZhu()
        {
            // 计算惊羽白雨跳珠
            const string zmsf = "ZM_SF"; // 白雨跳珠次数=顺发追命次数
            double byfreq = 0;
            Data.TryGetValue(zmsf, out byfreq);
            Data.SetKeyValue("BaiYuTiaoZhu", byfreq); // 白雨跳珠（非侠士）

            double zx_org_freq = 0;
            Data.TryGetValue("_ZX_Org", out zx_org_freq);
            var bypzfreq = zx_org_freq * XFStaticConst.PZ_BaiYuPer_Normal_ZX;
            Data.SetKeyValue("PZ_BaiYu", bypzfreq); // 白雨跳珠（逐星破招）
        }

        public double CalcJYEnergyInjection()
        {
            // 计算惊羽注能次数
            double pzRes = 0.0;
            var res = StaticXFData.DB.SkillInfo.GetEnergyInjection(Data);
            Data.TryGetValue("_ZX_Org", out var zx_org); // 修正逐星充能，防止橙武连续逐星影响
            Data.TryGetValue("ZX", out double zx);
            res = res - zx + zx_org;
            pzRes = res;
            EnergyInjectionFreq = res;
            PZEnergyInjectionFreq = pzRes;
            Data.SetKeyValue("PZ", PZEnergyInjectionFreq / 3); // 3注能一次破招
            return EnergyInjectionFreq;
        }

        public double GetJYHanJiangFreq()
        {
            // 计算寒江触发频率
            CalcJYBaiYuTiaoZhu();
            CalcJYEnergyInjection();
            double res = 0.0;
            Data.TryGetValue("ZX", out double zx); // 常规逐星触发寒江
            Data.TryGetValue("PZ", out double pz);
            Data.TryGetValue("PZ_BaiYu", out double pz_by);
            res = zx + pz + pz_by;
            HanJiangFreq = res;
            return res;
        }


        /// <summary>
        /// 重新调整百里频率
        /// </summary>
        /// <param name="newFreq">新的百里频率</param>
        /// <param name="BLTime">百里时间</param>
        public void ResetBLFreq(double newFreq, double BLTime, JYSkillCountItem count)
        {
            if (Data.TryGetValue("BL", out var oldFreq))
            {
                var oldcoef = 1 - oldFreq * BLTime; // 因为打百里导致其他技能少打的损失系数
                var newcoef = 1 - newFreq * BLTime;
                var fixcoef = newcoef / oldcoef; // 其他技能频率的修正系数

                var effectNames = new HashSet<string>(JYSkillCountItem.BLEffectNames);
                foreach (var kvp in count.FieldToDictionary)
                {
                    if (effectNames.Contains(kvp.Key))
                    {
                        effectNames.AddRange(kvp.Value);
                    }
                }

                foreach (var _ in effectNames)
                {
                    if (Data.TryGetValue(_, out var oldValue))
                    {
                        Data[_] = oldValue * fixcoef;
                    }
                }
                Data["BL"] = newFreq;
            }
        }

        /// <summary>
        /// 降低夺魄频率，增加给穿心弩
        /// </summary>
        /// <param name="freq">频率值</param>
        public void MoveDPToCXL(double freq)
        {
            if (Data.TryGetValue("CXL", out var oldCXL))
            {
                // 穿心弩频率不能降低，这部分由夺魄支付
                Data["DP"] -= freq;
                Data["CXL"] += freq;
            }
        }

        public double CalcGF()
        {
            double res = 0.0;
            foreach (var kvp in GFDict)
            {
                if (Data.TryGetValue(kvp.Key, out double freq))
                {
                    res += freq * kvp.Value;
                }
            }

            Data["GF"] = res;
            return res;

        }


        #endregion
    }
}