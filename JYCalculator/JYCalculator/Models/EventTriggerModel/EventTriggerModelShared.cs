using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Class;
using JYCalculator.Data;
using JYCalculator.Globals;

namespace JYCalculator.Models
{
    public partial class EventTriggerModel
    {
        #region 成员

        public readonly SkillNumModel SkillNum;
        public readonly FullCharacter InputChar; // 输入的属性，已经计算过所有其他BUFF
        public readonly EventTriggerArg Arg;
        public readonly Period<FullCharacter> BuffedFChars; // 存储计算了特殊BUFF，但是未考虑腰坠后的面板（腰坠的属性需要结合时间计算）
        public readonly Period<SkillFreqCTDF> SkillFreqCTDFs;
        public readonly Period<SkillDataDF> SkillDfs; // 技能信息
        public readonly BuffCoverDF BuffCover;
        public (double Normal, double XW) HuiChangInterval; // 回肠平均触发间隔

        public static readonly Buff SL = StaticXFData.GetExtraTriggerBuff("ShenLi");
        public static readonly Buff SY1 = StaticXFData.GetExtraTriggerBuff("Shang·Yao_1"); // 伤腰1%
        public static readonly Buff SY5 = StaticXFData.GetExtraTriggerBuff("Shang·Yao_5"); // 伤腰5%

        #endregion

        #region 构造

        public EventTriggerModel(SkillNumModel skill, FullCharacter fChar,
            Period<SkillDataDF> skillDfs, BuffCoverDF buffCover,
            EventTriggerArg arg)
        {
            SkillNum = skill;
            InputChar = fChar;
            SkillDfs = skillDfs;
            Arg = arg;
            BuffedFChars = new Period<FullCharacter>
            {
                Normal = new FullCharacter(InputChar)
            };
            SkillFreqCTDFs = new Period<SkillFreqCTDF>();
            BuffCover = buffCover;
        }

        #endregion

        public void CalcSkillCTDF()
        {
            // 计算常规和心无状态下的不同面板和技能频率;
            BuffedFChars.Normal = InputChar.Copy("+特殊BUFF");
            BuffedFChars.XW = BuffedFChars.Normal.ToBurst(Arg.BigXW);

            SkillFreqCTDFs.Normal =
                new SkillFreqCTDF(SkillDfs.Normal, BuffedFChars.Normal, SkillNum.Normal.FinalSkillFreq);
            SkillFreqCTDFs.XW = new SkillFreqCTDF(SkillDfs.XW, BuffedFChars.XW, SkillNum.XW.FinalSkillFreq);
        }

        protected void CalcBigFMBelt()
        {
            // 计算伤腰大附魔
            if (!Arg.SY) return;
            const string key = "BigFM_BELT";

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

        /// <summary>
        /// 分别向常规和心无期间增加不同的Buff
        /// </summary>
        /// <param name="normal">常规期间buff</param>
        /// <param name="xw">心无期间buff</param>
        protected void AddPeriodBaseBuff(BaseBuff normal, BaseBuff xw)
        {
            BuffedFChars.Normal.AddBaseBuff(normal);
            BuffedFChars.XW.AddBaseBuff(xw);

            SkillFreqCTDFs.Normal.UpdateFChar();
            SkillFreqCTDFs.XW.UpdateFChar();
        }

        /// <summary>
        /// 分别向常规和心无期间增加不同的秘籍
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="xw"></param>
        protected void AddPeriodRecipe(Recipe normal, Recipe xw)
        {
            SkillDfs.Normal.AddRecipeAndApply(normal);
            SkillDfs.XW.AddRecipeAndApply(xw);
            SkillFreqCTDFs.Normal.UpdateFChar();
            SkillFreqCTDFs.XW.UpdateFChar();
        }


        /// <summary>
        /// 通用应用覆盖率Buff方法
        /// </summary>
        /// <param name="buff">Buff</param>
        /// <param name="key">key</param>
        /// <param name="normalCover">常规覆盖率</param>
        /// <param name="xwCover">心无覆盖率</param>
        protected void ApplyCoverBuff(Buff buff, string key, double normalCover, double xwCover)
        {
            BuffCover.SetCover(key, normalCover, xwCover);
            var normal = buff.Emit(normalCover, 1);
            var xw = buff.Emit(xwCover, 1);
            AddPeriodBaseBuff(normal, xw);
        }

        public void CalcSLBuff()
        {
            // 计算神力BUFF
            if (!Arg.SL) return;
            const string key = nameof(SL);

            var normal = SkillNum.Normal.BasicSLCover;
            var xw = SkillNum.XW.BasicSLCover;

            if (XFAppStatic.XinFaTag == "JY")
            {
                normal = SkillNum.Normal.GetFinalSLCover();
                xw = SkillNum.XW.GetFinalSLCover();
            }

            ApplyCoverBuff(SL, key, normal, xw);
        }

        public void CalcLongMenBuff()
        {
            // 计算龙门飞剑BUFF
            if (Arg.LongMen <= 0) return;
            var buffname = $"FeiJianJueYi·Feng#{Arg.LongMen:D}";
            var LMBuff = StaticXFData.GetExtraTriggerBuff(buffname);
            var normalCD = SkillNum.Normal.FeiJianJueYiBuffCD;
            var XWCD = SkillNum.XW.FeiJianJueYiBuffCD;
            var normalCover = LMBuff.Time / normalCD;
            var xwCover = LMBuff.Time / XWCD;
            const string key = "LM";
            ApplyCoverBuff(LMBuff, key, normalCover, xwCover);
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
}