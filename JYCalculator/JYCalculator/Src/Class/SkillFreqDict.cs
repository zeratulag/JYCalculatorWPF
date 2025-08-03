using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using JYCalculator.Class.SkillCount;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Src;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JX3CalculatorShared.Globals;
using Serilog;

namespace JYCalculator.Class
{
    public struct SkillFreqArg
    {
        public readonly bool 白雨跳珠;
        public readonly bool 百步凝形;

        public SkillFreqArg(bool BaiYu, bool BaiBuNingXing = false)
        {
            白雨跳珠 = BaiYu;
            百步凝形 = BaiBuNingXing;
        }
    }

    public class SkillFreqDict : IEnumerable
    {
        #region 成员

        public readonly Dictionary<string, double> Data;

        public double EnergyInjectionFreq; // 注能频率
        public double PZEnergyInjectionFreq; // 破招注能频率（逐星注能不触发破招）
        public double HanJiangFreq; // 寒江触发频率
        public readonly SkillFreqArg Arg;

        public readonly Dictionary<string, double>
            SkillEventTypeMaskHitFreqCache = new Dictionary<string, double>(20); // 缓存的技能事件触发基础频率

        #endregion

        #region 公用数据字典

        public static readonly Dictionary<string, double> GangFengPerSkill = new Dictionary<string, double>()
        {
            {SkillKeyConst._夺魄箭_释放, 1}, {SkillKeyConst._追命箭_释放, 1}, {SkillKeyConst._追命箭_瞬发_释放, 1},
            {SkillKeyConst.百里追魂, 1}, {SkillKeyConst._暴雨梨花针_释放, 1}, {SkillKeyConst.逐星箭, 1},
            {SkillKeyConst.穿心弩, 1}, {SkillKeyConst.孔雀翎, 1}
        }; // 每个技能附带的罡风数

        public static readonly Dictionary<string, double> WuShengPerSkill = new Dictionary<string, double>()
        {
            {SkillKeyConst._夺魄箭_释放, 1}, {SkillKeyConst._暴雨梨花针_释放, 1}, {SkillKeyConst.逐星箭, 1}, {"_XinWuCast", 1},
        }; // 每个技能叠加的无声数

        public static readonly Dictionary<string, double> LveYingQiongCangPerSkill = new Dictionary<string, double>()
        {
            {SkillKeyConst._夺魄箭_释放, 1}, {SkillKeyConst._追命箭_释放, 1}, {SkillKeyConst._追命箭_瞬发_释放, 1},
            {SkillKeyConst.百里追魂, 1}, {SkillKeyConst.逐星箭, 1},
            {SkillKeyConst.穿心弩, 1}, {SkillKeyConst.孔雀翎, 5 - 1}
        }; // 每个技能附带的掠影穹苍数据


        public static readonly string[] DuoPoKeys = new[] {SkillKeyConst.夺魄箭, SkillKeyConst.夺魄箭_牢甲利兵}; // 夺魄可能的Key

        #endregion


        #region 构建

        /// <summary>
        /// 直接指向data，不复制
        /// </summary>
        /// <param name="data"></param>
        public SkillFreqDict(Dictionary<string, double> data, SkillFreqArg arg)
        {
            Data = data;
            Arg = arg;
        }

        public SkillFreqDict(IDictionary<string, double> data, double time, SkillFreqArg arg)
        {
            var dict = data.ToDictionary(_ => _.Key, _ => _.Value / time);
            Data = dict;
            Arg = arg;
        }

        public SkillFreqDict(IDictionary<string, double> data, SkillFreqArg arg) :
            this(data.ToDict(), arg)
        {
        }

        // 复制构造
        public SkillFreqDict(SkillFreqDict other) :
            this(other.Data.ToDict(), other.Arg)
        {
        }

