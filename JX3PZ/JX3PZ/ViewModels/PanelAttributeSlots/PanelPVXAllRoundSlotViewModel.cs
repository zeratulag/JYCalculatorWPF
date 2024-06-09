using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelPVXAllRoundSlotViewModel: PzAttributeSlotsViewModelBase
    {
        public const string CTitle = "全能";

        public void UpdateFromSlot(PanelPVXAllRoundSlot slot)
        {
            Title = CTitle;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.PVXAllRoundSlot);
        }
    }
}