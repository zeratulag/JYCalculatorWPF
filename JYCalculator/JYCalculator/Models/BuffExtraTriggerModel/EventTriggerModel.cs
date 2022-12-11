using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Class;
using JYCalculator.Class;
using JYCalculator.Src;
using JYCalculator.Src.Class;
using JYCalculator.Src.Data;

namespace JYCalculator.Models.BuffExtraTriggerModel
{
    /// <summary>
    /// 用于计算特殊触发BUFF的模型
    /// </summary>
    public class EventTriggerModel
    {
        #region 成员

        public readonly SkillNumModel SkillNum;
        public readonly FullCharacter InputChar; // 输入的属性，已经计算过所有其他BUFF

        public readonly ExtraTriggerArg Arg;

        public readonly Period<FullCharacter> BuffedFChars; // 存储计算了特殊BUFF，但是未考虑腰坠后的面板（腰坠的属性需要结合时间计算）
        public readonly Period<SkillFreqCTDF> SkillFreqCTDFs;
        public readonly Period<SkillDataDF> SkillDataDfs; // 技能信息
        public readonly BuffCoverDF BuffCover;

        public (double Normal, double XW) HuiChangInterval; // 回肠平均触发间隔

        public static readonly Buff SL = StaticJYData.DB.Buff.Buff_ExtraTrigger["ShenLi"];
        public static readonly Buff SY1 = StaticJYData.DB.Buff.Buff_ExtraTrigger["Shang·Yao_1"]; // 伤腰1%
        public static readonly Buff SY5 = StaticJYData.DB.Buff.Buff_ExtraTrigger["Shang·Yao_5"]; // 伤腰5%


        #endregion

        #region 构造

        public EventTriggerModel()
        {
            BuffCover = new BuffCoverDF();
        }

        public EventTriggerModel(SkillNumModel skill, FullCharacter fchar,
            Period<SkillDataDF> skillDatas,
            ExtraTriggerArg arg)
        {
            SkillNum = skill;
            InputChar = fchar;
            SkillDataDfs = skillDatas;
            Arg = arg;
            BuffedFChars = new Period<FullCharacter>
            {
                Normal = new FullCharacter(InputChar)
            };
            SkillFreqCTDFs = new Period<SkillFreqCTDF>();
            BuffCover = new BuffCoverDF();
        }

        #endregion

        public void Calc()
        {
            BuffCover.Reset();
            CalcSkillCTDF();
            CalcBigFMBelt();
            CalcSLBuff();

            BuffedFChars.Normal.Has_Special_Buff = true;
            BuffedFChars.XW.Has_Special_Buff = true;

            CalcBigFM_SHOES_120();

#if DEBUG
            CalcHuiChang(); // 计算回肠频率
#endif

        }

        // 计算常规和心无状态下的不同面板和技能频率;
        public void CalcSkillCTDF()
        {
            BuffedFChars.Normal = InputChar.Copy("+特殊BUFF");
            BuffedFChars.XW = BuffedFChars.Normal.ToBurst(Arg.BigXW);

            SkillFreqCTDFs.Normal =
                new SkillFreqCTDF(SkillDataDfs.Normal, BuffedFChars.Normal, SkillNum.Normal.FinalSkillFreq);
            SkillFreqCTDFs.XW = new SkillFreqCTDF(SkillDataDfs.XW, BuffedFChars.XW, SkillNum.XW.FinalSkillFreq);
        }

        // 计算伤腰大附魔
        protected void CalcBigFMBelt()
        {
            if (!Arg.SY) return;
            var key = "BigFM_BELT";
            var normalfreq = SkillNum.Normal.EventsHitFreq[key];
            var xwfreq = SkillNum.XW.EventsHitFreq[key];
            var normalSY = SkillNum.HitSkillEvents[key].CDBuffCoverRate(normalfreq);
            var xwSY = SkillNum.HitSkillEvents[key].CDBuffCoverRate(xwfreq);

            BuffCover.SetCover(key, normalSY, xwSY);

            ApplyBigFMBeltBuff(BuffedFChars.Normal, normalSY);
            ApplyBigFMBeltBuff(BuffedFChars.XW, xwSY);
        }

