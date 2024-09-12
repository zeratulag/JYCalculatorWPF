using JX3CalculatorShared.Class;

namespace JYCalculator.JX3CalculatorShared.Src
{
    public class KAttributeEntry : KAttributeEntryBase
    {
        public double Value { get; set; }
        public KAttributeEntry(string modifyType, double value) : base(modifyType)
        {
            Value = value;
        }
    }
}