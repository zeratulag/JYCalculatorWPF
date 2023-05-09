using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using System;

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
            OrgPhysicsDmg = Data.WPCoef * FChar.WP + Data.APCoef * FChar.Final_AP + Data.Info.Fixed_Dmg;
            if (Type == SkillDataTypeEnum.PZ)
            {
                OrgPDmg = Math.Max(0, FChar.PZ) * XFStaticConst.fGP.XPZ;
            }

            StdPhysicsDmg = OrgPhysicsDmg + OrgPDmg;
        }

        #endregion

        // 计算最终防御
        public void GetDef()
        {
            FinalPDef = Math.Max(CTarget.Final_PDef - CTarget.Base_PDef * Data.IgnoreB, 0);

            ClosingPDef = FinalPDef * Math.Max(0, 1 - FChar.IgnoreA);

            BearPDef = CTarget.GetBearDef(ClosingPDef); // 最终外功承伤率

        }

        // 计算各类增伤
        public void GetParams()
        {
            ParaPOC = 1 + FChar.Final_OC_Pct;
            ParaPYS = 1 + CTarget.P_YS;

            ParaPDmgAdd = 1 + FChar.DmgAdd + Data.AddDmg;
            ParaWS = 1 + FChar.WS;
            ParaNPC = 1 + FChar.NPC_Coef;

            ParaLevelCrush = DamageTool.LevelCrushCoef(CTarget.Level);
        }

        // 计算实际伤害
        public void GetRealDamage()
        {
            var OtherParas = ParaWS * ParaNPC * ParaLevelCrush; // 除了破防之外的其他系数之积
            RealPhysicsDmg = StdPhysicsDmg * BearPDef * ParaPOC * ParaPYS * ParaPDmgAdd * OtherParas;
            RealDmg = RealPhysicsDmg;
        }

        // 计算圆桌期望

        // 计算最终期望伤害
        public void GetFinalDmg()
        {
            ExpectPhysicsDmg = RealPhysicsDmg * Expect;
            FinalEDamage = ExpectPhysicsDmg;
        }

        // 获取相对伤害
        public void CalcRelativeDamage(double baseline)
        {
            RelativeDamage = FinalEDamage / baseline * 100;
        }

        // 计算属性收益（求导）
        public DamageDeriv CalcDeriv()
        {
            var res = new DamageDeriv(Name);
            res.Final_AP = Data.APCoef > 0 ? Data.APCoef * ExpectPhysicsDmg / OrgPhysicsDmg : 0;
            res.WP = Data.WPCoef > 0 ? Data.WPCoef * ExpectPhysicsDmg / OrgPhysicsDmg : 0;

            res.PZ = Data.Info.IsP ? XFStaticConst.fGP.XPZ * ExpectPhysicsDmg / OrgPDmg : 0;

            res.Final_OC = ExpectPhysicsDmg / ParaPOC / XFStaticConst.fGP.OC;
            res.WS_Point = FinalEDamage / ParaWS / XFStaticConst.fGP.WS;

            res.CF_Point = CF < 3 ? FinalEDamage / Expect * CT / XFStaticConst.fGP.CF : 0;
            res.CT_Point = CT < 1 ? FinalEDamage / Expect * (CF - 1) / XFStaticConst.fGP.CT : 0;

            res.Base_OC = res.Final_OC * (1 + FChar.OC_Percent);
            res.Base_AP = res.Final_AP * (1 + FChar.AP_Percent);

            res.GetFinal_L();
            res.GetBase_L(FChar.L_Percent);

            return res;
        }
    }
}