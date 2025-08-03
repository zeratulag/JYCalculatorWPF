using JX3CalculatorShared.Class;
using JX3PZ.Models;
using JYCalculator.Globals;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Data;
using static JYCalculator.Globals.XFStaticConst;

namespace JYCalculator.Class
{
    public partial class InitCharacter
    {
        #region 成员

        // 力道
        public double BaseStrength { get; set; }

        [JsonIgnore] public double FinalStrength => BaseStrength * (1 + StrengthPercent);
        public double StrengthPercent { get; set; } = 0;

        [JsonIgnore] public bool HasBaseStrengthPercent => StrengthPercent > 0; // 是否有非基础力道

        #endregion

        #region 构造

        /// <summary>
        /// 复制构造
        /// </summary>
        /// <param name="old">旧的对象</param>
        public InitCharacter(InitCharacter old) : base()
        {
            BaseStrength = old.BaseStrength;
            StrengthPercent = old.StrengthPercent;
            PhysicsBaseAttackPower = old.PhysicsBaseAttackPower;
            PhysicsFinalAttackPower = old.PhysicsFinalAttackPower;
            BaseWeaponDamage = old.BaseWeaponDamage;

            PhysicsCriticalStrike = old.PhysicsCriticalStrike;
            PhysicsCriticalStrikeRate = old.PhysicsCriticalStrikeRate;

            PhysicsCriticalPowerValue = old.PhysicsCriticalPowerValue;
            StrainRate = old.StrainRate;

            BaseSurplus = old.BaseSurplus;
            PhysicsBaseOvercome = old.PhysicsBaseOvercome;
            Haste = old.Haste;

            Had_BigFM_jacket = old.Had_BigFM_jacket;
            Had_BigFM_hat = old.Had_BigFM_hat;
            Name = old.Name;
            EquipScore = old.EquipScore;
            PostConstructor();
        }

        #endregion

        /// <summary>
        /// 从导入的JB面板更新
        /// </summary>
        /// <param name="panel"></param>
        protected void _UpdateFromJBPanel(JBBB panel)
        {
            BaseStrength = panel.Strength;
            StrengthPercent = 0;
            PhysicsBaseAttackPower = panel.PhysicsAttackPowerBase;
            PhysicsFinalAttackPower = panel.PhysicsAttackPower;
            BaseWeaponDamage = panel.MeleeWeaponDamage + panel.MeleeWeaponDamageRand / 2;

            PhysicsCriticalStrike = panel.PhysicsCriticalStrike;

            var criticalStrikeRate =
                panel.CalcPhysicsCriticalStrikeRate(XFStaticConst.CurrentLevelParams.CriticalStrike);
            if (Math.Abs(criticalStrikeRate - 0.01) < 0.001)
            {
                PhysicsCriticalStrikeRate = 0.01; // 弩箭加1%会心。
            }
            //FinalStrainValue = panel.StrainPercent;


            PhysicsCriticalPowerValue = panel.PhysicsCriticalDamagePowerPercent;
            StrainRate = 0;
            BaseSurplus = panel.SurplusValue;
            BaseStrain = panel.Strain;
            //PhysicsBaseOvercome = Math.Round(panel.PhysicsOvercomePercent * CurrentLevelParams.Overcome);
            PhysicsBaseOvercome = panel.PhysicsOvercome;
            //Haste = Math.Round(panel.HastePercent * CurrentLevelParams.Haste);
            Haste = panel.Haste;
            Had_BigFM_jacket = panel.EquipList.Had_BigFM_jacket;
            Had_BigFM_hat = panel.EquipList.Had_BigFM_hat;
            Name = panel.Title;
            EquipScore = panel.Score;
        }

        /// <summary>
        /// 从另外一个面板更新
        /// </summary>
        /// <param name="ichar"></param>
        protected void _UpdateFromIChar(InitCharacter ichar)
        {
            BaseStrength = ichar.BaseStrength;
            StrengthPercent = ichar.StrengthPercent;
            PhysicsBaseAttackPower = ichar.PhysicsBaseAttackPower;
            PhysicsFinalAttackPower = ichar.PhysicsFinalAttackPower;
            BaseWeaponDamage = ichar.BaseWeaponDamage;

            PhysicsCriticalStrike = ichar.PhysicsCriticalStrike;
            PhysicsCriticalStrikeRate = ichar.PhysicsCriticalStrikeRate;

            PhysicsCriticalPowerValue = ichar.PhysicsCriticalPowerValue;

            BaseSurplus = ichar.BaseSurplus;
            PhysicsBaseOvercome = ichar.PhysicsBaseOvercome;
            Haste = ichar.Haste;

            BaseStrain = ichar.BaseStrain;
            StrainRate = ichar.StrainRate;

            Had_BigFM_jacket = ichar.Had_BigFM_jacket;
            Had_BigFM_hat = ichar.Had_BigFM_hat;
            Name = ichar.Name;

            EquipScore = ichar.EquipScore;
        }

        public InitCharacter(XinFaCharacterPanel panel) : this()
        {
            UpdateFromXFPanel(panel);
        }

        public void UpdateFromXFPanel(XinFaCharacterPanel panel)
        {
            BaseStrength = panel.Primary.Base;
            StrengthPercent = panel.Primary.BasePercentAdd / 1024.0;

            PhysicsBaseAttackPower = panel.Attack.Base;
            PhysicsFinalAttackPower = panel.Attack.Final;
            BaseWeaponDamage = panel.MeleeWeapon.DamageAverage;

            PhysicsCriticalStrike = panel.CriticalStrike.Point;
            PhysicsCriticalStrikeRate = panel.CriticalStrike.RatePct;

            PhysicsCriticalPowerValue = panel.CriticalDamage.PanelFinal;
            BaseStrain = panel.Strain.BasePoint;
            StrainRate = panel.Strain.PercentPct;

            BaseSurplus = panel.Surplus.Final;
            PhysicsBaseOvercome = panel.Overcome.Final;
            Haste = panel.Haste.FinalPoint;

            Had_BigFM_jacket = true;
            Had_BigFM_hat = true;
            Name = panel.Name;
            EquipScore = panel.EquipScore;
        }

        #region 显示

        public IList<string> GetCatStrList()
        {
            var res = new List<string>
            {
                "人物初始属性：",
                $"{BaseStrength:F0} 力道，{PhysicsBaseAttackPower:F0} 基础攻击，{PhysicsFinalAttackPower:F0} 最终攻击，{BaseWeaponDamage:F1} 武器伤害，{BaseSurplus:F0} 破招",

                $"{PhysicsCriticalStrikeValue:P2} 会心，{PhysicsCriticalPowerValue:P2} 会效，{FinalStrainValue:P2} 无双，" +
                $"{PhysicsBaseOvercome:F0}({PhysicsBaseOvercome / CurrentLevelParams.Overcome:P2}) 破防，{Haste:F0}({Haste / CurrentLevelParams.Haste:P2}) 加速",
            };

            res.Add("");
            return res;
        }

        #endregion

    }
}