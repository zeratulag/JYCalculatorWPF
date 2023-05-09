using System.Dynamic;
using JX3CalculatorShared.Globals;
using JX3PZ.Globals;
using JX3PZ.Models;
using Syncfusion.Windows.Shared;

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

    public class PanelPhysicsShieldViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.PhysicsShield);
        }
    }

    public class PanelMagicShieldViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.MagicShield);
        }
    }

    public class PanelDecriticalDamageViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.DecriticalDamage);
        }
    }

    public class PanelToughnessViewModel : PanelPercentSlotViewModelBase
    {
        public override void UpdateFrom(PzPlanModel model)
        {
            UpdateFrom(model, _ => _.Toughness);
        }
    }
}