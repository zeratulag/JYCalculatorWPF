using JX3CalculatorShared.Class;

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