using System;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using System.Collections.Generic;
using JX3CalculatorShared.Globals;
using JYCalculator.Src.Class;
using Syncfusion.UI.Xaml.Grid;

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
        public readonly BaseSkillRatioGroup SkillRatioGroup = null; // 凝形的技能数分配比例
        public double Time;
        public bool IsXW => Num._XW;
        public bool IsBigXW; // 当前是否处于大心无状态

        public WPOption WP => Equip.WP;
        public int AbilityRank; // 手法等级，0~3
        public Dictionary<string, SkillEventItem> SkillEvents;
        public SkillFreqDict BasicSkillFreq; // 未考虑橙武改变循环前的技能频率
        public SkillFreqDict FinalSkillFreq; // 考虑橙武改变循环后的技能频率
        public Dictionary<string, double> BasicEventsHitFreq; // 在基础频率上，各个时间触发Hit数
        public Dictionary<string, double> FinalEventsHitFreq; // 在橙武改变循环后的频率上，各个时间触发Hit数

        public Dictionary<string, double> EventsHitFreq =>
            XFAppStatic.XinFaTag == "JY"
                ? FinalEventsHitFreq
                : BasicEventsHitFreq; // 用于计算非橙武触发内的时间Hit数，考虑到天罗橙武Hit太高，使用基础的而非最终的

        public double SLTypeHitFreq = 0; // 神力类触发的Hit频率（无论当前有没有套装效果均计算）
        public double BasicSLCover = 0; // 基础神力覆盖率（未考虑大橙武改变频率）
        public static SkillEventItem SLEventItem = StaticXFData.DB.BaseSkillInfo.Events["SL"];
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

        /// <summary>
        /// 计算伤腕大附魔
        /// </summary>
        public void CalcBigFM_WRIST()
        {
            if (BigFM.Wrist == null) return; // 伤腕大附魔
            var wristKey = "BigFM_WRIST";
            if (BigFM.Wrist.ExpansionPackLevel == 130)
            {
                wristKey = "BigFM_WRIST_130";
            }
            var wristEvent = SkillEvents[wristKey];
            double wristInterval = 0.0;
            // 计算大附魔次数
            //var wristInterval = SkillEvents[wristKey].MeanTriggerInterval(EventsHitFreq[wristKey]);
            wristInterval = BasicSkillFreq.CalcMeanTriggerInterval(wristEvent);

            var skillKey = "KW_XR";
            if (BigFM.Wrist.IsSuperCustom)
            {
                skillKey = $"{skillKey}_Fixed"; // 120级最后一个版本的大附魔改成固定伤害
            }

            if (BigFM.Wrist.ExpansionPackLevel >= 130) // 130 级大附魔key
            {
                skillKey = $"{skillKey}_{BigFM.Wrist.SkillNameSuffix}";
            }

            AddSkillByIntervalToFinalSkillFreq(skillKey, wristInterval);
        }

        /// <summary>
        /// 基于释放间隔添加技能
        /// </summary>
        /// <param name="key">技能名</param>
        /// <param name="interval">技能释放间隔</param>
        public void AddSkillByIntervalToFinalSkillFreq(string key, double interval)
        {
            FinalSkillFreq.AddByInterval(key, interval);
        }

        public void CalcBigFM_SHOES()
        {
            if (BigFM.Shoes != null && BigFM.Shoes.ExpansionPackLevel == 110) // 110级伤鞋大附魔
            {
                var shoesKey = "BigFM_SHOES_110";
                var shoesInterval = SkillEvents[shoesKey].MeanTriggerInterval(EventsHitFreq[shoesKey]);
                var skillKey = "KW_ZF";
                AddSkillByIntervalToFinalSkillFreq(skillKey, shoesInterval);
            }
        }


        public void GetBigFMFreq()
        {
            CalcBigFM_WRIST();
            CalcBigFM_SHOES();
        }

        public void GetLMJF()
        {
            // 计算龙门剑风频率
            if (WP.IsLongMen)
            {
                var level = WP.Value; // TODO
                var skillKey = $"LM_JF#{level:D}";
                var key = WP.Tag;
                var skillEvent = SkillEvents[key];
                double interval = 0.0;
                //interval = SkillEvents[key].MeanTriggerInterval(BasicEventsHitFreq[key]);
                interval = BasicSkillFreq.CalcMeanTriggerInterval(skillEvent);
                AddSkillByIntervalToFinalSkillFreq(skillKey, interval);
                FeiJianJueYiBuffCD = interval * 5;
            }
        }

        /// <summary>
        /// 计算 逐云寒蕊（飘黄） 频率，注意此频率依赖于罡风频率，请在CalcGF()后使用！
        /// </summary>
        public void GetPiaoHuangFreq()
        {
            if (Arg.PiaoHuangCover == 0)
            {
                return;
            }

            GetPiaoHuangFreq120();
            GetPiaoHuangFreq130();
        }

        // 120级飘黄逐云寒蕊算法
        public void GetPiaoHuangFreq120()
        {
            if (StaticConst.CurrentLevel == 120)
            {
                const string piaohuang = "PiaoHuang";
                var ph = SkillEvents[piaohuang];

                var eHSP = IsBigXW ? XFStaticConst.XinWuConsts.ExtraHaste : 0;
                var realCD = XFStaticConst.CurrentHaste.CalcHasteTime(ph.CD, Arg.HS, eHSP);
                var interval = ph.MeanTriggerInterval(hitfreq: EventsHitFreq[piaohuang], cd: realCD);
                var freq = 1 / interval;
                FinalSkillFreq.AddByFreq("ZhuYunHanRui", freq * Arg.PiaoHuangCover);
            }
        }

        // 130级飘黄逐云寒蕊算法
        public void GetPiaoHuangFreq130()
        {
            if (StaticConst.CurrentLevel == 130)
            {
                const string piaohuang = "PiaoHuang";
                var ph = SkillEvents[piaohuang];
                var eHSP = IsBigXW ? XFStaticConst.XinWuConsts.ExtraHaste : 0;
                var realCD = XFStaticConst.CurrentHaste.CalcHasteTime(ph.CD, Arg.HS, eHSP);
                var interval = ph.MeanTriggerInterval(hitfreq: EventsHitFreq[piaohuang], cd: realCD);
                var freq = 1 / interval;
                const double ZhuRuiCD = 60.0;
                const double ZhuiRuiBuffCd = 30.0;
                var zhuRuiCastPerCD = Arg.PiaoHuangCover * ZhuRuiCD / 20; // 药奶每分钟释放了多少次逐云寒蕊
                zhuRuiCastPerCD = Math.Min(zhuRuiCastPerCD, ZhuRuiCD / ZhuiRuiBuffCd); // 逐云寒蕊每分钟最多触发2次，因为30秒内置cd
                var maxFreq = zhuRuiCastPerCD * 6 / ZhuRuiCD; // 每次释放逐蕊最多造成6次伤害
                var finalFreq = Math.Min(maxFreq, freq * Arg.PiaoHuangCover);
                FinalSkillFreq.AddByFreq(SkillKeyConst.逐云寒蕊_130, finalFreq);
            }
        }

        #endregion
    }
}