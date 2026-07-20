using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using PropertyChanged;

namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 候选项的收益展示信息
    /// </summary>
    public class ProfitItemInfo : ObservableObject
    {
        [AlsoNotifyFor(nameof(Color))]
        public double Value { get; set; }

        public double Percent { get; set; }

        [AlsoNotifyFor(nameof(Show), nameof(ShowValue), nameof(ShowUnsupportedHasteChange))]
        public bool IsCurrent { get; set; }

        [AlsoNotifyFor(nameof(Color), nameof(ShowValue), nameof(ShowUnsupportedHasteChange))]
        public bool IsUnsupportedHasteChange { get; set; }

        public string Color => IsUnsupportedHasteChange
            ? DeltaDisplayTool.NegativeColor
            : DeltaDisplayTool.GetPointColor(Value);

        public bool Show => !IsCurrent;
        public bool ShowValue => Show && !IsUnsupportedHasteChange;
        public bool ShowUnsupportedHasteChange => Show && IsUnsupportedHasteChange;

        public void Clear()
        {
            Value = 0;
            Percent = 0;
            IsCurrent = true;
            IsUnsupportedHasteChange = false;
        }
    }
}
