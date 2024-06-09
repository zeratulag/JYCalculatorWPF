using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Src;
using System.Collections.Generic;

namespace JX3PZ.Models
{
    public class CharacterPanel
    {
        public CharacterPanelPrimary Primary { get; } // 主属性
        public CharacterPanelAttack Attack { get; } // 攻击
        public CharacterPanelCriticalStrike CriticalStrike { get; } // 会心
        public CharacterPanelCriticalDamage CriticalDamage { get; } // 会效
        public CharacterOvercome Overcome { get; } // 破防
        public PanelHasteSlot Haste { get; } // 加速
        public PanelStrainSlot Strain { get; } // 无双
        public PanelSurplusSlot Surplus { get; } // 破招
        public PanelPVXAllRoundSlot PVXAllRoundSlot { get; } // 全能
        public PanelPhysicsShieldSlot PhysicsShield { get; } // 外防
        public PanelMagicShieldSlot MagicShield { get; } // 内防
        public PanelDecriticalDamageSlot DecriticalDamage { get; } // 化劲
        public PanelToughnessSlot Toughness { get; } // 御劲
        public PanelWeaponSlot MeleeWeapon { get; } // 近身武器伤害属性
        public PanelWeaponSlot RangeWeapon { get; } // 远程武器伤害属性

        public PanelMaxLifeSlot MaxLife { get; } // 最大生命值

        public PlayerLevelData LevelData;

        public static readonly XinFaAttribute CurrentXinFa = XinFaAttribute.CurrentXinFa;

        public Dictionary<string, int> SystemPrimary; // 系统主属性提供的加成
        public XinFaPrimaryAttributeValue XinFaPrimary; // 心法主属性提供的加成

        public CharacterPanel()
        {
            Primary = new CharacterPanelPrimary();
            Attack = new CharacterPanelAttack();
            CriticalStrike = new CharacterPanelCriticalStrike();
            CriticalDamage = new CharacterPanelCriticalDamage();
            Overcome = new CharacterOvercome();
            Haste = new PanelHasteSlot();
            Strain = new PanelStrainSlot();
            Surplus = new PanelSurplusSlot();
            PVXAllRoundSlot = new PanelPVXAllRoundSlot();
            PhysicsShield = new PanelPhysicsShieldSlot();
            MagicShield = new PanelMagicShieldSlot();
            DecriticalDamage = new PanelDecriticalDamageSlot();
            Toughness = new PanelToughnessSlot();

            MeleeWeapon = new PanelWeaponSlot(WeaponAttributeTypeEnum.Melee);
            RangeWeapon = new PanelWeaponSlot(WeaponAttributeTypeEnum.Range);

            MaxLife = new PanelMaxLifeSlot();

            var ld = StaticPzData.Data.LevelData["m1"];
            AttachLevelData(ld);
            ApplyLevelDataPrimary();
        }

        /// <summary>
        /// 应用玩家初始属性
        /// </summary>
        public void AttachLevelData(PlayerLevelData ld)
        {
            LevelData = ld;
        }

        public void UpdatePrimaryFrom(IDictionary<string, int> valueDict)
        {
            Primary.UpdateFrom(valueDict);
        }

        public void ApplyLevelDataPrimary()
        {
            // 先应用主属性
            Primary.ApplyLevelData(LevelData);
            MaxLife.ApplyLevelData(LevelData);
        }

        public void CalcFirst(IDictionary<string, int> valueDict)
        {
            Primary.UpdateFrom(valueDict);
            Attack.UpdateFrom(valueDict);
            CriticalStrike.UpdateFrom(valueDict);
            CriticalDamage.UpdateFrom(valueDict);
            Overcome.UpdateFrom(valueDict);
            Haste.UpdateFrom(valueDict);
            Strain.UpdateFrom(valueDict);
            Surplus.UpdateFrom(valueDict);
            PhysicsShield.UpdateFrom(valueDict);
            MagicShield.UpdateFrom(valueDict);
            DecriticalDamage.UpdateFrom(valueDict);
            MeleeWeapon.UpdateFrom(valueDict);
            RangeWeapon.UpdateFrom(valueDict);
            MaxLife.UpdateFrom(valueDict);
            Toughness.UpdateFrom(valueDict);
            PVXAllRoundSlot.UpdateFrom(valueDict);
        }

        public void CalcSystemPrimary()
        {
            var dict = GetSystemPrimaryAttributeValues();
            Attack.UpdateFrom(dict);
            CriticalStrike.UpdateFrom(dict);
            Overcome.UpdateFrom(dict);
            MaxLife.UpdateFrom(dict);
            SystemPrimary = dict;
        }

        public void Calc(IDictionary<string, int> valueDict)
        {
            CalcFirst(valueDict);
            CalcSystemPrimary();
            CalcXFPrimaryAttributeValues();
            CalcAllRound();
        }

        public void CalcAllRound()
        {
            var dict = PVXAllRoundSlot.ConvertDict;
            Surplus.UpdateFrom(dict);
            Strain.UpdateFrom(dict);
            DecriticalDamage.UpdateFrom(dict);
        }

        public Dictionary<string, int> GetSystemPrimaryAttributeValues()
        {
            // 计算系统主属性加成
            var res = Primary.GetSystemPrimaryAttributeValues();
            return res;
        }

        public XinFaPrimaryAttributeValue GetXFPrimaryAttributeValues()
        {
            var primary = CurrentXinFa.Primary; // 主属性类型
            int primaryAttributeValue = Primary[primary].Final; // 主属性点数
            var res = new XinFaPrimaryAttributeValue(CurrentXinFa, primaryAttributeValue);
            return res;
        }

        public void CalcXFPrimaryAttributeValues()
        {
            var res = GetXFPrimaryAttributeValues();
            XinFaPrimary = res;
            Attack[res.XF.Attack].AddAdditional(res.AdditionalAttack);
            Attack[res.XF.Attack].Calc();
            CriticalStrike[res.XF.Critical].AddPoint(res.CriticalStrikePoint);
            CriticalStrike[res.XF.Critical].Calc();
            Overcome[res.XF.Overcome].AddAdditional(res.AdditionalOvercome);
            Overcome[res.XF.Overcome].Calc();

            Surplus.CalcXFDamage(CurrentXinFa);
        }
    }
}