using JX3CalculatorShared.Class;
using JYCalculator.Globals;


namespace JYCalculator.Class
{
    /// <summary>
    /// 描述覆盖率统计的类
    /// </summary>
    public class BuffCoverDF : BuffCoverDFBase
    {
        public BuffCoverDF() : base()
        {
            if (XFAppStatic.XinFaTag == "TL")
            {
                var NX = new CoverItem("NX", "弩心");
                var CH = new CoverItem("CH", "催寒");
                var BL7 = new CoverItem("BL7", "擘两分星70-");
                var BL3 = new CoverItem("BL3", "擘两分星30+");

                Data.Add(nameof(NX), NX);
                Data.Add(nameof(CH), CH);
                Data.Add(nameof(BL7), BL7);
                Data.Add(nameof(BL3), BL3);
            }
        }
    }
}