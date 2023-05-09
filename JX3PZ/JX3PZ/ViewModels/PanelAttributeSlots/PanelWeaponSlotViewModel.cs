using System.Dynamic;
using JX3PZ.Globals;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelWeaponSlotViewModel : PzAttributeSlotsViewModelBase
    {
        public const string CTitle = "武器伤害";

        public readonly WeaponAttributeTypeEnum WeaponType;
        public readonly string RealTitle;

        public PanelWeaponSlotViewModel(WeaponAttributeTypeEnum weaponType = WeaponAttributeTypeEnum.Melee)
        {
            WeaponType = weaponType;
            RealTitle = PanelWeaponSlot.GetTypeDesc(WeaponType);
        }

        public void UpdateFromSlot(PanelWeaponSlot slot)
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
            UpdateFromSlot(model.XFPanel.MeleeWeapon);
        }
    }
}