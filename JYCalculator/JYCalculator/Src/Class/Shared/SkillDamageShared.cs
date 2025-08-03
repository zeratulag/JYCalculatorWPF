using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JYCalculator.Globals;
using MiniExcelLibs.Attributes;
using System;
using System.Net.NetworkInformation;

namespace JYCalculator.Class
{
    public partial class SkillDamage
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

        public double OrgPhysicsDamage { get; set; } = 0; // 原始外功伤害（固定部分+外功攻击部分+武器部分）
        public double OrgSurplusDamage { get; set; } = 0; // 原始破招伤害（仅当技能是破招类才有）
        public double StdPhysicsDmg { get; set; } // 基准外功伤害

        // 目标防御
        public double FinalPhysicsDef { get; set; } // 等价外功最终防御，这里先计算B类无视防御以及比例减防御
        public double ClosingPhysicsDef { get; set; } // 结算防御，在Final的基础上考虑了A类无视防御
        public double BearPhysicsDef { get; set; } // 防御承伤率

        // 各类增伤
        public double ParaPhysicsFinalOvercomeValue { get; set; } // 外功破防增伤
        public double ParaPhysicsDamageCoefficient { get; set; } // 外功易伤
        public double ParaFinalStrainValue { get; set; } // 无双增伤
        public double ParaPhysicsDamageAdd { get; set; } // 外功伤害提高
        public double ParaNPC_Coef { get; set; } // 非侠士增伤
        public double ParaLevelCrush { get; set; } // 等级压制系数
        public double RealPhysicsDamage { get; set; } // 实际外功伤害
        public double RealDamage { get; set; } // 实际伤害
        public double RealCriticalDamage { get; set; } // 实际会心伤害
        public double CriticalStrikeValue { get; set; } // 会心会效
        public double CriticalPowerValue { get; set; }
        public double ExpectValue { get; set; } // 期望
        public double ExpectPhysicsDmg { get; set; } // 实际外功伤害期望
        public double FinalExpectDamage { get; set; } // 最终伤害期望

        public double RelativeDamage { get; set; } // 相对伤害

        [ExcelIgnore] public DamageDeriv Deriv { get; protected set; } // 求导

        public bool IsSuperCustom { get; }
        public bool IsPiaoHuang130 { get; } // 是否为飘黄130
        public int PiaoHuangStack { get; set; } // 飘黄层数

        #endregion

        #region 构造

        public SkillDamage(SkillData data)
        {
            Data = data;
            Name = data.Name;
            IsPiaoHuang130 = Data.Name == SkillKeyConst.逐云寒蕊_130;
            IsSuperCustom = Data.Info.Type == SkillDataTypeEnum.SuperCustom;
        }

        public SkillDamage(SkillData data, FullCharacter fchar, Target target) : this(data)
        {
            FChar = fchar;
            CTarget = target;
        }


        public void SetFChar(FullCharacter fchar)
        {
            // 重新设定人物面板
            FChar = fchar;
        }

        #endregion

        public void GetETable()
        {
            if (IsSuperCustom)
            {
                CriticalStrikeValue = 0;
                CriticalPowerValue = StaticConst.CriticalDamageStart;
                ExpectValue = 1;
            }
            else
            {
                CriticalStrikeValue = Math.Min(1, Data.AddCriticalStrikeRate + FChar.PhysicsCriticalStrikeValue);
                CriticalPowerValue = Math.Max(StaticConst.CriticalDamageStart,
                    Math.Min(StaticConst.CriticalDamageMax,
                        Data.AddCriticalPowerRate + FChar.PhysicsCriticalPowerValue));
                ExpectValue = CriticalStrikeValue * (CriticalPowerValue - 1) + 1;
            }

            RealCriticalDamage = RealDamage * ExpectValue;
        }

        public void SetFreq(double freq)
        {
            Freq = freq;
        }

        public void GetDPS()
        {
            DPS = FinalExpectDamage * Freq;
        }

        public void GetDeriv()
        {
            Deriv = CalcDeriv();
        }
    }
}