﻿using System;
using MiniExcelLibs.Attributes;

namespace JX3CalculatorShared.Class
{
    public class CombatStatItem
    {
        public CombatStatItem(string name, string skillName)
        {
            Name = name;
            SkillName = skillName;
        }

        public string Name { get; set; }
        public string SkillName { get; set; }
        public double Num { get; set; } = 0; // 实际技能数
        public double CTNum { get; set; } // 实际会心数
        public double ShowNumMultiplier { get; protected set; } = 1; // 显示技能数倍率
        public double ShowNum { get; set; } // 显示的技能数
        public double CTRate => Num > 0 ? CTNum / Num : 0; // 会心率
        public double TotalDamage { get; set; } = 0; // 总伤害
        public double Proportion { get; set; } = 0; // 比重
        public int Rank { get; set; } = 0; // 排名
        [ExcelIgnore] public double CTRatePct => CTRate * 100;
        [ExcelIgnore] public double ProportionPct => Proportion * 100;

        public void Merge(CombatStatItem other)
        {
            if (Name != other.Name)
            {
                throw new ArgumentException("名称不同！");
            }

            if (SkillName != other.SkillName)
            {
                SkillName += $", {other.SkillName}";
            }

            Num += other.Num;
            CTNum += other.CTNum;
            TotalDamage += other.TotalDamage;
            Proportion = 0;
            Rank = 0;
            ShowNum += other.ShowNum;
        }

        public void SetShowNumMultiplier(double value)
        {
            ShowNumMultiplier = value;
            GetShowNum();
        }

        public void GetShowNum()
        {
            ShowNum = Num * ShowNumMultiplier;
        }
    }
}