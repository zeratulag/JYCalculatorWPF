using JYCalculator.Src;

namespace JYCalculator.Models
{
    public readonly struct EventTriggerArg
    {
        public readonly bool SL; // 是否有神力
        public readonly bool SY; // 是否有伤腰大附魔

        public readonly bool BigFM_SHOES_120; // 是否有120级的伤鞋大附魔
        public readonly bool BigXW; // 是否为大心无

        public readonly int LongMen; // 龙门飞剑等级

        public readonly double PiaoHuangCover; // 飘黄Buff覆盖率

        public EventTriggerArg(bool sl, bool sy, bool bigFM_SHOES_120, bool bigXW, int longMen, double piaoHuangCover)
        {
            SL = sl;
            SY = sy;
            BigFM_SHOES_120 = bigFM_SHOES_120;
            BigXW = bigXW;
            LongMen = longMen;
            PiaoHuangCover = piaoHuangCover;
        }

        public EventTriggerArg(DPSCalcShellArg arg)
        {
            SY = arg.BigFM.Belt != null; // 是否有伤腰大附魔
            BigFM_SHOES_120 = arg.BigFM.Shoes?.DLCLevel == 120; // 是否有120级伤鞋子大附魔
            SL = arg.SL;
            BigXW = arg.BigXW;
            LongMen = arg.LongMen;
            PiaoHuangCover = arg.BuffSpecial.PiaoHuangCover;
        }
    }
}