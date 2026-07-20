using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Src;
using JYCalculator.ViewModels;

namespace JYCalculator.Models
{
    public class BuildCompareDpsEvaluator
    {
        public BuildCompareConditionSnapshot Condition { get; }

        public BuildCompareDpsEvaluator(BuildCompareConditionSnapshot condition)
        {
            Condition = condition;
        }

        public CalcResult Calc(InitInputSav initInputSav)
        {
            var initInput = CreateRuntimeInitInput(initInputSav);
            var arg = Condition.CreateCalculatorShellArg(initInput);
            var shell = new CalculatorShell(initInput,
                Condition.QiXue,
                Condition.Target?.Copy(),
                Condition.Ability,
                Condition.Zhen,
                Condition.CopyFightTime(),
                Condition.Buffs,
                Condition.SkillDataDFMiJiQiXue.Copy(),
                arg);

            shell.Calc();
            var calcResult = shell.GetCalcResult();

            if (!calcResult.Success && string.IsNullOrEmpty(calcResult.FailReason))
            {
                calcResult.FailReason = shell.FinalDPStxt ?? "计算失败";
            }

            return calcResult;
        }

        private static InitInputViewModel CreateRuntimeInitInput(InitInputSav sav)
        {
            var initChar = sav.InitChar.Copy();
            var equipOption = new EquipOptionConfigViewModel();
            var bigFm = new BigFMConfigViewModel();
            var equipSpecialEffect = new EquipSpecialEffectConfigViewModel();
            var initInput = new InitInputViewModel(initChar, equipOption, bigFm, equipSpecialEffect);
            initInput.Load(sav);
            return initInput;
        }
    }
}
