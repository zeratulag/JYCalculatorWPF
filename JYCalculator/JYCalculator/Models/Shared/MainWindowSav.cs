using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.ViewModels;
using System.Collections.Generic;

namespace JYCalculator.Models
{
    public class MainWindowSav
    {
        public AppMetaInfo AppMeta;

        public double SnapFinalDPS;

        public InitInputSav InitInput;
        public Dictionary<int, string[]> SkillMiJi;
        public int[] QiXue;
        public AllBuffConfigSav Buff;
        public ItemDTConfigSav ItemDT;
        public FightOptionSav FightOption;

    }
}