        /// <summary>
        /// 向人物属性上增加平均的伤腰大附魔BUFF
        /// </summary>
        /// <param name="fchar">人物属性</param>
        /// <param name="cover">伤腰覆盖率</param>
        public void ApplyBigFMBeltBuff(FullCharacter fchar, double cover)
        {
            var Belt1 = SY1.Emit(cover * 0.3, 1); // 30%几率提高1%
            var Belt5 = SY5.Emit(cover * 0.7, 1); // 有70%几率提高5%
            // 平均为 3.8%
            fchar.AddBaseBuff(Belt1);
            fchar.AddBaseBuff(Belt5);
        }


        // 计算神力BUFF
        public void CalcSLBuff()
        {
            if (!Arg.SL) return;
            var key = "SL";
            BuffCover.SetCover(key, SkillNum.Normal.SLCover, SkillNum.XW.SLCover);
            var normalSL = SL.Emit(cover: BuffCover[key].Normal, stack: 1);
            var XWSL = SL.Emit(cover: BuffCover[key].XW, stack: 1);

            BuffedFChars.Normal.AddBaseBuff(normalSL);
            BuffedFChars.XW.AddBaseBuff(XWSL);

            SkillFreqCTDFs.Normal.UpdateFChar();
            SkillFreqCTDFs.XW.UpdateFChar();
        }



        /// <summary>
        /// 计算120级伤鞋大附魔频率
        /// </summary>
        public void CalcBigFM_SHOES_120()
        {
            if (!Arg.BigFM_SHOES_120) return;
            var shoesEvent = SkillNum.CTSkillEvents["BigFM_SHOES_120"];
            var names = shoesEvent.TriggerSkillNames;

            var normalHitfreq = SkillFreqCTDFs.Normal.GetSumCTFreq(names);
            var normalInterval = shoesEvent.MeanTriggerInterval(normalHitfreq);
            const string renling = "RenLing";
            SkillFreqCTDFs.Normal.AddSkillFreq(renling, 1 / normalInterval);

            var xwHitfreq = SkillFreqCTDFs.XW.GetSumCTFreq(names);
            var xwInterval = shoesEvent.MeanTriggerInterval(xwHitfreq);
            SkillFreqCTDFs.XW.AddSkillFreq(renling, 1 / xwInterval);
        }



        /// <summary>
        /// 计算回肠触发间隔
        /// </summary>
        protected void CalcHuiChang()
        {
            var key = "HuiChangDangQi";
            if (SkillNum.CTSkillEvents.ContainsKey(key))
            {
                var huichangEvent = SkillNum.CTSkillEvents[key];
                var names = huichangEvent.TriggerSkillNames;

                var normalHitfreq = SkillFreqCTDFs.Normal.GetSumCTFreq(names);
                var normalInterval = huichangEvent.MeanTriggerInterval(normalHitfreq);

                var xwHitfreq = SkillFreqCTDFs.XW.GetSumCTFreq(names);
                var xwInterval = huichangEvent.MeanTriggerInterval(xwHitfreq);

                HuiChangInterval = (normalInterval, xwInterval);
                double HuiChangEnergySpeed = 25.0 / normalInterval; // 回肠每秒回复效率
            }

        }
    }


    public readonly struct ExtraTriggerArg
    {
        public readonly bool SL; // 是否有神力
        public readonly bool SY; // 是否有伤腰大附魔

        public readonly bool BigFM_SHOES_120; // 是否有120级的伤鞋大附魔
        public readonly bool BigXW; // 是否为大心无
        public readonly double PiaoHuangCover; // 飘黄Buff覆盖率


        public ExtraTriggerArg(bool sl, bool sy, bool bigFM_SHOES_120, bool bigXW, double piaoHuangCover)
        {
            SL = sl;
            SY = sy;
            BigFM_SHOES_120 = bigFM_SHOES_120;
            BigXW = bigXW;
            PiaoHuangCover = piaoHuangCover;
        }
    }
}