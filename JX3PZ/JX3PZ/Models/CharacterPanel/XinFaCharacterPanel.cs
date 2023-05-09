using JX3PZ.Data;

namespace JX3PZ.Models
{
    public class XinFaCharacterPanel
    {
        public PanelPrimarySlot Vitality { get; } // 体质
        public PanelPrimarySlot Primary { get; } // 主属性
        public PanelAttackSlot Attack { get; } // 攻击
        public PanelCriticalStrikeSlot CriticalStrike { get; } // 会心
        public PanelCriticalDamageSlot CriticalDamage { get; } // 会效
        public PanelOvercomeSlot Overcome { get; } // 破防
        public PanelHasteSlot Haste { get; } // 加速
        public PanelStrainSlot Strain { get; } // 无双
        public PanelSurplusSlot Surplus { get; } // 破招
        public PanelPhysicsShieldSlot PhysicsShield { get; } // 外防
        public PanelMagicShieldSlot MagicShield { get; } // 内防
        public PanelDecriticalDamageSlot DecriticalDamage { get; } // 化劲
        public PanelToughnessSlot Toughness { get; }

        public PanelWeaponSlot MeleeWeapon { get; } // 近身武器伤害属性
        public PanelWeaponSlot RangeWeapon { get; } // 远程武器伤害属性
        public PanelMaxLifeSlot MaxLife { get; } // 最大生命值

        public readonly XinFaAttribute CurrentXinFa;

        public string Name { get; set; } = "";

        public XinFaCharacterPanel(){}

        public XinFaCharacterPanel(CharacterPanel cPanel)
        {
            CurrentXinFa = CharacterPanel.CurrentXinFa;

            Primary = cPanel.Primary[CurrentXinFa.Primary]; 
            Vitality = cPanel.Primary.Vitality;
            Attack = cPanel.Attack[CurrentXinFa.Attack];
            CriticalStrike = cPanel.CriticalStrike[CurrentXinFa.Critical];
            CriticalDamage = cPanel.CriticalDamage[CurrentXinFa.Critical];
            Overcome = cPanel.Overcome[CurrentXinFa.Overcome];
            Haste = cPanel.Haste;
            Strain = cPanel.Strain;
            Surplus = cPanel.Surplus;
            PhysicsShield = cPanel.PhysicsShield;
            MagicShield = cPanel.MagicShield;
            DecriticalDamage = cPanel.DecriticalDamage;
            Toughness = cPanel.Toughness;
            MeleeWeapon = cPanel.MeleeWeapon;
            RangeWeapon = cPanel.RangeWeapon;
            MaxLife = cPanel.MaxLife;
        }
    }
}