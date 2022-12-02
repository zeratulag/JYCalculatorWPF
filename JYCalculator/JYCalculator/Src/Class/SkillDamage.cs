using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using JYCalculator.Src;
using JYCalculator.Src.Class;
using MiniExcelLibs.Attributes;
using System;

namespace JYCalculator.Class
{
    public class SkillDamage
    {
        #region 成员

        public readonly SkillData Data;
        public readonly Target CTarget; // 当前目标

        [ExcelIgnore] public FullCharacter FChar { get; protected set; }
        public string Name { get; }
        public string SkillName => Data.SkillName;
        public SkillDataTypeEnum Type => Data.Info.Type;
        public double Freq { get; private set; } = 0; // 频率（默认为0）

        public double DPS { get; private set; } = 0;

        // 基准伤害
        public double OrgPhysicsDmg { get; set; } = 0; // 原始外功伤害（固定部分+外功攻击部分+武器部分）
        public double OrgPDmg { get; set; } = 0; // 原始破招伤害（仅当技能是破招类才有）

        public double StdPhysicsDmg { get; set; } // 基准外功伤害

        // 目标防御
        public double FinalPDef { get; set; } // 等价外功最终防御，这里先计算B类无视防御以及比例减防御

        public double ClosingPDef { get; set; } // 结算防御，在Final的基础上考虑了A类无视防御

        public double BearPDef { get; set; } // 防御承伤率

        // 各类增伤
        public double ParaPOC { get; set; } // 外功破防增伤

        public double ParaPYS { get; set; } // 外功易伤

        public double ParaWS { get; set; } // 无双增伤
        public double ParaPDmgAdd { get; set; } // 外功伤害提高
        public double ParaNPC { get; set; } // 非侠士增伤

        public double ParaLevelCrush { get; set; } // 等级压制系数

        // 实际伤害
        public double RealPhysicsDmg { get; set; } // 实际外功伤害
        public double RealMagicDmg { get; set; } // 实际外功伤害
        public double RealDmg { get; set; } // 实际伤害
        public double RealCriticalDmg { get; set; } // 实际会心伤害

        // 会心会效
        public double CT { get; set; }
        public double CF { get; set; }
        public double Expect { get; set; } // 期望

        public double ExpectPhysicsDmg { get; set; } // 实际外功伤害期望

        public double FinalEDamage { get; set; } // 最终伤害期望
        public double RelativeDamage { get; set; } // 相对伤害

        [ExcelIgnore]
        public DamageDeriv Deriv { get; protected set; } // 求导

        #endregion

        #region 构造

        public SkillDamage(SkillData data)
        {
            Data = data;
            Name = data.Name;
        }

        public SkillDamage(SkillData data, FullCharacter fchar, Target target) : this(data)
        {
            FChar = fchar;
            CTarget = target;
        }

        // 重新设定人物面板
        public void SetFChar(FullCharacter fchar)
        {
            FChar = fchar;
        }

        #endregion

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
                OrgPDmg = Math.Max(0, FChar.PZ) * JYStaticData.fGP.XPZ;
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
            RealDmg = RealPhysicsDmg + RealMagicDmg;
        }

        // 计算圆桌期望
        public void GetETable()
        {
            CT = Math.Min(1, Data.AddCT + FChar.CT);
            CF = Math.Min(3, Data.AddCF + FChar.CF);
            Expect = CT * (CF - 1) + 1;
            RealCriticalDmg = RealDmg * Expect;
        }

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

        public void SetFreq(double freq)
        {
            Freq = freq;
        }

        public void GetDPS()
        {
            DPS = FinalEDamage * Freq;
        }

        // 计算属性收益（求导）
        public DamageDeriv CalcDeriv()
        {
            var res = new DamageDeriv(Name);
            res.Final_AP = Data.APCoef > 0 ? Data.APCoef * ExpectPhysicsDmg / OrgPhysicsDmg : 0;
            res.WP = Data.WPCoef > 0 ? Data.WPCoef * ExpectPhysicsDmg / OrgPhysicsDmg : 0;

            res.PZ = Data.Info.IsP ? JYStaticData.fGP.XPZ * ExpectPhysicsDmg / OrgPDmg : 0;

            res.Final_OC = ExpectPhysicsDmg / ParaPOC / JYStaticData.fGP.OC;
            res.WS = FinalEDamage / ParaWS / JYStaticData.fGP.WS;

            res.CF = CF < 3 ? FinalEDamage / Expect * CT / JYStaticData.fGP.CF : 0;
            res.CT = CT < 1 ? FinalEDamage / Expect * (CF - 1) / JYStaticData.fGP.CT : 0;

            res.Base_OC = res.Final_OC * (1 + FChar.OC_Percent);
            res.Base_AP = res.Final_AP * (1 + FChar.AP_Percent);

            res.GetFinal_L();
            res.GetBase_L(FChar.L_Percent);

            return res;
        }

        public void GetDeriv()
        {
            Deriv = CalcDeriv();
        }
    }
}