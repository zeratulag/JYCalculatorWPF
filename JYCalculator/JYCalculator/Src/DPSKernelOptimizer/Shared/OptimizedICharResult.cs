using JX3CalculatorShared.Class;

namespace JYCalculator.Src
{
    public class OptimizedICharResult : OptimizedICharResultBase
    {
        public OptimizedICharResult() : base()
        {
        }

        public OptimizedICharResult(FullCharInfo fInfo) : this()
        {
            CT = fInfo.Info.ICT;
            OC = fInfo.Info.IOC;
            WS = fInfo.Info.IWS;
            PZ = fInfo.Info.IPZ;

            CTProportion = fInfo.CTProportion;
            WSProportion = fInfo.WSProportion;
        }
    }
}