using JX3PZ.Globals;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelAttackSlotViewModel : PzAttributeSlotsViewModelBase
    {
        public const string CTitle = "攻击力";
        public string BaseTitle { get; } = $"{PzConstString.Base}{CTitle}";
        public int BaseValue { get; private set; }

        public void UpdateFromSlot(PanelAttackSlot slot)
        {
            Title = CTitle;
            BaseValue = slot.Base;
            ValueDesc = slot.GetValueDesc();
            Desc1 = slot.GetDesc1();
            Desc2 = slot.GetDesc2();
            DescTips = slot.GetDescTips();
            ToolTip = GetToolTipFromDescTips();
        }

        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFromSlot(model.XFPanel.Attack);
        }
    }
}