using JX3CalculatorShared.Class;
using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;
using System;
using Force.DeepCloner;


namespace JYCalculator.Models
{
    // 分时段模型
    public partial class CommonSkillNumModel
    {
        #region 成员

        public readonly JYSkillCountItem Num; // 技能对象
        public JYSkillCountItem FinalNum; // 最终技能数对象


        public double CW_DOTPerCW = 30; // 单次橙武特效造成的伤害次数（常规10跳x3层，顶级可以多1跳2层）
        public double CW_DOTHitPerCW = 10; // 单次橙武特效在伤害统计记录的次数（顶级11）


        #endregion

        #region 构造

        public CommonSkillNumModel(QiXueConfigModel qixue, SkillHasteTable skillhaste, AbilitySkillNumItem abilityitem,
            EquipOptionConfigModel equip, BigFMConfigModel bigfm,
            SkillNumModelArg arg)
        {
            QiXue = qixue;
            SkillHaste = skillhaste;
            Equip = equip;
            BigFM = bigfm;
            Num = new JYSkillCountItem(abilityitem, qixue.BYPerCast);
            Arg = arg;

            AbilityRank = abilityitem.Rank;

            Time = IsXW ? QiXue.XWDuration : QiXue.NormalDuration;
            Num.ResetTime(Time);

            IsBigXW = IsXW && QiXue.聚精凝神;

            XFPostConstructor();
        }


        public void XFPostConstructor()
        {
            CW_DOTPerCW = 10 * 3.0 - 1 + AbilityRank;
            CW_DOTHitPerCW = 10 + AbilityRank / 3.0;
        }


        #endregion

        #region 计算

        public void CommonCalcBefore()
        {
            CalcCX_DOT();
            CalcZX_DOT();
        }

        public void CommonCalcAfter()
        {
            // 在其他技能数已经确定时的通用计算
            CalcGFFinal();
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
                var zx = SkillHaste.ZX_DOT;
                double interval;
                if (IsBigXW)
                {
                    interval = zx.XWIntervalTime;
                }
                else
                {
                    interval = zx.IntervalTime;
                }

                Num.ZX_DOT = Math.Min(Num.ZX * 6, Num._Time / interval);
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
            double raw_blcd = StaticXFData.DB.SkillInfo.Skills["BL"].CD; // 原始CD
            double real_cd = raw_blcd;

            if (QiXue.寒江夜雨)
            {
                var efreq = Num.GetEnergyInjection();
                real_cd = QiXue.GetBLCDByEnergyInjectionFreq(efreq * 0.986);
            }

            var rawBLFreq = 1 / real_cd;
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


        /*public void CalcGF()
        {
            Num.CalcGF();
        }*/

        public void CalcGFFinal()
        {
            FinalSkillFreq.CalcGF();
        }


        /// <summary>
        /// 计算触发事件
        /// </summary>
        public void CalcEvents()
        {
            GetBasicSkillFreq();
            GetBasicEventHitFreq();
            FinalNum = Num; // 惊羽可以直接赋值
            FinalSkillFreq = FinalNum.ToSkillFreqDict();
            GetBigCWFreq(); // 惊羽需要先计算橙武频率，再计算其他频率
            GetBasicSLCover();
            GetBigFMFreq();
            GetPiaoHuangFreq();
            GetLMJF();
        }

        public void GetBigCWFreq()
        {
            // 计算橙武技能频率
            if (!WP.IsBigCW) return;
            
            const string key = "CW_ZX";
            var CWInterval = SkillEvents[key].MeanTriggerInterval(BasicEventsHitFreq[key]);
            double CWTime = SkillEvents[key].Time; // 橙武特效持续时间，7秒

            var ZX_Origin_Num = FinalSkillFreq["ZX"] * CWTime;
            var ZX_Target_Num = 3.0; // 橙武特效期间需要打3逐星。
            var ZX_Time = IsBigXW ? SkillHaste.GCD.XWTime : SkillHaste.GCD.Time; // 施展逐星所需时间;

            double Total_Interval_Time = CWInterval;

            FinalSkillFreq.Data.TryGetValue("CXL", out var oldCXL);

            if (ZX_Origin_Num < ZX_Target_Num)
            {
                var ZX_Extra_Num = ZX_Target_Num - ZX_Origin_Num; // 需要额外打的逐星数量;
                var ZX_Extra_Time = ZX_Time * ZX_Extra_Num;

                Total_Interval_Time = CWInterval + ZX_Extra_Time; // 由于多打逐星，循环被拉长后的总时间
                var coef = CWInterval / Total_Interval_Time; // 损失系数, ~ 0.94
                //var coef = (CWInterval - ZX_Extra_Time) / Total_Interval_Time; // 损失系数, ~ 0.94
                FinalSkillFreq.AppplyFreqChange(coef);
                FinalSkillFreq.RefreshJYGFFreq();
                FinalSkillFreq.AddSkillFreq("ZX", ZX_Extra_Num / Total_Interval_Time); // 增加逐星频率

                if (QiXue.星落如雨)
                {
                    FinalSkillFreq["ZX_DOT"] += 3 / Total_Interval_Time; // 逐星DOT本身也增加
                }
            }

            FinalSkillFreq.AddByFreq("_CW_DOT_Hit", CW_DOTHitPerCW / Total_Interval_Time);
            FinalSkillFreq.AddByFreq("CW_DOT", CW_DOTPerCW / Total_Interval_Time);

            double CW_DP;
            var CW_DPkey = nameof(CW_DP);
            CW_DP = SkillEvents[CW_DPkey].TriggerFreq(FinalSkillFreq["DP"]);
            FinalSkillFreq.AddByFreq(CW_DPkey, CW_DP);

            GetBigCWBLFreq(); // 修正橙武百里数

            FinalSkillFreq.Data.TryGetValue("CXL", out var newCXL);
            var deltaCXL = oldCXL - newCXL;
            FinalSkillFreq.MoveDPToCXL(deltaCXL); // 穿心弩的频率不能减少

            GetFinalEventHitFreq();
        }


        public void GetBigCWBLFreq()
        {
            // 修正大橙武百里数
            if (!QiXue.寒江夜雨) return;

            var freq = FinalSkillFreq.CalcJYEnergyInjection();
            var rawBLFreq = 1 / QiXue.GetBLCDByEnergyInjectionFreq(freq);
            var abilityrankcoef = (0.85 + 0.05 * AbilityRank); // 顶级手法下无延迟，应该为1
            var realBLFreq = rawBLFreq * abilityrankcoef;
            var bltime = IsXW ? SkillHaste.BL.XWTime : SkillHaste.BL.Time;
            FinalSkillFreq.ResetBLFreq(realBLFreq, bltime, FinalNum);
        }

        #endregion
    }
}