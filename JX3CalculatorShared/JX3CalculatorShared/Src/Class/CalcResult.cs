namespace JX3CalculatorShared.Class
{
    public class CalcResultBase
    {
        public CalcResultStatus Status = CalcResultStatus.Default;
        public bool Success => Status == CalcResultStatus.Success;
        public double FinalDPS;
        public string ProfitOrderDesc;
        public DamageDerivBase DamageDeriv;
        public string FailReason;
    }

    public enum CalcResultStatus
    {
        Default = -1,
        Success = 0,
        Error = 1,
        InvalidInput = 2,
        UnsupportedSkillBuild = 3,
        UnsupportedHaste = 4 // 加速不支持，此时计算结果不可信
    }
}
