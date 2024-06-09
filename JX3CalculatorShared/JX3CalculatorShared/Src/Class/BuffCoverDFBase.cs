namespace JX3CalculatorShared.Class
{
    public class BuffCoverDFBase : CoverDFBase
    {
        public BuffCoverDFBase()
        {
            var SL = new CoverItem("SL", "神力");
            var BigFM_BELT = new CoverItem("BigFM_BELT", "伤·腰");
            var LM = new CoverItem("LM", "飞剑绝意·锋");
            Data.Add(nameof(SL), SL);
            Data.Add(nameof(BigFM_BELT), BigFM_BELT);
            Data.Add(nameof(LM), LM);
        }
    }
}