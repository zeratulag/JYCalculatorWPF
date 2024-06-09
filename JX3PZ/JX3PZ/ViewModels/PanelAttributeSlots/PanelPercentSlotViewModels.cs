using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PanelStrainSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.Strain);
        }
    }

    public class PanelHasteSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.Haste);
        }
    }

    public class PanelPhysicsShieldSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.PhysicsShield);
        }
    }

    public class PanelMagicShieldSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.MagicShield);
        }
    }

    public class PanelDecriticalDamageSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.DecriticalDamage);
        }
    }

    public class PanelToughnessSlotViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.Toughness);
        }
    }
}