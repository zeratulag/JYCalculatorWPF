using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Utils;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public abstract class PzAttributeSlotsViewModelBase : ObservableObject
    {
        public string Title { get; protected set; } = ""; // 标题描述
        public string ValueDesc { get; protected set; } = ""; // 数值描述
        public List<string> DescTips { get; protected set; } = null;
        public string ToolTip { get; protected set; } = ""; // 提示文字
        public string Desc1 { get; protected set; } = "";
        public string Desc2 { get; protected set; } = "";
        public bool HasDesc2 => Desc2.IsNotEmptyOrWhiteSpace();

        public PzAttributeSlotsViewModelBase()
        {
        }

        public abstract void UpdateFrom(PzPlanModel model);

        public string GetToolTipFromDescTips()
        {
            var res = string.Join("\n", DescTips);
            return res;
        }
    }
}