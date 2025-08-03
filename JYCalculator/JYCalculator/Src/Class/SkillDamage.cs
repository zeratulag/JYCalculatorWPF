using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using System;
using JYCalculator.Src.Class;

namespace JYCalculator.Class
{
    public partial class SkillDamage
    {
        #region 方法

        // 计算伤害
        public void GetDamage()
        {
            GetStdDamage();
            GetDef();
            GetParams();
            GetRealDamage();
            GetETable();
            GetFinalDmg();
        }

        // 计算基准伤害
        public void GetStdDamage()
        {
            if (IsPiaoHuang130)
            {
                GetPiaoHuang130Damage();
            }
            else
            {
                GetNormalStdDamage();
            }

            StdPhysicsDmg = OrgPhysicsDamage + OrgSurplusDamage;
        }


        // 计算常规技能（非飘黄）的基准伤害
        public void GetNormalStdDamage()
        {
            OrgPhysicsDamage = Data.WeaponDamageCoef * FChar.BaseWeaponDamage +
                               Data.APCoef * FChar.PhysicsFinalAttackPower + Data.Info.Fixed_Dmg; // 攻击力部分
            OrgSurplusDamage = Math.Max(0, FChar.BaseSurplus) * Data.PZCoef; // 破招部分
        }

        public void GetPiaoHuang130Damage()
        {
            OrgPhysicsDamage = PiaoHuangDamage.CalcDamage(FChar.EquipScore, PiaoHuangStack);
            OrgSurplusDamage = 0;
        }

        #endregion

        // 计算最终防御
        public void GetDef()
        {
            FinalPhysicsDef = Math.Max(CTarget.PhysicsFinalShield - CTarget.PhysicsBaseShield * Data.IgnoreB, 0);

            ClosingPhysicsDef = FinalPhysicsDef * Math.Max(0, 1 - FChar.AllShieldIgnore);

            BearPhysicsDef = IsSuperCustom ? 1.0 : CTarget.GetBearDef(ClosingPhysicsDef); // 最终外功承伤率
        }

        // 计算各类增伤
        public void GetParams()
        {
            ParaPhysicsDamageCoefficient = CTarget.PhysicsDamageCoefficient;
            ParaLevelCrush = DamageTool.LevelCrushCoef(CTarget.Level);

            if (IsSuperCustom)
            {
                ParaPhysicsFinalOvercomeValue = 1.0;
                ParaPhysicsDamageAdd = 1.0;
                ParaFinalStrainValue = 1.0;
                ParaNPC_Coef = 1.0;
            }
            else
            {
                ParaPhysicsFinalOvercomeValue = 1 + FChar.PhysicsFinalOvercomeValue;
                ParaPhysicsDamageAdd = 1 + FChar.PhysicsDamageAdd + Data.AddDamage;
                ParaFinalStrainValue = 1 + FChar.FinalStrainValue;
                ParaNPC_Coef = 1 + FChar.NPC_Coef + Data.AddNPC_Coef;
            }
        }

        // 计算实际伤害
        public void GetRealDamage()
        {
            var otherParas = ParaFinalStrainValue * ParaNPC_Coef * ParaLevelCrush; // 除了破防之外的其他系数之积
            RealPhysicsDamage = StdPhysicsDmg * BearPhysicsDef * ParaPhysicsFinalOvercomeValue *
                                ParaPhysicsDamageCoefficient * ParaPhysicsDamageAdd * otherParas;
            RealDamage = RealPhysicsDamage;
        }

        // 计算圆桌期望

        // 计算最终期望伤害
        public void GetFinalDmg()
        {
            ExpectPhysicsDmg = RealPhysicsDamage * ExpectValue;
            FinalExpectDamage = ExpectPhysicsDmg;
        }

        // 获取相对伤害
        public void CalcRelativeDamage(double baseline)
        {
            RelativeDamage = FinalExpectDamage / baseline * 100;
        }

        // 计算破招伤害，注意破招可能为0，所以需要单独计算
        public double CalcBaseSurplusDeriv()
        {
            double res = 0;
            if (Data.PZCoef == 0)
            {
                return 0;
            }

            if (OrgSurplusDamage > 0)
            {
                res = Data.PZCoef * ExpectPhysicsDmg / OrgSurplusDamage; // 快速算法
            }
            else
            {
                var otherParas = ParaFinalStrainValue * ParaNPC_Coef * ParaLevelCrush; // 除了破防之外的其他系数之积
                var paras = BearPhysicsDef * ParaPhysicsFinalOvercomeValue *
                            ParaPhysicsDamageCoefficient * ParaPhysicsDamageAdd * otherParas * ExpectValue;
                res = Data.PZCoef * paras;
            }

            return res;
        }

        // 计算属性收益（求导）
        public DamageDeriv CalcDeriv()
        {
            var res = new DamageDeriv(Name);

            res.PhysicsFinalAttackPower = Data.APCoef > 0 ? Data.APCoef * ExpectPhysicsDmg / OrgPhysicsDamage : 0;
            res.BaseWeaponDamage = Data.WeaponDamageCoef > 0
                ? Data.WeaponDamageCoef * ExpectPhysicsDmg / OrgPhysicsDamage
                : 0;
            res.BaseSurplus = CalcBaseSurplusDeriv();

            if (IsSuperCustom)
            {
                res.PhysicsFinalOvercome = 0;
                res.FinalStrain = 0;
                res.PhysicsCriticalPower = 0;
                res.PhysicsCriticalStrike = 0;
            }
            else
            {
                res.PhysicsFinalOvercome = ExpectPhysicsDmg / ParaPhysicsFinalOvercomeValue /
                                           XFStaticConst.CurrentLevelParams.Overcome;
                res.FinalStrain = FinalExpectDamage / ParaFinalStrainValue / XFStaticConst.CurrentLevelParams.Strain;
                res.PhysicsCriticalPower = CriticalPowerValue < 3
                    ? FinalExpectDamage / ExpectValue * CriticalStrikeValue /
                      XFStaticConst.CurrentLevelParams.CriticalPower
                    : 0;
                res.PhysicsCriticalStrike = CriticalStrikeValue < 1
                    ? FinalExpectDamage / ExpectValue * (CriticalPowerValue - 1) /
                      XFStaticConst.CurrentLevelParams.CriticalStrike
                    : 0;
            }

            res.PhysicsBaseOvercome = res.PhysicsFinalOvercome * (1 + FChar.PhysicsOvercomePercent);
            res.PhysicsBaseAttackPower = res.PhysicsFinalAttackPower * (1 + FChar.PhysicsAttackPowerPercent);
            res.BaseStrain = res.FinalStrain * (1 + FChar.StrainPercent);

            res.GetFinalStrength();
            res.GetBaseStrength(FChar.StrengthPercent);

            return res;
        }
    }
}