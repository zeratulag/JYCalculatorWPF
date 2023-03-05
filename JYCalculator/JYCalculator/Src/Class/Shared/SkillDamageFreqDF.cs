using JX3CalculatorShared.Class;

namespace JYCalculator.Class
{
    /// <summary>
    /// 综合考虑伤害和频率
    /// </summary>
    public class SkillDamageFreqDF
    {
        #region 成员

        public readonly SkillDamageDF DamageDF;
        public readonly SkillFreqCTDF FreqDF;
        public readonly double Time;


        public string[] ValidSkillNames; // 频率大于0的技能Name集合
        public double RawDPS { get; private set; }

        public CombatStat Combatstat;

        public DamageDeriv RawDamageDeriv; // 导数

        #endregion

        #region 构造

        public SkillDamageFreqDF(SkillDamageDF damageDf, SkillFreqCTDF freqDf, double time)
        {
            DamageDF = damageDf;
            FreqDF = freqDf;
            Time = time;
            ValidSkillNames = FreqDF.GetValidNames();

        }

        #endregion

        public void Calc()
        {
            DamageDF.AttachFreq(FreqDF);
            CalcDPS();
            GetCombatStat();
            CalcDeriv();
        }

        public void CalcDPS()
        {
            RawDPS = 0;
            foreach (var name in ValidSkillNames)
            {
                RawDPS += DamageDF.Data[name].DPS;
            }
        }

        // 生成战斗统计
        public void GetCombatStat()
        {
            Combatstat = new CombatStat(this);
        }

        public DPSTableItem GetDPSTableItem(string name, string descName, double cover)
        {
            var res = new DPSTableItem(name, descName, RawDPS)
            {
                Cover = cover
            };
            res.Proceed();
            return res;
        }

        // 求导计算收益

        public DamageDeriv CalcDeriv()
        {
            DamageDF.GetDeriv();
            RawDamageDeriv = new DamageDeriv("收益");
            foreach (var name in ValidSkillNames)
            {
                var item = DamageDF.Data[name];
                RawDamageDeriv.WeightedAdd(item.Deriv, item.Freq);
            }

            return RawDamageDeriv;
        }

    }
}