        public string GetDPKey()
        {
            var res = DuoPoKeys.First();
            foreach (var key in DuoPoKeys)
            {
                if (this[key] > 0)
                {
                    res = key;
                    break;
                }
            }

            return res;
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

        private void ClearCache()
        {
            SkillEventTypeMaskHitFreqCache.Clear();
        }

        /// <summary>
        /// 按照一定频率增加技能
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void AddByFreq(string key, double freq)
        {
            Data.Add(key, freq);
            ClearCache();
        }


        /// <summary>
        /// 为已有技能增加频率
        /// </summary>
        /// <param name="key">技能key</param>
        /// <param name="freq">技能频率</param>
        public void AddSkillFreq(string key, double freq)
        {
            Data[key] += freq;
            ClearCache();
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
            return GetEventHitFreq(item.EventTypeMask);
        }

        // 对于有CD的触发事件，计算其平均触发间隔
        public double CalcMeanTriggerInterval(SkillEventItem item)
        {
            if (item.EventType == SkillEventTypeEnum.CriticalStrike)
            {
                Log.Error("错误的事件类型！{SkillEventTypeMask}", item);
                return 0;
            }

            var hitFreq = GetEventHitFreq(item);
            var res = item.MeanTriggerInterval(hitFreq);
            return res;
        }

        // 对于有CD的Buff类触发事件，计算其Buff平均覆盖率
        public double CalcCDBuffCoverRate(SkillEventItem item)
        {
            if (item.EventType == SkillEventTypeEnum.CriticalStrike)
            {
                Log.Error("错误的事件类型！{SkillEventTypeMask}", item);
                return 0;
            }

            var hitFreq = GetEventHitFreq(item);
            var res = item.CDBuffCoverRate(hitFreq);
            return res;
        }


        /// <summary>
        /// 对于一个触发事件，获取总触发频率（Hit次数）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private double _CalcEventHitFreq(SkillEventTypeMask item)
        {
            if (item.EventType == SkillEventTypeEnum.CriticalStrike)
            {
                Log.Error("错误的事件类型！{SkillEventTypeMask}", item);
                return 0;
            }

            var validNames = item.TriggerSkillNames.Intersect(Data.Keys); // 能够触发的技能名称
            double res = validNames.Sum(name => Data[name]);
            return res;
        }

        public double GetEventHitFreq(SkillEventTypeMask item)
        {
            double res = 0.0;
            if (SkillEventTypeMaskHitFreqCache.TryGetValue(item.Name, out res)) return res;
            res = _CalcEventHitFreq(item);
            SkillEventTypeMaskHitFreqCache.Add(item.Name, res);
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

            ClearCache();
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
            var GF = this[SkillKeyConst.夺魄箭] + this[SkillKeyConst.追命箭] + this[SkillKeyConst.百里追魂] +
                     this[SkillKeyConst._暴雨梨花针_释放] +
                     this[SkillKeyConst.追命箭_瞬发] * 1 + this[SkillKeyConst.逐星箭] * 1.5;
            this[SkillKeyConst.罡风镖法] = GF;
            ClearCache();
            return GF;
        }

        public void CalcJYBaiYuTiaoZhu()
        {
            if (!Arg.白雨跳珠) return;
            // 计算惊羽白雨跳珠
            const string zmsf = SkillKeyConst.追命箭_瞬发; // 白雨跳珠次数=顺发追命次数
            double byfreq = 0;
            Data.TryGetValue(zmsf, out byfreq);
            Data.SetKeyValue(SkillKeyConst.白雨跳珠, byfreq); // 白雨跳珠（非侠士）

            double zx_org_freq = 0;
            Data.TryGetValue("_ZX_Org", out zx_org_freq);
            var bypzfreq = zx_org_freq * XFStaticConst.PZ_BaiYuPer_Normal_ZX;
            Data.SetKeyValue(SkillKeyConst.破_白雨跳珠, bypzfreq); // 白雨跳珠（逐星破招）
        }

        public double CalcJYEnergyInjection()
        {
            // 计算惊羽注能次数
            if (Arg.百步凝形)
            {
                // 此时基于破招反推注能数
                var pz = Data[SkillKeyConst.破];
                PZEnergyInjectionFreq = pz * XFStaticConst.EnergyInjectionFreqToPZCoef;
                EnergyInjectionFreq = PZEnergyInjectionFreq;
            }
            else
            {
                double pzRes = 0.0;
                var res = StaticXFData.DB.BaseSkillInfo.GetEnergyInjection(Data);
                Data.TryGetValue("_ZX_Org", out var zx_org); // 修正逐星充能，防止橙武连续逐星影响
                Data.TryGetValue(SkillKeyConst.逐星箭, out double zx);
                res = res - zx + zx_org;
                pzRes = res;
                EnergyInjectionFreq = res;
                PZEnergyInjectionFreq = pzRes;
                Data.SetKeyValue(SkillKeyConst.破,
                    PZEnergyInjectionFreq / XFStaticConst.EnergyInjectionFreqToPZCoef); // 3注能一次破招
            }

            return EnergyInjectionFreq;
        }

        public double GetJYHanJiangFreq()
        {
            // 计算寒江触发频率
            CalcJYBaiYuTiaoZhu();
            CalcJYEnergyInjection();
            double res = 0.0;
            Data.TryGetValue(SkillKeyConst.逐星箭, out double zx); // 常规逐星触发寒江
            Data.TryGetValue(SkillKeyConst.破, out double pz);
            Data.TryGetValue(SkillKeyConst.破_白雨跳珠, out double pz_by);
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
            if (Data.TryGetValue(SkillKeyConst.百里追魂, out var oldFreq))
            {
                var oldcoef = 1 - oldFreq * BLTime; // 因为打百里导致其他技能少打的损失系数
                var newcoef = 1 - newFreq * BLTime;
                var fixcoef = newcoef / oldcoef; // 其他技能频率的修正系数

                var effectNames = new HashSet<string>();
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

                Data[SkillKeyConst.百里追魂] = newFreq;
            }

            ClearCache();
        }

        /// <summary>
        /// 降低夺魄频率，增加给穿心弩
        /// </summary>
        /// <param name="freq">频率值</param>
        public void MoveDPToCXL(double freq)
        {
            if (Data.TryGetValue(SkillKeyConst.穿心弩, out var oldCXL))
            {
                // 穿心弩频率不能降低，这部分由夺魄支付
                var key = GetDPKey();
                Data[key] -= freq;
                Data[SkillKeyConst.穿心弩] += freq;
            }

            ClearCache();
        }

        /// <summary>
        /// 基于权重字典，求出频率和
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public double CalcSumByDict(IDictionary<string, double> dict)
        {
            double res = 0.0;
            foreach (var kvp in dict)
            {
                if (Data.TryGetValue(kvp.Key, out double freq))
                {
                    res += freq * kvp.Value;
                }
            }

            return res;
        }


        public double CalcGangFeng()
        {
            double res = CalcSumByDict(GangFengPerSkill);
            Data[SkillKeyConst.罡风镖法] = res;
            ClearCache();
            return res;
        }

        /// <summary>
        /// 追夺流下罡风会变少，进行修正
        /// </summary>
        public void FixGangFengOnZhuiDuo()
        {
            Data[SkillKeyConst.罡风镖法] *= 0.845;
            ClearCache();
        }


        // 计算无声计数
        public double CalcWuSheng()
        {
            var res = CalcSumByDict(WuShengPerSkill);
            return res;
        }

        public void CalcNieJingZhuiMingFreq()
        {
            var wuSheng = CalcWuSheng();
            var zhuiMing = Data[SkillKeyConst._追命箭_瞬发_释放];
            var NJSolver = new NieJingZhuiMing(wuSheng, zhuiMing, true);
            NJSolver.DoWork();
            Data[SkillKeyConst.追命箭_瞬发] = 0;
            foreach (var kvp in NJSolver.Result)
            {
                Data[kvp.Key] = kvp.Value;
            }

            ClearCache();
        }

        #endregion

        public void CalcLveYingQiongCangFreq()
        {
            double res = CalcSumByDict(LveYingQiongCangPerSkill);
            Data[SkillKeyConst.掠影穹苍] = res;
            ClearCache();
        }
    }
}