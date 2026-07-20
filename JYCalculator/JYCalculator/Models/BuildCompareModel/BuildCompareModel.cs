using System;
using System.Collections.Generic;

namespace JYCalculator.Models
{
    public class BuildCompareModel
    {
        public BuildCompareArg Arg { get; }
        public BuildCompareResult Result { get; private set; }

        public BuildCompareModel(BuildCompareArg arg)
        {
            Arg = arg;
        }

        public BuildCompareResult Calc()
        {
            var result = new BuildCompareResult();
            var evaluator = new BuildCompareDpsEvaluator(Arg.Condition);

            foreach (var candidate in Arg.Candidates ?? new List<BuildCompareCandidate>())
            {
                result.Items.Add(CalcCandidate(evaluator, candidate));
            }

            Result = result;
            return Result;
        }

        public BuildCompareResultItem CalcCandidate(BuildCompareCandidate candidate)
        {
            var evaluator = new BuildCompareDpsEvaluator(Arg.Condition);
            return CalcCandidate(evaluator, candidate);
        }

        private BuildCompareResultItem CalcCandidate(BuildCompareDpsEvaluator evaluator,
            BuildCompareCandidate candidate)
        {
            var item = new BuildCompareResultItem
            {
                Candidate = candidate
            };

            try
            {
                var initInput = candidate.InitInput ?? candidate.CreateInitInput?.Invoke();
                candidate.InitInput = initInput;
                var calcResult = evaluator.Calc(initInput);
                item.CalcResult = calcResult;
                item.Success = calcResult.Success;
                item.Dps = calcResult.FinalDPS;
                item.DeltaDps = item.Dps - Arg.Condition.BaseDps;
            }
            catch (Exception ex)
            {
                item.Success = false;
                item.ErrorMessage = ex.Message;
            }

            return item;
        }
    }
}
