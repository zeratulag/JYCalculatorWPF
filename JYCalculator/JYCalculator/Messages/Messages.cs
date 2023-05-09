using JX3CalculatorShared.Class;
using JX3PZ.Models;
using JYCalculator.Models;
using JYCalculator.Src;

namespace JYCalculator.Messages
{
    public class CalcResultMessage
    {
        public readonly CalcResult Result;
        public readonly CalcInputViewModel Input;

        public CalcResultMessage(CalcResult result, CalcInputViewModel input = null)
        {
            Result = result;
            Input = input;
        }
    }
}