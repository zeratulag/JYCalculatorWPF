using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelPrimarySlotViewModel: PzAttributeSlotsViewModelBase
    {
        public void UpdateFromSlot(PanelPrimarySlot slot)
        {
            Title = slot.BaseAttribute.SimpleDesc;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.Primary);
        }
    }
}