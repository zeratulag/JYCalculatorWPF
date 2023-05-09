using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;

namespace JX3CalculatorShared.ViewModels
{
    public class FightOptionSummaryViewModel : ObservableObject
    {
        public ZhenFa CZhenFa { get; set; }
        public Target CTarget { get; set; }
        public int FightTime { get; set; }
        public bool ShortFight { get; set; }
        public AbilityItem CAbility { get; set; }

        public string FightTimeDesc => GetFightTimeDesc();
        public bool HasZhen => CZhenFa != null && ! CZhenFa.IsNone; // 是否有阵
        public string GetFightTimeDesc()
        {
            string desc = $"{FightTime}秒";
            if (ShortFight)
            {
                desc += "（短时间）";
            }

            return desc;
        }
    }
}