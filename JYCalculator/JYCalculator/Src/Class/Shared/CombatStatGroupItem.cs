﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using Syncfusion.Windows.Controls.Input;

namespace JYCalculator.Class
{
    public class CombatStatGroupItem: CombatStatItem
    {
        public CombatStatItem[] Items { get; set; }

        public CombatStatGroupItem(CombatStatItem[] items) : base("", items.First().FightName)
        {
            var fightNames = items.Select(_ => _.FightName).ToHashSet();
            if (fightNames.Count > 1)
            {
                throw new ArgumentException("战斗统计名称不同！");
            }

            Items = SortByTotalDamage(items).Select(_ => (CombatStatItem)_).ToArray();
            FightName = fightNames.First();
            foreach (var item in items)
            {
                Num += item.Num;
                CTNum += item.CTNum;
                TotalDamage += item.TotalDamage;
                ShowNum += item.ShowNum;
            }
        }

    }
}