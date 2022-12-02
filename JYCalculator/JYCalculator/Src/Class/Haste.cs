using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JYCalculator.Globals;
using static JX3CalculatorShared.Globals.StaticData;


namespace JYCalculator.Class
{
    public class Haste : HasteBase
    {
        public Haste(double fhs) : base(fhs)
        {
        }

        public Haste(int level) : base(level)
        {
        }


        /// <summary>
        /// 获取一段加速下惊羽关键技能的标准时间表，用于计算时间利用率
        /// </summary>
        public Period<JYSkillStandardTime> GetJY_SkillStandardTime()
        {
            int t1hsp = GetT1GCD_HSP(); // 一段加速阈值
            double BY = 0.5, CX = 3, BLp = 0.5;
            var GCD_time = SKT(GCD, t1hsp, 0);
            var BY_time = SKT(BY, t1hsp, 0);
            var CX_time = SKT(CX, t1hsp, 0);

            var BL_time = SKT(BLp, t1hsp, 0) * 4;

            var BigXW_GCD_time = SKT(GCD, t1hsp, JYStaticData.XWConsts.ExtraSP);
            var BigXW_BY_time = SKT(BY, t1hsp, JYStaticData.XWConsts.ExtraSP);
            var BigXW_HX_time = SKT(CX, t1hsp, JYStaticData.XWConsts.ExtraSP);
            var BigXW_BL_time = SKT(BLp, t1hsp, JYStaticData.XWConsts.ExtraSP) * 4;

            var Normal = new JYSkillStandardTime() {GCD = GCD_time, BY = BY_time, CX = CX_time, BL = BL_time};
            var BigXW = new JYSkillStandardTime() {GCD = BigXW_GCD_time, BY = BigXW_BY_time, CX = BigXW_HX_time, BL = BigXW_BL_time};
            var Res = new Period<JYSkillStandardTime>(Normal, BigXW);
            return Res;
        }
    }

    /// <summary>
    /// 用于描述惊羽标准技能加速时间的类
    /// </summary>
    public class JYSkillStandardTime
    {
        public double GCD;
        public double BY; // 暴雨单跳时间
        public double CX; // 穿心DOT时间
        public double BL; // 百里总时间
    }
}