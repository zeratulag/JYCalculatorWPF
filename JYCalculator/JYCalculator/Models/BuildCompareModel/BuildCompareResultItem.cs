using JX3CalculatorShared.Class;
using JYCalculator.Class;

namespace JYCalculator.Models
{
    public class BuildCompareResultItem
    {
        public BuildCompareCandidate Candidate { get; set; }
        public bool Success { get; set; }
        public double Dps { get; set; }
        public double DeltaDps { get; set; }
        public string ErrorMessage { get; set; }
        public CalcResult CalcResult { get; set; }
    }
}
