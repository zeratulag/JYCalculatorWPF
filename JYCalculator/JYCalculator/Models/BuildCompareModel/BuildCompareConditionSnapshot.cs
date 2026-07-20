using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Src;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.ViewModels;
using System.Linq;

namespace JYCalculator.Models
{
    public class BuildCompareConditionSnapshot
    {
        public QiXueConfigModel QiXue { get; set; }
        public Target Target { get; set; }
        public AbilityItem Ability { get; set; }
        public ZhenFa Zhen { get; set; }
        public FightTimeSummary FightTime { get; set; }
        public BuffShellInput Buffs { get; set; }
        public SkillDataDF SkillDataDFMiJiQiXue { get; set; }
        public SkillBuild[] RecipeCompatibleSkillBuilds { get; set; }
        public BuffSpecialArg BuffSpecial { get; set; }
        public bool TargetAllWaysFullHP { get; set; }
        public double ShortTimeBonus { get; set; }
        public double BaseDps { get; set; }

        public static BuildCompareConditionSnapshot FromMainModel(MainWindowModel model)
        {
            var fightTime = CopyFightTime(model.FightTimeSummaryVM.Model);
            var buffs = new BuffShellInput(model.BuffVM.BuffAttrsDesc, model.BuffVM.DebuffAttrsDesc,
                model.ItemDTVM.AttrsDesc, model.BuffVM.Arg);
            var calcResult = model.CalcShell?.GetCalcResult();

            return new BuildCompareConditionSnapshot
            {
                QiXue = model.QiXueVM.Model,
                Target = model.FightOptionVM.SelectedTarget?.Copy(),
                Ability = model.FightOptionVM.SelectedAbility,
                Zhen = model.FightOptionVM.SelectedZhenFa,
                FightTime = fightTime,
                Buffs = buffs,
                SkillDataDFMiJiQiXue = model.SkillDataDFMiJiQiXueVM.Model.Copy(),
                RecipeCompatibleSkillBuilds = model.SkillMiJiVM.CompatibleSkillBuilds.ToArray(),
                BuffSpecial = model.BuffVM.Arg,
                TargetAllWaysFullHP = model.FightOptionVM.TargetAllWaysFullHP,
                ShortTimeBonus = model.CalcShellArg?.ShortTimeBonus ?? 0.0,
                BaseDps = calcResult?.FinalDPS ?? 0.0
            };
        }

        public FightTimeSummary CopyFightTime()
        {
            return CopyFightTime(FightTime);
        }

        public CalculatorShellArg CreateCalculatorShellArg(InitInputViewModel initInput)
        {
            return new CalculatorShellArg(RecipeCompatibleSkillBuilds, (int)initInput.InitCharVM.Haste,
                false, initInput.BigFMVM.Arg, BuffSpecial, initInput.EquipSpecialEffectVM.Model.Arg,
                TargetAllWaysFullHP, ShortTimeBonus, false);
        }

        private static FightTimeSummary CopyFightTime(FightTimeSummary source)
        {
            var result = new FightTimeSummary();
            result.Update(source.TotalTime, source.XWCD, source.XWDuration, source.IsShort);
            return result;
        }
    }
}
