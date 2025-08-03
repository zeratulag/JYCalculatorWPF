using System;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using JX3CalculatorShared.Utils;

namespace JYCalculator.Class
{
    public class EquipSpecialEffectConfigArg
    {
        public EquipSpecialEffectEntry[] SuperCustomDamage { get; }
        public EquipSpecialEffectEntry[] AddBuff { get; }
        public EquipSpecialEffectEntry[] RandomBuff { get; }
        public EquipSpecialEffectEntry[] AdaptiveBuff { get; }

        public EquipSpecialEffectConfigArg(
            IDictionary<EquipSpecialEffectBaseTypeEnum, EquipSpecialEffectEntry[]> groupedItems)
        {
            SuperCustomDamage = groupedItems.GetValueOrUseDefault(EquipSpecialEffectBaseTypeEnum.SuperCustomDamage, Array.Empty<EquipSpecialEffectEntry>());
            AddBuff = groupedItems.GetValueOrUseDefault(EquipSpecialEffectBaseTypeEnum.AddBuff, Array.Empty<EquipSpecialEffectEntry>());
            RandomBuff = groupedItems.GetValueOrUseDefault(EquipSpecialEffectBaseTypeEnum.RandomBuff, Array.Empty<EquipSpecialEffectEntry>());
            var adaptiveBuff = groupedItems.GetValueOrUseDefault(EquipSpecialEffectBaseTypeEnum.AdaptiveBuff, Array.Empty<EquipSpecialEffectEntry>());
            AdaptiveBuff = adaptiveBuff.SortAdaptiveBuffEntriesByCalcOrder();
        }
    }
}