using JX3CalculatorShared.Class;
using JYCalculator.Utils;

namespace JYCalculator.Class
{
    public interface IFinalCharacterPanel
    {
        double Haste { get; }
        int HasteRank { get; }
    }

    public class CalcResult : CalcResultBase
    {
        public IFinalCharacterPanel NormalFinal;

        public bool IsHasteChangeSupported(double hasteDelta)
        {
            if (hasteDelta == 0)
            {
                return true;
            }

            return NormalFinal != null && HasteSupportTool.IsSupportedHaste(NormalFinal.Haste + hasteDelta);
        }
    }
}
