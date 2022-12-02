using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JX3CalculatorShared.Src;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using JYCalculator.Class;
using JYCalculator.Src;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;

namespace JYCalculator.Models
{
    // 分时段模型
    public class CommonSkillNumModel
    {
        #region 成员

        public readonly QiXueConfigModel QiXue;
        public readonly SkillHasteTable SkillHaste;

        public readonly EquipOptionConfigModel Equip;
        public readonly BigFMConfigModel BigFM;
        public readonly bool HasZhen;

        public double Time;
        public readonly JYSkillCountItem Num; // 技能对象

        public bool IsXW => Num._XW;
        public bool IsBigXW; // 当前是否处于大心无状态
        public WPOption WP => Equip.WP;

        public int AbilityRank; // 手法等级，0~3


        public Dictionary<string, SkillEventItem> SkillEvents;

        public SkillFreqDict BasicSkillFreq; // 未考虑橙武改变循环前的技能频率
        public SkillFreqDict FinalSkillFreq; // 考虑橙武改变循环后的技能频率

        public Dictionary<string, double> EventsHitFreq; // 在基础频率上，各个时间触发Hit数

        public double SLTypeHitFreq = 0; // 神力类触发的Hit频率（无论当前有没有套装效果均计算）
        public double SLCover = 0; // 神力覆盖率
        public SkillEventItem SLEventItem = StaticJYData.DB.SkillInfo.Events["SL"];

        public double CW_DOTPerCW = 30; // 单次橙武特效造成的伤害次数（常规10跳x3层，顶级可以多1跳2层）
        public double CW_DOTHitPerCW = 10; // 单次橙武特效在伤害统计记录的次数（顶级11）

        #endregion

        #region 构造

        public CommonSkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste, AbilitySkillNumItem abilityitem,
            EquipOptionConfigModel equip, BigFMConfigModel bigfm,
            bool hasZhen)
        {
            QiXue = qixue;
            SkillHaste = skillhaste;
            Equip = equip;
            BigFM = bigfm;
            HasZhen = hasZhen;
            Num = new JYSkillCountItem(abilityitem, qixue.BYPerCast);

            AbilityRank = abilityitem.Rank;

            Time = IsXW ? QiXue.XWDuration : QiXue.NormalDuration;
            Num.ResetTime(Time);

            IsBigXW = IsXW && QiXue.聚精凝神;

            CW_DOTPerCW = 10 * 3.0 - 1 + AbilityRank;
            CW_DOTHitPerCW = 10 + AbilityRank / 3.0;
        }

        #endregion

        #region 计算

        public void CommonCalc()
        {
            CalcCX_DOT();
            CalcZX_DOT();
        }


        // 计算穿心跳数
        public void CalcCX_DOT()
        {
            var cx = SkillHaste.CX_DOT;
            double interval;
            if (IsBigXW)
            {
                interval = cx.XWIntervalTime;
            }
            else
            {
                interval = cx.IntervalTime;
            }

            Num._CX_DOT_Hit = Num._Time / interval;
            var totalNum = Num._CX_DOT_Hit * QiXue.CX_DOT_Stack;

            if (QiXue.鹰扬虎视)
            {
                Num.CXY_DOT = totalNum;
            }
            else
            {
                Num.CX_DOT = totalNum;
            }
        }


        // 计算逐星DOT
        public void CalcZX_DOT()
        {
            if (QiXue.星落如雨)
            {
                var cx = SkillHaste.ZX_DOT;
                double interval;
                if (IsBigXW)
                {
                    interval = cx.XWIntervalTime;
                }
                else
                {
                    interval = cx.IntervalTime;
                }

                Num.ZX_DOT = Math.Min(Num.ZX * 3, Num._Time / interval);
            }
        }

        /// <summary>
        /// 计算百里数
        /// </summary>
        public void CalcBL()
        {
            var bl = GetBLFreqTime();

            Num.ApplyBLFreq(bl.Freq, bl.Time);
        }

