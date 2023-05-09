using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using System.Collections.Generic;

namespace JYCalculator.Models
{
    public partial class CommonSkillNumModel
    {
        #region 成员

        public readonly QiXueConfigModel QiXue;
        public readonly SkillHasteTable SkillHaste;
        public readonly EquipOptionConfigModel Equip;
        public readonly BigFMConfigModel BigFM;
        public readonly SkillNumModelArg Arg;
        public double Time;
        public bool IsXW => Num._XW;
        public bool IsBigXW; // 当前是否处于大心无状态

        public string Genre => QiXue.Genre;
        public WPOption WP => Equip.WP;
        public int AbilityRank; // 手法等级，0~3
        public Dictionary<string, SkillEventItem> SkillEvents;
        public SkillFreqDict BasicSkillFreq; // 未考虑橙武改变循环前的技能频率
        public SkillFreqDict FinalSkillFreq; // 考虑橙武改变循环后的技能频率
        public Dictionary<string, double> BasicEventsHitFreq; // 在基础频率上，各个时间触发Hit数
        public Dictionary<string, double> FinalEventsHitFreq; // 在橙武改变循环后的频率上，各个时间触发Hit数

        public Dictionary<string, double> EventsHitFreq =>
            XFAppStatic.XinFaTag == "JY" ? FinalEventsHitFreq : BasicEventsHitFreq; // 用于计算非橙武触发内的时间Hit数，考虑到天罗橙武Hit太高，使用基础的而非最终的

        public double SLTypeHitFreq = 0; // 神力类触发的Hit频率（无论当前有没有套装效果均计算）
        public double BasicSLCover = 0; // 基础神力覆盖率（未考虑大橙武改变频率）
        public static SkillEventItem SLEventItem = StaticXFData.DB.SkillInfo.Events["SL"];
        public double FeiJianJueYiBuffCD = 0; // 飞剑绝意·锋 CD

        #endregion

        #region 计算
        public Dictionary<string, double> GetBasicEventHitFreq(IEnumerable<SkillEventItem> items)
        {
            var skillfreq = Num.ToSkillFreqDict();
            var res = skillfreq.GetEventsHitFreq(items);
            return res;
        }

        public void GetBasicSkillFreq()
        {
            // 计算基础技能频率
            BasicSkillFreq = Num.ToSkillFreqDict();
        }

        public void GetBasicEventHitFreq()
        {
            // 计算触发事件Hit频率
            BasicEventsHitFreq = BasicSkillFreq.GetEventsHitFreq(SkillEvents.Values);
            FinalEventsHitFreq = BasicEventsHitFreq;
        }

        public void GetFinalEventHitFreq()
        {
            // 计算触发事件Hit频率
            FinalEventsHitFreq = FinalSkillFreq.GetEventsHitFreq(SkillEvents.Values);
        }

        public void GetBasicSLCover()
        {
            // 计算基础神力覆盖率
            SLTypeHitFreq = BasicSkillFreq.GetEventHitFreq(SLEventItem); // 无论有没有神力效果，均计算
            const string key = "SL";
            if (SkillEvents.ContainsKey(key))
            {
                BasicSLCover = SkillEvents[key].BuffCoverRate(BasicEventsHitFreq[key]);
            }
        }

        public double GetFinalSLCover()
        {
            // 计算神力覆盖率
            SLTypeHitFreq = BasicSkillFreq.GetEventHitFreq(SLEventItem); // 无论有没有神力效果，均计算
            const string key = "SL";
            double res = 0.0;
            if (SkillEvents.ContainsKey(key))
            {
                res = SkillEvents[key].BuffCoverRate(FinalEventsHitFreq[key]);
            }
            return res;
        }

        public void GetBigFMFreq()
        {
            if (BigFM.Wrist != null) // 伤腕大附魔
            {
                // 计算大附魔次数
                var wirstKey = "BigFM_WRIST";
                var wristInterval = SkillEvents[wirstKey].MeanTriggerInterval(EventsHitFreq[wirstKey]);
                FinalSkillFreq.AddByInterval("KW_XR", wristInterval);
            }

            if (BigFM.Shoes != null && BigFM.Shoes.DLCLevel == 110) // 110级伤鞋大附魔
            {
                var shoesKey = "BigFM_SHOES_110";
                var shoesInterval = SkillEvents[shoesKey].MeanTriggerInterval(EventsHitFreq[shoesKey]);
                FinalSkillFreq.AddByInterval("KW_ZF", shoesInterval);
            }
        }

        public void GetLMJF()
        {
            // 计算龙门剑风频率
            if (WP.IsLongMen)
            {
                var level = WP.Value; // TODO
                var skillKey = $"LM_JF#{level:D}";
                var key = WP.Tag;
                var interval = SkillEvents[key].MeanTriggerInterval(BasicEventsHitFreq[key]);
                FinalSkillFreq.AddByInterval(skillKey, interval);
                FeiJianJueYiBuffCD = interval * 5;
            }
        }

        /// <summary>
        /// 计算 逐云寒蕊（飘黄） 频率，注意此频率依赖于罡风频率，请在CalcGF()后使用！
        /// </summary>
        public void GetPiaoHuangFreq()
        {
            if (Arg.PiaoHuangCover > 0)
            {
                const string piaohuang = "PiaoHuang";
                var ph = SkillEvents[piaohuang];

                var eHSP = IsBigXW ? XFStaticConst.XW.ExtraSP : 0;
                var realCD = XFStaticConst.CurrentHaste.SKT(ph.CD, Arg.HS, eHSP);
                var interval = ph.MeanTriggerInterval(hitfreq: EventsHitFreq[piaohuang], cd: realCD);
                var freq = 1 / interval;
                FinalSkillFreq.AddByFreq("ZhuYunHanRui", freq * Arg.PiaoHuangCover);
            }
        }

        #endregion


    }
}