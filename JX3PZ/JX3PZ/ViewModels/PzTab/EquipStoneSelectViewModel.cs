using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Data;
using JX3PZ.Src;
using JX3PZ.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JX3PZ.ViewModels
{
    public static class EquipStoneSelectSources
    {
        public static ImmutableArray<Stone>[] Stone; // 基于Level的分表
        public static ImmutableArray<Stone>[] UseFulStone; // 基于Level的有效五彩石分表
        public static ICollectionView[] StoneView;
        public static ICollectionView[] UseFulStonesView;
        public static bool Loaded { get; private set; } // 已完成加载

        public static void Load()
        {
            if (Loaded) return;
            Stone = StaticPzData.Stone.Stones;
            UseFulStone = StaticPzData.Stone.UsefulStones;
            StoneView = Stone.Select(_ => CollectionViewSource.GetDefaultView(_)).ToArray();
            UseFulStonesView = UseFulStone.Select(_ => CollectionViewSource.GetDefaultView(_)).ToArray();
        }

        public static bool AnyFitName(this IEnumerable<Stone> stones, string name)
        {
            return stones.Any(_ => _.FitName(name));
        }
    }


    public class EquipStoneSelectViewModel : AbsViewModel
    {
        public bool HasStoneSlot { get; private set; } // 是有有五彩石孔
        public int Level { get; set; } = 5;
        public int RealLevel => Math.Min(Math.Max(Level, 1), 6);
        public ICollectionView ItemsSourceView { get; private set; }
        public int SelectedIndex { get; set; } = -1;
        public Stone SelectedItem { get; set; } = null;

        public bool HasSelectedItem => SelectedItem != null; // 是否有选中的五彩石
        public bool Useful { get; set; } = true; // 匹配心法 
        public string FilterName { get; set; } // 文字筛选
        public static CheckItem[] AttrFilterItems { get; private set; }
        public ObservableCollection<object> AttrFilterSelected { get; set; }
        public StoneFilterArg FilterArg { get; set; }

        public EquipStoneSelect _View;
        public EquipEmbedViewModel _EmbedVM;

        public RelayCommand ConfirmSelectionCmd { get; } // 确认选择
        public RelayCommand CancelSelectionCmd { get; } // 取消选择
        public RelayCommand DropStoneCmd { get; } // 取下五彩石
        public RelayCommand SearchStoneCmd { get; } // 搜索五彩石

        private bool _AutoFilter = true; // 自动筛选

        public EquipStoneSelectViewModel(bool hasStoneSlot) : base()
        {
            HasStoneSlot = hasStoneSlot;

            if (!HasStoneSlot)
            {
                return;
            }

            AttrFilterSelected = new ObservableCollection<object>();

            ChangeSource();
            GetFilterArg();
            RegisterFilterEvents();

            ConfirmSelectionCmd = new RelayCommand(ConfirmSelection);
            CancelSelectionCmd = new RelayCommand(CancelSelection);
            SearchStoneCmd = new RelayCommand(SearchStone);
            DropStoneCmd = new RelayCommand(DropStone);
            SetOutName("Stone");
        }

        public void DropStone()
        {
            SelectedIndex = -1;
            ConfirmSelection();
        }

        public void OnFilterNameChanged()
        {
            GetFilterArg();
        }

        private void SearchStone()
        {
            // 若当前选项不满足要求，则扩大搜索范围
            if (!ItemsSourceView.IsEmpty) return;
            var level = Level;
            var filterStr = FilterName;
            var total = StringConsts.ChinaBigNumber.Length;
            for (int i = 0; i < total; i++)
            {
                var n = StringConsts.ChinaBigNumber[i];
                if (filterStr.Contains(n))
                {
                    level = i;
                    if (EquipStoneSelectSources.UseFulStone[level].AnyFitName(FilterArg.Name))
                    {
                        _AutoFilter = false;
                        Level = i;
                        AttrFilterSelected.Clear();

                        ChangeSource();
                        ApplyFilter();
                        _AutoFilter = true;
                    }
                    else if (EquipStoneSelectSources.Stone[level].AnyFitName(FilterArg.Name))
                    {
                        _AutoFilter = false;
                        Level = i;
                        AttrFilterSelected.Clear();
                        Useful = false;
                        Task.Run(ChangeSource);
                        ApplyFilter();
                        _AutoFilter = true;
                    }
                }
            }
        }

        private void RegisterFilterEvents()
        {
            // 注册筛选器事件
            AttrFilterSelected.CollectionChanged += GetFilterArg;
        }

        private bool CanFilter(Stone s)
        {
            if (s.CanFilter(FilterArg))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void GetFilterArg(object sender, NotifyCollectionChangedEventArgs e) => GetFilterArg();

        public StoneFilterArg GetFilterArg()
        {
            FilterArg = new StoneFilterArg(this);
            return FilterArg;
        }

        public void ApplyFilter()
        {
            ItemsSourceView.Refresh();
        }


        public void ChangeSource()
        {
            // 当等级或是心法筛选改变时，直接换源

            if (Useful)
            {
                ItemsSourceView = EquipStoneSelectSources.UseFulStonesView[RealLevel];
            }
            else
            {
                ItemsSourceView = EquipStoneSelectSources.StoneView[RealLevel];
            }

            ItemsSourceView.Filter = (_ => CanFilter((Stone)_));
        }

        public void OnUsefulChanged()
        {
            if (_AutoFilter) Task.Run(ChangeSource);
        }

        public void OnLevelChanged()
        {
            if (_AutoFilter) ChangeSource();
        }

        public void OnFilterArgChanged()
        {
            if (_AutoFilter) ApplyFilter();
        }

        //public void OnFilterTxtChanged()
        //{
        //    GetFilterArg();
        //}

        public void OnSelectedItemChanged()
        {
        }

        protected override void _Update()
        {
        }

        protected override void _RefreshCommands()
        {
        }

        public void ConfirmSelection()
        {
            _EmbedVM.SelectedStone = SelectedItem;
            RaiseOutputChanged();
            _EmbedVM.StoneSelectDrawerIsOpen = false;
        }

        public void CancelSelection()
        {
            _EmbedVM.StoneSelectDrawerIsOpen = false;
        }

        public void Load(Stone s)
        {
            _EmbedVM.SelectedStone = s;
            if (s != null)
            {
                FilterName = s.Name;
                SearchStone();
            }
        }

        public void Load(int stoneId)
        {
            Stone s = null;
            StaticPzData.Data.Stone.TryGetValue(stoneId, out s);
            Load(s);
        }

        public void Load(JBPZEquipSnapshot s)
        {
            if (s == null)
            {
                DropStone();
            }
            else
            {
                Load(s.stone ?? 0);
            }
        }


        // 基于ID获得筛选器
        public static List<CheckItem> CreateFilterItems(List<string> fullIDs)
        {
            var res = new List<CheckItem>(fullIDs.Count);
            foreach (var _ in fullIDs)
            {
                var at = AttributeIDLoader.GetAttributeID(_);
                var item = new CheckItem(_, at.SimpleDesc);
                res.Add(item);
            }

            return res;
        }

        public static void SetFilterItems(List<string> fullIDs)
        {
            AttrFilterItems = CreateFilterItems(fullIDs).ToArray();
        }
    }


    public class StoneFilterArg
    {
        //public readonly int Level;
        public readonly string[] AttrFilter;
        public readonly string Name;

        public StoneFilterArg(string name, IEnumerable<object> attrFilter)
        {
            Name = name;
            AttrFilter = attrFilter.Select(_ => ((CheckItem)_).Name).ToArray();
        }

        public StoneFilterArg(EquipStoneSelectViewModel vm) : this(vm.FilterName, vm.AttrFilterSelected)
        {
        }


        public bool HasFit(IEnumerable<Stone> stones)
        {
            // 判断一组五彩石中是否有满足条件的
            return stones.Any(_ => _.CanFilter(this));
        }
    }
}