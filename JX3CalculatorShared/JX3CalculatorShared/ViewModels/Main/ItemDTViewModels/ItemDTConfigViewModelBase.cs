using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JX3CalculatorShared.ViewModels
{
    public class ItemDTConfigViewModelBase : CollectionViewModel<ItemDTSlotViewModel>
    {
        public ImmutableDictionary<ItemDTTypeEnum, ItemDTSlotViewModel> ItemDTSlotDict;
        public Dictionary<string, double> SAts;
        public List<ItemDT> ValidItemDts; // 存储有效的单体列表
        public CharAttrCollection SCharAttr;  // 最终有效属性
        public Dictionary<ItemDTTypeEnum, int> Config; // 存储ItemID的字典
        public NamedAttrs AttrsDesc; // 调试用
        public ImmutableArray<ItemDTSlotViewModel> ItemsSource { get; set; }
        public bool IsChecked { get; set; } = true;
        public int ValidItemDTNum { get; set; } // 有效单体数
        public string Header { get; set; }

        /// 整体是否启用
        public void OnIsCheckedChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var _ in ItemsSource)
            {
                _.IsEnabled = IsChecked;
            }
        }

        protected override void _Update()
        {
            FindValid();
            GetNamedAttrs();
            GetHead();
        }

        protected void FindValid()
        {
            ValidItemDts.Clear();
            var Buffs = new List<BaseBuff>(8);
            ValidItemDTNum = 0;

            SCharAttr = null;

            Config = ItemDTSlotDict.ToDictionary(_ => _.Key, _ => _.Value.ItemID);

            if (!IsChecked) return;
            foreach (var VM in Data)
            {
                var dt = VM.SelectedItem;
                if (dt.IsValid)
                {
                    ValidItemDts.Add(VM.SelectedItem);
                    Buffs.Add(dt.Buff);
                }
            }

            ValidItemDTNum = ValidItemDts.Count;
            if (ValidItemDTNum > 0)
            {
                var buffGroup = new BaseBuffGroup(Buffs);
                buffGroup.Calc();
                SCharAttr = buffGroup.SCharAttr;
            }
        }

        protected override void _DEBUG()
        {
            Config.TraceCat();

            if (SCharAttr == null)
            {
                Trace.WriteLine("未使用单体！");
            }
            else
            {
                SCharAttr.Values.TraceCat();
            }
            
        }

        protected void GetNamedAttrs()
        {
            var names = from _ in ValidItemDts select _.DescName;
            AttrsDesc = new NamedAttrs(names, SCharAttr);
            AttrsDesc.ParcelName("单体：");
        }

        protected void GetHead()
        {
            var names = from _ in ValidItemDts select _.ItemName;
            var res = new StringBuilder();
            int i = 0;
            foreach (var name in names)
            {

                if (i > 0)
                {
                    res.Append(", ");
                    if (i % 2 == 0)
                    {
                        res.Append('\n');
                    }
                }

                res.Append(name);
                i++;
            }

            Header = res.ToString();
        }

        public ItemDTConfigSav Export()
        {
            var res = new ItemDTConfigSav()
            {
                IsChecked = IsChecked,
                Data = ItemDTSlotDict.ToDictionary(_ => _.Key, _ => _.Value.RawID),
            };
            return res;
        }

        protected void _Load(ItemDTConfigSav sav)
        {
            IsChecked = sav.IsChecked;
            foreach (var KVP in ItemDTSlotDict)
            {
                if (sav.Data.ContainsKey(KVP.Key))
                {
                    KVP.Value.Load(sav.Data[KVP.Key]);
                }
                else
                {
                    KVP.Value.Reset();
                }
            }
        }

        public void Load(ItemDTConfigSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }
    }
}