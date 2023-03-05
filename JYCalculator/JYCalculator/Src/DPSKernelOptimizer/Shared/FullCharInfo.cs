using JX3CalculatorShared.Class;
using JYCalculator.Class;

namespace JYCalculator.Src
{
    public class FullCharInfo
    {
        public readonly CharacterInfo Info;
        public readonly FullCharacter FChar;
        public readonly double CTProportion;
        public readonly double WSProportion;
        public double DPS;

        public FullCharInfo(CharacterInfo info, FullCharacter fChar, double ctProportion, double wsProportion)
        {
            Info = info;
            FChar = fChar;
            CTProportion = ctProportion;
            WSProportion = wsProportion;
        }
    }
}