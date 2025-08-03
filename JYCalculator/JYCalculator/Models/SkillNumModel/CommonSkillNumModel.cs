using JX3CalculatorShared.Models;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Src;
using System;
using JX3CalculatorShared.Class;
using JYCalculator.Class.SkillCount;


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
        public double PZ_BYPerZX;

        public readonly bool 白雨流_万灵当歌 = false;
        public readonly bool 追夺流_雾海寻龙 = false;
        public readonly bool 百步凝形_丝路风语 = false;
        public readonly bool 百步凝形_丝路风语_橙武 = false;

        public bool CXMaintainThroughout => !百步凝形_丝路风语; // 穿心是否能全程保持

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

            白雨流_万灵当歌 = abilityitem.GenreEnum == GenreTypeEnum.白雨流_万灵当歌;
            追夺流_雾海寻龙 = abilityitem.GenreEnum == GenreTypeEnum.追夺流_雾海寻龙;
            百步凝形_丝路风语_橙武 = abilityitem.GenreEnum == GenreTypeEnum.百步凝形_丝路风语_橙武;
            百步凝形_丝路风语 = abilityitem.GenreEnum == GenreTypeEnum.百步凝形_丝路风语 || 百步凝形_丝路风语_橙武;

            switch (abilityitem.GenreEnum)
            {
                case GenreTypeEnum.白雨流_万灵当歌:
                {
                    Num = new BaoYuSkillCountItem(abilityitem, qixue);
                    break;
                }
                case GenreTypeEnum.追夺流_雾海寻龙:
                {
                    Num = new DuoZhuiSkillCountItem(abilityitem, qixue);
                    break;
                }
                case GenreTypeEnum.百步凝形_丝路风语:
                case GenreTypeEnum.百步凝形_丝路风语_橙武:
                {
                    Num = new BaiBuNingXingSkillCountItem(abilityitem, qixue);
                    var baseSkillRatioGroups = StaticXFData.Data.DefaultBaseSkillRatioGroups;
                    if (百步凝形_丝路风语_橙武)
                    {
                        baseSkillRatioGroups = StaticXFData.Data.ChengWuBaseSkillRatioGroups;
                    }

                    SkillRatioGroup = IsXW ? baseSkillRatioGroups.XinWu : baseSkillRatioGroups.Normal;
                    break;
                }
            }

            Arg = arg;

            AbilityRank = abilityitem.Rank;

            Time = IsXW ? QiXue.XWDuration : QiXue.NormalDuration;
            PZ_BYPerZX = IsXW ? 2.0 : XFStaticConst.PZ_BaiYuPer_Normal_ZX; // 单次逐星的平均白雨破招数
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
            DoBeforeWork();
            CalcCX_DOT();
            CalcZX_DOT();
        }

        // 夺追流需要的准备
        public void DoBeforeWork()
        {
            if (追夺流_雾海寻龙)
            {
                var realNum = Num as DuoZhuiSkillCountItem;
                realNum.IsBigXW = IsBigXW;
                realNum?.DoPreWork();
            }

            if (百步凝形_丝路风语)
            {
                var realNum = Num as BaiBuNingXingSkillCountItem;
                if (QiXue.百步穿杨)
                {
                    realNum.AttachBaiBuChuanYang(Arg.TargetAllWaysFullHP);
                }

                realNum?.DoPreWork();
            }
        }


        public void CommonCalcAfter()
        {
            // 在其他技能数已经确定时的通用计算
            CalcGangFengFinal();
            CalcNieJingZhuiMingFreq();
            CalcLveYingQiongCangFreq();
            CalcNingXingZhuimingFreq();
        }

        // 分配凝形追命的各种技能数
        private void CalcNingXingZhuimingFreq()
        {
            if (!百步凝形_丝路风语) return;
            SkillRatioGroup.AllocateSkillNum(FinalSkillFreq);
        }

        // 计算穿心跳数
        public void CalcCX_DOT()
        {
            if (!CXMaintainThroughout)
            {
                Num.SetCXDotByCXDotCount(Num.CXDotCount);
                return;
            }

            var cx = QiXue.穿林打叶 ? SkillHaste.CX2_CL_DOT : SkillHaste.CX3_DOT;

            double interval;
            if (IsBigXW)
            {
                interval = cx.XWIntervalTime;
            }
            else
            {
                interval = cx.IntervalTime;
            }

            Num.SetCXDOTByHit(Num._Time / interval);
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
            if (白雨流_万灵当歌)
            {
                var bl = GetBLFreqTime();
                var realNum = Num as BaoYuSkillCountItem;
                realNum?.ApplyBLFreq(bl.Freq, bl.Time);
            }
        }

        /// <summary>
        /// 计算百里频率和CD时间
        /// </summary>
        /// <returns></returns>
        public (double Freq, double Time) GetBLFreqTime()
        {
            double raw_blcd = StaticXFData.DB.BaseSkillInfo.Skills[SkillKeyConst.百里追魂].CD; // 原始CD
            double real_cd = raw_blcd;

            if (QiXue.寒江夜雨)
            {
                var efreq = Num.GetEnergyInjection();
                var hjfreq = efreq / 3.0 + Num.ZX / Num._Time;
                if (QiXue.白雨跳珠)
                {
                    hjfreq += Num.ZX * PZ_BYPerZX / Num._Time;
                }

                real_cd = QiXue.GetBLCDByHanJiangFreq(hjfreq);
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

        public void CalcGangFengFinal()
        {
            FinalSkillFreq.CalcGangFeng();
            if (QiXue.蹑景追风)
            {
                FinalSkillFreq.FixGangFengOnZhuiDuo();
            }
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
            GetBigCWDPFreq();
            GetFinalEventHitFreq();
            GetBasicSLCover();
            GetBigFMFreq();
            GetPiaoHuangFreq();
            GetLMJF();
        }

        public void GetBigCWDPFreq()
        {
            if (!WP.IsBigCW) return;
            double CW_DP;
            var CW_DPkey = nameof(CW_DP);
            CW_DP = SkillEvents[CW_DPkey].TriggerFreq(FinalSkillFreq[SkillKeyConst._夺魄箭_释放]);
            FinalSkillFreq.AddByFreq(CW_DPkey, CW_DP);
        }

        public void GetBigCWFreq()
        {
            // 计算橙武技能频率
            if (!WP.IsBigCW) return;
            if (百步凝形_丝路风语) return; // 暂时不支持

            const string key = "CW_ZX";
            var CWInterval = SkillEvents[key].MeanTriggerInterval(BasicEventsHitFreq[key]);
            double CWTime = SkillEvents[key].Time; // 橙武特效持续时间，7秒

            var ZX_Origin_Num = FinalSkillFreq[SkillKeyConst.逐星箭] * CWTime;
            var ZX_Target_Num = 3.0; // 橙武特效期间需要打3逐星。
            var ZX_Time = IsBigXW ? SkillHaste.GCD.XWTime : SkillHaste.GCD.Time; // 施展逐星所需时间;

            double Total_Interval_Time = CWInterval;

            FinalSkillFreq.Data.TryGetValue(SkillKeyConst.穿心弩, out var oldCXL);

            if (ZX_Origin_Num < ZX_Target_Num)
            {
                var ZX_Extra_Num = ZX_Target_Num - ZX_Origin_Num; // 需要额外打的逐星数量;
                var ZX_Extra_Time = ZX_Time * ZX_Extra_Num;

                Total_Interval_Time = CWInterval + ZX_Extra_Time; // 由于多打逐星，循环被拉长后的总时间
                var coef = CWInterval / Total_Interval_Time; // 损失系数, ~ 0.94
                //var coef = (CWInterval - ZX_Extra_Time) / Total_Interval_Time; // 损失系数, ~ 0.94
                FinalSkillFreq.AppplyFreqChange(coef);
                FinalSkillFreq.RefreshJYGFFreq();
                FinalSkillFreq.AddSkillFreq(SkillKeyConst.逐星箭, ZX_Extra_Num / Total_Interval_Time); // 增加逐星频率

                if (QiXue.星落如雨)
                {
                    FinalSkillFreq[SkillKeyConst.星斗阑干] += 3 / Total_Interval_Time; // 逐星DOT本身也增加
                }

                if (QiXue.穿林打叶)
                {
                    FinalSkillFreq[SkillKeyConst.穿林打叶] = FinalSkillFreq[SkillKeyConst.逐星箭];
                }
            }

            FinalSkillFreq.AddByFreq("_CW_DOT_Hit", CW_DOTHitPerCW / Total_Interval_Time);
            FinalSkillFreq.AddByFreq("CW_DOT", CW_DOTPerCW / Total_Interval_Time);

            GetBigCWBLFreq(); // 修正橙武百里数

            FinalSkillFreq.Data.TryGetValue(SkillKeyConst.穿心弩, out var newCXL);
            var deltaCXL = oldCXL - newCXL;
            FinalSkillFreq.MoveDPToCXL(deltaCXL); // 穿心弩的频率不能减少

        }


        public void GetBigCWBLFreq()
        {
            // 修正大橙武百里数
            if (!QiXue.寒江夜雨) return;

            var freq = FinalSkillFreq.GetJYHanJiangFreq();
            var rawBLFreq = 1 / QiXue.GetBLCDByHanJiangFreq(freq);
            var abilityrankcoef = (0.85 + 0.05 * AbilityRank); // 顶级手法下无延迟，应该为1
            var realBLFreq = rawBLFreq * abilityrankcoef;
            var bltime = IsXW ? SkillHaste.BL.XWTime : SkillHaste.BL.Time;
            FinalSkillFreq.ResetBLFreq(realBLFreq, bltime, FinalNum);
        }


        /// <summary>
        /// 分配蹑景追命技能数
        /// </summary>
        public void CalcNieJingZhuiMingFreq()
        {
            if (!QiXue.蹑景追风) return;
            if (百步凝形_丝路风语) return;
            FinalSkillFreq.CalcNieJingZhuiMingFreq();
        }

        public void CalcLveYingQiongCangFreq()
        {
            if (!QiXue.掠影穹苍) return;
            if (百步凝形_丝路风语) return;
            FinalSkillFreq.CalcLveYingQiongCangFreq();
        }

        #endregion
    }
}