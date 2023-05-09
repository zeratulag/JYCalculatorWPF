using System.Dynamic;
using JX3PZ.Globals;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelCriticalSlotViewModel : PzAttributeSlotsViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            throw new System.NotImplementedException();
        }
    }

    public class PanelCriticalStrikeSlotViewModel : PanelCriticalSlotViewModel
    {
        public void UpdateFromSlot(PanelCriticalStrikeSlot slot)
        {
            Title = slot.PointAttribute.EquipTag;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.CriticalStrike);
        }
    }

    public class PanelCriticalDamageSlotViewModel : PanelCriticalSlotViewModel
    {
        public void UpdateFromSlot(PanelCriticalDamageSlot slot)
        {
            Title = "会心效果";
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.CriticalDamage);
        }
    }
}