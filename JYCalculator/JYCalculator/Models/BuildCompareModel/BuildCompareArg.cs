using System.Collections.Generic;

namespace JYCalculator.Models
{
    public class BuildCompareArg
    {
        public BuildCompareConditionSnapshot Condition { get; set; }
        public IList<BuildCompareCandidate> Candidates { get; set; }
    }
}
