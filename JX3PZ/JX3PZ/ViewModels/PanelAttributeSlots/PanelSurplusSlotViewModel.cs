using System.Dynamic;
using JX3PZ.Globals;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelSurplusSlotViewModel : PzAttributeSlotsViewModelBase
    {
        public const string CTitle = "破招";

        public void UpdateFromSlot(PanelSurplusSlot slot)
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
            UpdateFromSlot(model.XFPanel.Surplus);
        }
    }
}