using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Globals;
using JX3PZ.Models;
using System;
using System.Collections.Generic;

namespace JX3PZ.ViewModels
{
    public class PanelMaxLifeSlotViewModel : PzAttributeSlotsViewModelBase
    {
        public string MaxLifeBase { get; private set; }
        public string MaxLifePercentAdd { get; private set; }
        public string FinalMaxLifeBase { get; private set; }
        public string FinalMaxLifeFinal { get; private set; }

        public const string HP = "气血最大值";

        public int Value { get; private set; }

        public PanelMaxLifeSlotViewModel()
        {
            Title = "最大气血";
        }

        public PanelMaxLifeSlotViewModel(PanelMaxLifeSlot slot) : this()
        {
            UpdateFromSlot(slot);
        }

        public void UpdateFromSlot(PanelMaxLifeSlot slot)
        {
            Value = slot.FinalMaxLifeFinal;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.MaxLife);
        }
    }
}