using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Globals;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class PzResultViewModel : ObservableObject
    {
        // 用于显示配装结果
        public PzEquipScoreViewModel Score { get; } // 装分
        public PanelMaxLifeSlotViewModel MaxLife { get; } // 气血
        public PanelPrimarySlotViewModel Vitality { get; } // 体质
        public PanelPrimarySlotViewModel Primary { get; } // 主属性
        public PanelAttackSlotViewModel Attack { get; } // 攻击
        public PanelCriticalStrikeSlotViewModel CriticalStrike { get; }
        public PanelCriticalDamageSlotViewModel CriticalDamage { get; }
        public PanelOvercomeSlotViewModel Overcome { get; }
        public PanelStrainSlotViewModel Strain { get; }
        public PanelHasteSlotViewModel Haste { get; }
        public PanelSurplusSlotViewModel Surplus { get; }
        public PanelWeaponSlotViewModel MeleeWeapon { get; }

        public PanelPhysicsShieldSlotViewModel PhysicsShield { get; }
        public PanelMagicShieldSlotViewModel MagicShield { get; }
        public PanelDecriticalDamageSlotViewModel DecriticalDamage { get; }
        public PanelToughnessSlotViewModel Toughness { get; }
        public PanelPVXAllRoundSlotViewModel PVXAllRoundSlot { get; }

        public PzResultViewModel()
        {
            Score = new PzEquipScoreViewModel();
            MaxLife = new PanelMaxLifeSlotViewModel();
            Vitality = new PanelPrimarySlotViewModel();
            Primary = new PanelPrimarySlotViewModel();
            Attack = new PanelAttackSlotViewModel();
            CriticalStrike = new PanelCriticalStrikeSlotViewModel();
            CriticalDamage = new PanelCriticalDamageSlotViewModel();
            Overcome = new PanelOvercomeSlotViewModel();
            Strain = new PanelStrainSlotViewModel();
            Surplus = new PanelSurplusSlotViewModel();
            Haste = new PanelHasteSlotViewModel();
            MeleeWeapon = new PanelWeaponSlotViewModel(WeaponAttributeTypeEnum.Melee);
            PhysicsShield = new PanelPhysicsShieldSlotViewModel();
            MagicShield = new PanelMagicShieldSlotViewModel();
            DecriticalDamage = new PanelDecriticalDamageSlotViewModel();
            Toughness = new PanelToughnessSlotViewModel();

            PVXAllRoundSlot = new PanelPVXAllRoundSlotViewModel();
        }

        public PzResultViewModel(PzPlanModel model)
        {
            UpdateFrom(model);
        }

        public void UpdateFrom(PzPlanModel model)
        {
            Score.UpdateFrom(model);
            MaxLife.UpdateFrom(model);
            Vitality.UpdateFromSlot(model.XFPanel.Vitality);
            Primary.UpdateFrom(model);
            Attack.UpdateFrom(model);
            CriticalStrike.UpdateFrom(model);
            CriticalDamage.UpdateFrom(model);
            Overcome.UpdateFrom(model);
            Strain.UpdateFrom(model);
            Surplus.UpdateFrom(model);
            Haste.UpdateFrom(model);
            MeleeWeapon.UpdateFrom(model);
            PhysicsShield.UpdateFrom(model);
            MagicShield.UpdateFrom(model);
            DecriticalDamage.UpdateFrom(model);
            Toughness.UpdateFrom(model);

            PVXAllRoundSlot.UpdateFrom(model);
        }

        public PzAttributeSlotsViewModelBase[] GetAttributeSlots()
        {
            var res = new PzAttributeSlotsViewModelBase[]
            {
                Attack, CriticalStrike, CriticalDamage, Overcome, Haste,
                Surplus, Strain, MeleeWeapon, Primary, Vitality,
                MaxLife, PhysicsShield, MagicShield, Toughness, DecriticalDamage,
            };
            return res;
        }
    }
}