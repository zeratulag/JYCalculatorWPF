using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3PZ.Class;
using JX3PZ.Globals;
using JYCalculator.Data;
using JYCalculator.DB;
using JYCalculator.Models;

namespace JX3CalculatorShared.ViewModels
{
    public class EquipSpecialEffectConfigViewModel : CollectionViewModel<EquipSpecialEffectSlotViewModel>
    {
        public readonly Dictionary<EquipSlotEnum, EquipSpecialEffectSlotViewModel> EquipSpecialEffectSlotDict;

        public readonly Dictionary<EquipSlotEnum, EquipSpecialEffectItem> SelectedItems;

        public readonly EquipSpecialEffectConfigModel Model;

        public string Header { get; set; }

        public Dictionary<string, string> Config { get; protected set; }

        public EquipSpecialEffectConfigViewModel(EquipSpecialEffectItemDB itemDb)
        {
            EquipSpecialEffectSlotDict =
                new Dictionary<EquipSlotEnum, EquipSpecialEffectSlotViewModel>(PzConst.POSITIONS);
            var vmList = new List<EquipSpecialEffectSlotViewModel>(PzConst.POSITIONS);
            foreach (int position in itemDb.ValidPositions)
            {
                var slotVM = new EquipSpecialEffectSlotViewModel(position);
                vmList.Add(slotVM);
                EquipSpecialEffectSlotDict.Add((EquipSlotEnum) position, slotVM);
            }

            Model = new EquipSpecialEffectConfigModel();
            Data = vmList.ToImmutableArray();
            SelectedItems = new Dictionary<EquipSlotEnum, EquipSpecialEffectItem>(PzConst.POSITIONS);
            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();
        }

        public EquipSpecialEffectConfigViewModel() : this(StaticXFData.DB.EquipSpecialEffectItems)
        {
        }

        protected override void _Update()
        {
            foreach (var KVP in EquipSpecialEffectSlotDict)
            {
                SelectedItems[KVP.Key] = KVP.Value.SelectedItem;
            }

            Model.UpdateInput(this);
            Model.Calc();
            UpdateHeader();
            Config = GetConfig();
        }

        public void UpdateHeader()
        {
            var validEffectItemNames = Model.ValidEquipSpecialEffectItems.Select(e => e.ItemName).ToArray();

            if (validEffectItemNames.Length == 0)
            {
                Header = "无";
            }
            else
            {
                Header = validEffectItemNames.Join(", ");
            }
        }

        public Dictionary<string, string> GetConfig()
        {
            var res = SelectedItems.ToDictionary(e => e.Key.ToString(),
                e => e.Value.EID);
            return res;
        }

        protected void _Load(IDictionary<string, string> config)
        {
            if (config == null)
            {
                ResetAll(); // 兼容旧版，如果没有数据就全部重置
                return;
            }

            foreach (var KVP in config)
            {
                Enum.TryParse<EquipSlotEnum>(KVP.Key, true, out var slot);
                if (EquipSpecialEffectSlotDict.TryGetValue(slot, out var slotVM))
                {
                    slotVM.LoadeByEquipID(KVP.Value);
                }
            }
        }

        private void ResetAll()
        {
            Data.ForEach(e => e.Reset());
        }

        public void Load(IDictionary<string, string> config)
        {
            ActionUpdateOnce(_Load, config);
        }
    }
}