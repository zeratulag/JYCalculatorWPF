using JX3CalculatorShared.Src.Data;


namespace JYCalculator.Src.Data
{
    public class AbilitySkillNumItem: AbilitySkillNumItemBase
    {
        public double DP { get; set; }
        public double BY_Cast { get; set; }
        public double ZM_SF { get; set; }
        public double ZX { get; set; }
    }


    public class DiamondValueItem : DiamondValueItemBase
    {
        // 五行石镶嵌数值
        public int L { get; set; }
    }
}