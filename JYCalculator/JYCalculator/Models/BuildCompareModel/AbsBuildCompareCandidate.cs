using JX3CalculatorShared.Globals;
using System;

namespace JYCalculator.Models
{
    public abstract class AbsBuildCompareCandidate
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Level { get; set; }
        public string LevelText { get; set; }
        public string AttributeDesc { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsEmpty { get; set; }
        public int IconID { get; set; }
        public QualityEnum Quality { get; set; }
        public InitInputSav InitInput { get; set; }
        public Func<InitInputSav> CreateInitInput { get; set; }
    }
}
