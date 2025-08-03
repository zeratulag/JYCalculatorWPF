using JX3CalculatorShared.Data;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using System.Collections.Immutable;
using JYCalculator.Class;

namespace JYCalculator.Models
{
    public class EquipSpecialEffectConfigModel : IModel
    {
        public Dictionary<EquipSlotEnum, EquipSpecialEffectItem> SelectedItems;
        public EquipSpecialEffectItem[] ValidEquipSpecialEffectItems; // 有效的装备特效
        public EquipSpecialEffectEntry[] EquipSpecialEffectEntries; // 装备特效列表
        public Dictionary<EquipSpecialEffectBaseTypeEnum, EquipSpecialEffectEntry[]> GroupedEquipSpecialEffectEntries; // 按照特效基础类型分组
        public EquipSpecialEffectConfigArg Arg { get; private set; }

        public void UpdateInput(EquipSpecialEffectConfigViewModel vm)
        {
            SelectedItems = vm.SelectedItems;
        }

        public void Calc()
        {
            FindValidEquipSpecialEffectItems();
            GroupValidEquipSpecialEffectEntries();
            MakeArg();
        }

        public void FindValidEquipSpecialEffectItems()
        {
            ValidEquipSpecialEffectItems =
                SelectedItems.Values.Where(e => e.SpecialEffectType != EquipSpecialEffectTypeEnum.None).ToArray();
            EquipSpecialEffectEntries = ValidEquipSpecialEffectItems.Select(e => e.SpecialEffectEntry).ToArray();
        }

        public void GroupValidEquipSpecialEffectEntries()
        {
            GroupedEquipSpecialEffectEntries = EquipSpecialEffectEntries.GroupBy(e => e.SpecialEffectBaseType)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }

        public EquipSpecialEffectConfigArg MakeArg()
        {
            Arg = new EquipSpecialEffectConfigArg(GroupedEquipSpecialEffectEntries);
            return Arg;
        }
    }
}