        /// <summary>
        /// 计算百里频率和CD时间
        /// </summary>
        /// <returns></returns>
        public (double Freq, double Time) GetBLFreqTime()
        {
            var bldata = SkillHaste.SkillDF.Data["BL"];
            var blcd = bldata.CD;
            var rawBLFreq = 1 / blcd;
            var abilityrankcoef = (0.85 + 0.05 * AbilityRank); // 顶级手法下无延迟，应该为1
            var realBLFreq = rawBLFreq * abilityrankcoef;

            double bltime;
            if (IsBigXW)
            {
                bltime = SkillHaste.BL.XWTime;
            }
            else
            {
                bltime = SkillHaste.BL.Time;
            }

            return (realBLFreq, bltime);
        }


        public void CalcGF()
        {
            Num.CalcGF();
        }

        public Dictionary<string, double> GetEventHitFreq(IEnumerable<SkillEventItem> items)
        {
            var skillfreq = Num.ToSkillFreqDict();
            var res = skillfreq.GetEventsHitFreq(items);
            return res;
        }

        /// <summary>
        /// 计算触发事件
        /// </summary>
        public void CalcEvents()
        {
            GetBasicSkillFreq();
            GetEventHitFreq();
            FinalSkillFreq = new SkillFreqDict(BasicSkillFreq);
            GetBigCWFreq(); // 惊羽需要先计算橙武频率，再计算其他频率
            GetSLCover();
            GetPZFreq();
            GetBigFMFreq();
        }

        // 计算基础技能频率
        public void GetBasicSkillFreq()
        {
            BasicSkillFreq = Num.ToSkillFreqDict();
        }

        // 计算触发事件Hit频率
        public void GetEventHitFreq()
        {
            EventsHitFreq = BasicSkillFreq.GetEventsHitFreq(SkillEvents.Values);
        }

        // 计算神力覆盖率
        public void GetSLCover()
        {
            SLTypeHitFreq = BasicSkillFreq.GetEventHitFreq(SLEventItem); // 无论有没有神力效果，均计算
            if (SkillEvents.ContainsKey("SL"))
            {
                SLCover = SkillEvents["SL"].BuffCoverRate(EventsHitFreq["SL"]);
            }
        }

        // 计算大附魔次数
        public void GetBigFMFreq()
        {
            if (BigFM.Wrist != null) // 伤腕大附魔
            {
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


        // 计算破招频率
        public void GetPZFreq()
        {
            FinalSkillFreq.AddByFreq("PZ_ZM", FinalSkillFreq["ZM_SF"]); // 瞬发追命带破招

            if (IsXW)
            {
                FinalSkillFreq.AddByFreq("PZ_BY", FinalSkillFreq["LH7"]); // 心无期间完整施展暴雨带破招
            }
        }

        // 计算橙武技能频率
        public void GetBigCWFreq()
        {
            if (!WP.IsBigCW) return;
            var key = "CW_ZX";
            var CWInterval = SkillEvents[key].MeanTriggerInterval(EventsHitFreq[key]);
            double CWTime = SkillEvents[key].Time; // 橙武特效持续时间，7秒

            var ZX_Origin_Num = FinalSkillFreq["ZX"] * CWTime;
            var ZX_Target_Num = 3.0; // 橙武特效期间需要打3逐星。
            var ZX_Time = IsBigXW ? SkillHaste.GCD.XWTime : SkillHaste.GCD.Time; // 施展逐星所需时间;

            var Total_Interval_Time = CWInterval;

            if (ZX_Origin_Num < ZX_Target_Num)
            {
                var ZX_Extra_Num = ZX_Target_Num - ZX_Origin_Num; // 需要额外打的逐星数量;
                var ZX_Extra_Time = ZX_Time * ZX_Extra_Num;

                Total_Interval_Time += ZX_Extra_Time; // 由于多打逐星，循环被拉长后的总时间
                var coef = CWInterval / Total_Interval_Time; // 损失系数, ~ 0.94
                FinalSkillFreq.AppplyFreqChange(coef);
                var new_GF = FinalSkillFreq.RefreshJYGFFreq();
            }

            FinalSkillFreq.AddByFreq("_CW_DOT_Hit", CW_DOTHitPerCW / Total_Interval_Time);
            FinalSkillFreq.AddByFreq("CW_DOT", CW_DOTPerCW / Total_Interval_Time);

            double CW_DP;
            var CW_DPkey = nameof(CW_DP);
            CW_DP = SkillEvents[CW_DPkey].TriggerFreq(EventsHitFreq[CW_DPkey]);
            FinalSkillFreq.AddByFreq(CW_DPkey, CW_DP);

        }




        #endregion
    }
}