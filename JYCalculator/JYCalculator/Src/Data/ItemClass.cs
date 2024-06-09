using System;
using JX3CalculatorShared.Data;
using JYCalculator.Globals;


namespace JYCalculator.Data
{
    public class AbilitySkillNumItem : AbilitySkillNumItemBase
    {
        public double DP { get; set; }
        public double BY_Cast { get; set; }
        public double ZM_SF { get; set; }
        public double ZX { get; set; }
        public double CXL { get; set; } = 0;
        public double BL { get; set; } = 0;
        public double KongQueLing { get; set; } = 0;
        public double LveYingQiongCang { get; set; } = 0;

        public GenreTypeEnum GenreEnum { get; private set; }

        public void Parse()
        {
            Enum.TryParse(Genre, out GenreTypeEnum res);
            GenreEnum = res;
        }

}

    public class SkillInfoItem : SkillInfoItemBase
    {
        public double AP_Coef { get; set; } = 0;
        public double IgnoreB { get; set; } = 0;

    }

    public class DiamondValueItem : DiamondValueItemBase
    {
        // 五行石镶嵌数值
        public int L { get; set; }
    }
}