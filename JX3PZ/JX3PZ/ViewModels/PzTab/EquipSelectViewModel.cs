using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.Messages;
using JX3PZ.Src;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace JX3PZ.ViewModels
{
    public class EquipSelectViewModel : AbsViewModel, IRecipient<PzEquipFilterMessage>
    {
        public int Position;
        public int SubType;
        public EquipMapItem Map;

        public ImmutableArray<Equip> Source; // 所有装备的来源
        public ICollectionView ItemsSourceView { get; private set; }
        public int SelectedIndex { get; set; } = -1;
        public Equip SelectedItem { get; set; }

        public int MaxLevel { get; set; } = PzConst.MAX_EQUIP_LEVEL;
        public int MinLevel { get; set; } = PzConst.MIN_EQUIP_LEVEL;

        public CheckItem[] AttrFilterItems { get; }
        public ObservableCollection<object> AttrFilterSelected { get; set; }

        public CheckItem[] OtherFilterItems { get; }
        public ObservableCollection<object> OtherFilterSelected { get; set; }

        public EquipFilterArg FilterArg { get; set; }

        public RelayCommand DropEquipCmd { get; }

        private bool _HandlingReceivePzEquipFilterMessage = false; // 正在处理同步

        public EquipSelectViewModel(int position) : base(nameof(SelectedItem))
        {
            Position = position;
            Map = EquipMapLib.GetEquipMapItem(position);
            SubType = Map.SubType;

            AttrFilterItems = new CheckItem[]
            {
                new CheckItem("会心"),
                new CheckItem("会效"),
                new CheckItem("破防"),
                new CheckItem("加速"),
                new CheckItem("破招"),
                new CheckItem("无双"),
            };
            AttrFilterSelected = new ObservableCollection<object>();

            OtherFilterItems = new CheckItem[]
            {
                new CheckItem("散件"),
                new CheckItem("套装"),
                new CheckItem("精简"),
                new CheckItem("无封"),
            };
            OtherFilterSelected = new ObservableCollection<object>(OtherFilterItems);

            Source = StaticPzData.Equip[SubType];
            ItemsSourceView = CollectionViewSource.GetDefaultView(Source);

            GetFilterArg();
            ItemsSourceView.Filter = (_ => CanFilter((Equip)_));

            DropEquipCmd = new RelayCommand(DropEquip);
            RegisterFilterEvents();
            WeakReferenceMessenger.Default.Register<PzEquipFilterMessage>(this);
        }


        private void RegisterFilterEvents()
        {
            // 注册筛选器事件
            AttrFilterSelected.CollectionChanged += GetFilterArg;
            OtherFilterSelected.CollectionChanged += GetFilterArg;
        }

        public void SendFilterArgIfNecessary()
        {
            if (!_HandlingReceivePzEquipFilterMessage)
            {
                WeakReferenceMessenger.Default.Send(new PzEquipFilterMessage(FilterArg));
            }
        }

        public void OnMaxLevelChanged()
        {
            GetFilterArg();
            SendFilterArgIfNecessary();
        }

        public void OnMinLevelChanged()
        {
            GetFilterArg();
            SendFilterArgIfNecessary();
        }

        public void OnFilterArgChanged()
        {
            ApplyFilter();
        }


        //public void OnSelectedItemChanged()
        //{// 当装备修改时触发事件
        //    RaiseOutputChanged();
        //}

        protected override void _Update()
        {
        }

        public void _Load(Equip e)
        {
            FilterArg = null;
            var i = Source.IndexOf(e);
            SelectedIndex = i;
        }

        public void Load(string eid)
        {
            FilterArg = null;
            var i = Source.IndexOf(_ => _.EID == eid);
            SelectedIndex = i;
        }

        public void Load(JBPZEquipSnapshot s)
        {
            if (s == null)
            {
                DropEquip();
            }
            else
            {
                Load(s.id);
            }
        }


        protected override void _RefreshCommands()
        {
        }

        public static EquipSelectViewModel[] GetVMs()
        {
            var res = new EquipSelectViewModel[EquipMapLib.MAX_POSITION + 1];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new EquipSelectViewModel(i);
            }

            return res;
        }

        public void DropEquip()
        {
            // 脱下装备
            SelectedIndex = -1;
        }

        protected void GetFilterArg(object sender, NotifyCollectionChangedEventArgs e) => GetFilterArg();

        public EquipFilterArg GetFilterArg()
        {
            FilterArg = new EquipFilterArg(this);

            return FilterArg;
        }

        public void ApplyFilter()
        {
            ItemsSourceView.Refresh();
        }

        public bool CanFilter(Equip equip)
        {
            if (equip.CanFilter(FilterArg) || equip.ID == SelectedItem?.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Receive(PzEquipFilterMessage message)
        {
            if (!_HandlingReceivePzEquipFilterMessage)
            {
                _HandlingReceivePzEquipFilterMessage = true;
                MaxLevel = message.MaxLevel;
                MinLevel = message.MinLevel;
                _HandlingReceivePzEquipFilterMessage = false;
            }
        }

    }

    public class EquipFilterArg
    {
        public readonly int MaxLevel;
        public readonly int MinLevel;
        public readonly string[] AttrFilter;
        public readonly string[] OtherFiler;

        public EquipFilterArg(int minLevel, int maxLevel, IEnumerable<object> attrFilter,
            IEnumerable<object> otherFilter)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            AttrFilter = attrFilter.Select(_ => ((CheckItem)_).Name).ToArray();
            OtherFiler = otherFilter.Select(_ => ((CheckItem)_).Name).ToArray();
        }

        public EquipFilterArg(EquipSelectViewModel vm) : this(vm.MinLevel, vm.MaxLevel, vm.AttrFilterSelected,
            vm.OtherFilterSelected)
        {
        }
    }
}