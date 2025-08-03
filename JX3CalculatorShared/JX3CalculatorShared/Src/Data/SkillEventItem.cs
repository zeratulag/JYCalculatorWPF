using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JYCalculator.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;

namespace JX3CalculatorShared.Data
{
    public partial class SkillEventItem
    {
        public SkillEventTypeMask EventTypeMask { get; private set; }
        public HashSet<string> TriggerSkillNames => EventTypeMask.TriggerSkillNames;
        public HashSet<int> TriggerSkillIDs => EventTypeMask.TriggerSkillIDs;

        #region 方法

        public bool CanTrigger(SkillInfoItemBase skillItem)
        {
            return skillItem.CanTrigger(this);
        }

        public IEnumerable<string> SkillTrigger(IEnumerable<SkillInfoItemBase> skillItems)
        {
            var res = from item in skillItems
                where CanTrigger(item)
                select item.Name;
            return res.ToImmutableArray();
        }

        //// 设置可以触发的技能名
        //public void SetTriggerSkillNames(IEnumerable<string> names)
        //{
        //    TriggerSkillNames = names.ToHashSet();
        //}

        public void Parse()
        {
            Prob = Odds / 1024.0;
            EventTypeMask = SkillEventTypeMaskManager.Create(EventType, EventMask1, EventMask2);
            EventTypeMask.SkillEventItems.Add(this);
        }

        #endregion

        /// <summary>
        /// 基于神力事件，制作破招触发
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static SkillEventItem GetPZEventItemFromSL(SkillEventItem item)
        {
            if (item.Name != "SL")
            {
                throw new ArgumentException("必须以神力为输入！");
            }

            var res = item.DeepClone();
            res.Name = "BaseSurplus";
            res.EventName = "破招";
            res.Type = "破招";
            res.Associate = "";
            res.DescName = "破招";
            res.EventType = SkillEventTypeEnum.Hit;
            res.Odds = 1024;
            res.Prob = 1.0;
            return res;
        }


        /// <summary>
        /// 计算事件的平均触发间隔
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <param name="prob">单次hit触发概率</param>
        /// <param name="cd">CD时间</param>
        /// <returns>平均触发间隔</returns>
        public static double MeanTriggerInterval(double hitfreq, double prob, double cd)
        {
            var res = 1 / (prob * hitfreq) + cd;
            return res;
        }

        /// <summary>
        /// 计算事件的平均触发间隔
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <param name="cd">CD时间</param>
        /// <returns>平均触发间隔</returns>
        public double MeanTriggerInterval(double hitfreq, double cd)
        {
            return MeanTriggerInterval(hitfreq, Prob, cd);
        }

        /// <summary>
        /// 计算事件的平均触发间隔
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <returns>平均触发间隔</returns>
        public double MeanTriggerInterval(double hitfreq)
        {
            return MeanTriggerInterval(hitfreq, Prob, CD);
        }

        /// <summary>
        /// 计算事件的平均触发频率
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <returns></returns>
        public double TriggerFreq(double hitfreq)
        {
            return 1 / MeanTriggerInterval(hitfreq);
        }

        /// <summary>
        /// 计算无内置CD的BUFF类事件的平均覆盖率
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <param name="prob">单次触发概率</param>
        /// <returns>BUFF覆盖率</returns>
        public double BuffCoverRate(double hitfreq, double prob)
        {
            var hitn = Time * hitfreq;
            var res = 1 - Math.Pow(Math.Max(0, 1 - prob), hitn);
            return res;
        }

        /// <summary>
        /// 计算无内置CD的BUFF类事件的平均覆盖率
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <param name="prob">单次触发概率</param>
        /// <returns>BUFF覆盖率</returns>
        public double BuffCoverRate(double hitfreq)
        {
            return BuffCoverRate(hitfreq, Prob);
        }

        /// <summary>
        /// 计算有内置CD的BUFF类事件的平均覆盖率
        /// </summary>
        /// <param name="hitfreq">hit频率</param>
        /// <param name="prob">单次触发概率</param>
        /// <param name="time">持续时间</param>
        /// <param name="cd">CD时间</param>
        /// <returns>BUFF覆盖率</returns>
        public static double CDBuffCoverRate(double hitfreq, double prob, double time, double cd)
        {
            var interval = MeanTriggerInterval(hitfreq, prob, cd);
            var res = Math.Min(time / interval, 1);
            return res;
        }

        public double CDBuffCoverRate(double hitfreq)
        {
            return CDBuffCoverRate(hitfreq, Prob, Time, CD);
        }
    }
}