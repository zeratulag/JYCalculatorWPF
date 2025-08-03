using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3PZ.Data;
using JX3PZ.Globals;
using JYCalculator.Data;

namespace JX3CalculatorShared.ViewModels
{
    public class EquipSpecialEffectSlotViewModel : ComboBoxViewModel<EquipSpecialEffectItem>
    {
        public readonly EquipSubTypeEnum EquipSubType;

        public readonly EquipSlotEnum EquipSlot;
        public readonly int SubType;

        public readonly int Position;

        public readonly ImmutableDictionary<string, int> EIDMap; // 通过ItemID导入与导出, ItemID到Index的映射

        public EquipSpecialEffectSlotViewModel(IEnumerable<EquipSpecialEffectItem> data, int position) : base(data)
        {
            Position = position;
            EquipSlot = (EquipSlotEnum) position;
            SubType = EquipMapLib.PositionToSubType(position);
            EquipSubType = (EquipSubTypeEnum) SubType;

            EIDMap = MakeEquipIDToIndexMap().ToImmutableDictionary();
        }

        public Dictionary<string, int> MakeEquipIDToIndexMap()
        {
            var eidMap = new Dictionary<string, int>();
            for (int i = 0; i < Length; i++)
            {
                var item = ItemsSource[i];
                eidMap.Add(item.EID, i);
            }

            return eidMap;
        }

        public EquipSpecialEffectSlotViewModel(int position) : this(StaticXFData.DB.EquipSpecialEffectItems
            .GetItemsByPosition(position), position)
        {
        }

        public void LoadeByEquipID(string EID)
        {
            int idx = 0;
            if (EID != null)
            {
                EIDMap.TryGetValue(EID, out idx);
            }

            SelectedIndex = idx;
        }

        public void Reset()
        {
            SelectedIndex = 0;
        }
    }
}