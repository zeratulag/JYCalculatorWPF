using JYCalculator.Globals;
using System.Collections.Generic;
using static JYCalculator.Globals.XFStaticConst;

namespace JYCalculator.Class
{
    public partial class FullCharacter
    {
        #region 成员

        // 基础力道，最终力道
        public double BaseStrength { get; set; }
        public double FinalStrength { get; set; }

        // 力道提升
        public double StrengthPercent { get; set; } = 0;

        // 伤害提高
        public double PhysicsDamageAdd { get; set; } // 外功伤害提高

        #endregion

        #region 构造

        /// <summary>
        /// 从InitCharacter转换
        /// </summary>
        /// <param name="iChar">初始属性</param>
        public FullCharacter(InitCharacter iChar)
        {
            BaseStrength = iChar.BaseStrength;
            FinalStrength = iChar.FinalStrength;
            PhysicsBaseAttackPower = iChar.PhysicsBaseAttackPower;
            PhysicsFinalAttackPower = iChar.PhysicsFinalAttackPower;
            PhysicsBaseOvercome = iChar.PhysicsBaseOvercome;
            PhysicsFinalOvercome = iChar.PhysicsBaseOvercome;

            BaseWeaponDamage = iChar.BaseWeaponDamage;
            PhysicsCriticalPowerValue = iChar.PhysicsCriticalPowerValue;

            PhysicsCriticalStrikeRate = iChar.PhysicsCriticalStrikeRate; // 弩箭加1%会心。
            PhysicsCriticalStrike = iChar.PhysicsCriticalStrike;
            BaseStrain = iChar.BaseStrain;
            FinalStrain = BaseStrain;
            StrainRate = iChar.StrainRate;
            StrainPercent = 0.0;

            BaseSurplus = iChar.BaseSurplus;
            Haste = iChar.Haste;

            StrengthPercent = iChar.StrengthPercent;
            PhysicsAttackPowerPercent = 0.0;
            PhysicsOvercomePercent = 0.0;

            AllShieldIgnore = 0.0;
            PhysicsDamageAdd = 0.0;

            Name = iChar.Name;
            EquipScore = iChar.EquipScore;
            Had_BigFM_jacket = iChar.Had_BigFM_jacket;
            Had_BigFM_hat = iChar.Had_BigFM_hat;
            PostConstructor();
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="old"></param>
        public FullCharacter(FullCharacter old)
        {
            BaseStrength = old.BaseStrength;
            FinalStrength = old.FinalStrength;
            PhysicsBaseAttackPower = old.PhysicsBaseAttackPower;
            PhysicsFinalAttackPower = old.PhysicsFinalAttackPower;
            PhysicsBaseOvercome = old.PhysicsBaseOvercome;
            PhysicsFinalOvercome = old.PhysicsFinalOvercome;

            BaseWeaponDamage = old.BaseWeaponDamage;
            PhysicsCriticalPowerValue = old.PhysicsCriticalPowerValue;

            BaseSurplus = old.BaseSurplus;
            Haste = old.Haste;

            PhysicsCriticalStrike = old.PhysicsCriticalStrike;
            PhysicsCriticalStrikeRate = old.PhysicsCriticalStrikeRate;

            BaseStrain = old.BaseStrain;
            FinalStrain = old.FinalStrain;
            StrainPercent = old.StrainPercent;
            StrainRate = old.StrainRate;

            StrengthPercent = old.StrengthPercent;
            PhysicsAttackPowerPercent = old.PhysicsAttackPowerPercent;
            PhysicsOvercomePercent = old.PhysicsOvercomePercent;

            AllShieldIgnore = old.AllShieldIgnore;
            PhysicsDamageAdd = old.PhysicsDamageAdd;

            Name = old.Name;
            EquipScore = old.EquipScore;
            ExtraHaste = old.ExtraHaste;
            Is_XW = old.Is_XW;
            Has_Special_Buff = old.Has_Special_Buff;

            Had_BigFM_jacket = old.Had_BigFM_jacket;
            Had_BigFM_hat = old.Had_BigFM_hat;

            PostConstructor();
        }

        // 复制构造

        // 复制构造，更改名称

        #endregion

        #region 显示

        public IList<string> GetCatStrList()
        {
            var hadDict = new Dictionary<bool, string>()
            {
                {true, "已"}, {false, "未"}
            };

            var res = new List<string>
            {
                $"{FinalStrength:F2}({BaseStrength:F2}) 力道，" +
                $"{PhysicsFinalAttackPower:F2}({PhysicsBaseAttackPower:F2}) 攻击，" +
                $"{PhysicsFinalOvercome:F2}({PhysicsBaseOvercome:F2}) 破防，" +
                $"{BaseWeaponDamage:F2} 武器伤害",

                $"{PhysicsCriticalStrikeValue:P2} 会心，{PhysicsCriticalPowerValue:P2} 会效，{FinalStrainValue:P2} 无双，" +
                $"{Haste}({Haste / CurrentLevelParams.Haste + ExtraHaste / 1024:P2}) 加速，{BaseSurplus} 破招",
                $"{StrengthPercent:P2} 元气提升，{PhysicsAttackPowerPercent:P2} 攻击提升，{PhysicsOvercomePercent:P2} 破防提升，" +
                $"{AllShieldIgnore:P2} 无视防御A，{PhysicsDamageAdd:P2} 伤害提高，",

                $"{hadDict[Is_XW]}处于心无状态，{hadDict[Has_Special_Buff]}计算特殊增益",

                $"常规GCD: {Normal_GCD:F4} s, 大心无GCD: {BigXW_GCD:F4} s"
            };

            return res;
        }

        #endregion
    }
}