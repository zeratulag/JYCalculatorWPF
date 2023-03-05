namespace JYCalculator.Src
{
    public partial class CalculatorShell
    {
        #region 天罗占位区
        protected void CalcTLSkillDataDFModel()
        {
        }

        protected void CalcBoLiangFenXingArg()
        {
        }

        #endregion



        public void GetDPSKernelShell()
        {
            var LM = Equip.WP.IsLongMen ? Equip.WP.Value : 0; // 龙门飞剑等级
            var arg = new DPSCalcShellArg(Equip.SL, QiXue.聚精凝神, LM, Equip.YZ, BigFM, Buffs.BuffSpecial,
                InitInput.NoneBigFMInitCharacter);
            KernelShell = new DPSKernelShell(FullCharGroup.ZhenBuffed, CTarget, SkillDFs, SkillNum, FightTime, arg);
        }
    }
}