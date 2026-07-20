using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;

namespace JYCalculator.Utils
{
    public static class HasteSupportTool
    {
        public const int MinSupportedHasteRank = 5;

        private static readonly HasteBase CurrentLevelHaste = new HasteBase(StaticConst.CurrentLevel);

        public static int CalcHasteRank(double haste)
        {
            return CurrentLevelHaste.CalcHasteRank((int) haste);
        }

        public static bool IsSupportedHaste(double haste)
        {
            return IsSupportedHasteRank(CalcHasteRank(haste));
        }

        public static bool IsSupportedHasteRank(int hasteRank)
        {
            return hasteRank >= MinSupportedHasteRank;
        }
    }
}
