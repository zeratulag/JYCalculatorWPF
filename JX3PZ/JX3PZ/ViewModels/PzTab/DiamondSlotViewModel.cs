using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.ViewModels
{
    public class DiamondSlotViewModel : AbsViewModel
    {
        private int _level = PzStatic.DEFAULT_DIAMON_LEVEL;
        public int Level
        {
            get => _level;
            set => _level = Math.Max(Math.Min(value, PzConst.MAX_DIAMOND_LEVEL), 0);
        } // 五行石等级

        public DiamondTabItem Item { get; private set; } // 五行石词条对象
        public DiamondLevelItem ShowItem => LevelData[Level]; // 当前选中词条，注意未选中时，为0级
        public RelayCommand DropDiamondCmd { get; }

        public string Tag
        {
            get
            {
                if (Item.Tag.IsEmptyOrWhiteSpace())
                {
                    return "？？";
                }
                else
                {
                    return Item.Tag;
                }
            }
        }

        public int ID => Item.ID; // 五行石ID
        public string Desc => Item.GetDesc(Level);
        public string SimpleDesc => Item.GetSimpleDesc(Level, false);

        public ImmutableArray<DiamondLevelItem> LevelData => Item.LevelItems;

        public DiamondSlotViewModel() : base(nameof(Level))
        {
            Item = DiamondTabItem.EmptyItem;
            DropDiamondCmd = new RelayCommand(DropDiamond);
        }

        public DiamondSlotViewModel(DiamondTabItem item) : this()
        {
            Item = item;
        }

        public void ChangeItem(DiamondTabItem item)
        {
            var oldLevel = Level;
            Item = item;
            Level = oldLevel;
        }

        public DiamondSlotViewModel(int iD) : this(DiamondTabLib.Get(iD))
        {
        }

        public static DiamondSlotViewModel[] GetDiamondViewModels(Equip equip)
        {
            // 从装备中一次性生成VM集合
            if (equip == null || !equip.HasDiamond)
            {
                return Array.Empty<DiamondSlotViewModel>();
            }
            var res = from _ in equip.Attributes.Diamond select new DiamondSlotViewModel(_);
            return res.ToArray();
        }

        public static ImmutableArray<DiamondSlotViewModel> GetEmptyDiamondViewModels(int n)
        {
            // 一次性生成n个空的VM集合
            var res = new DiamondSlotViewModel[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = new DiamondSlotViewModel();
            }

            return res.ToImmutableArray();
        }

        // 取下五行石
        public void DropDiamond()
        {
            Level = 0;
        }

        public void Load(int level)
        {
            Level = level;
        }

        protected override void _Update()
        {
        }

        protected override void _RefreshCommands()
        {
        }
    }
}