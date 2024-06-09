using JX3PZ.Models;
using System;

namespace JX3PZ.ViewModels
{
    public class PanelPercentSlotViewModelBase : PzAttributeSlotsViewModelBase
    {
        public PanelPercentAttributeSlot Slot { get; protected set; }

        public void UpdateFromSlot(PanelPercentAttributeSlot slot)
        {
            Slot = slot;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
        }

        protected void UpdateFrom(PzPlanModel model, Func<XinFaCharacterPanel, PanelPercentAttributeSlot> GetSlot)
        {
            var slot = GetSlot(model.XFPanel);
            Title = slot.PointAttribute.SimpleDesc;
            UpdateFromSlot(slot);
        }
    }
}