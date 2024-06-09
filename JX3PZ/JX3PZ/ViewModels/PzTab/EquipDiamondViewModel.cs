using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using System.Linq;

namespace JX3PZ.ViewModels
{
    public class EquipDiamondViewModel : CollectionViewModel<DiamondSlotViewModel>
    {
        // 描述装备上五行石组的VM

        public bool HasDiamond { get; private set; }
        public int DiamondSlotCount; // 五行石个数
        public int[] Levels;
        public DiamondLevelItem[] Items;
        public RelayCommand<string> OneKeyEmbedCmd { get; private set; } // 一键镶嵌

        public EquipDiamondViewModel(Equip equip) : base(DiamondSlotViewModel.GetDiamondViewModels(equip))
        {
            HasDiamond = Data.Any();
            DiamondSlotCount = Data.Length;
            PostConstruct();
        }

        public EquipDiamondViewModel(int n) : base(DiamondSlotViewModel.GetEmptyDiamondViewModels(n))
        {
            DiamondSlotCount = n;
            HasDiamond = Data.Any();
            PostConstruct();
        }

        public void PostConstruct()
        {
            Levels = new int[DiamondSlotCount];
            Items = new DiamondLevelItem[DiamondSlotCount];
            GetLevels();
            OneKeyEmbedCmd = new RelayCommand<string>(OneKeyEmbed);
            SetOutName("Diamond");
        }

        /// <summary>
        /// 一键镶嵌镶嵌
        /// </summary>
        /// <param name="l"></param>
        public void OneKeyEmbed(string l = "6")
        {
            if (int.TryParse(l, out var level))
            {
                OneKeyEmbed(level);
            }
        }

        private void _OneKeyEmbed(int level = 6)
        {
            if (level >= 0 && level <= PzConst.MAX_DIAMOND_LEVEL)
            {
                foreach (var _ in Data)
                {
                    _.Level = level;
                }
            }
        }

        public void OneKeyEmbed(int level = 6) => ActionUpdateOnce(_OneKeyEmbed, level);

        // 更换装备
        private void _ChangeEquip(Equip e)
        {
            if (e == null)
            {
                MakeEmpty();
                return;
            }

            if (e.Attributes == null)
            {
                // 可能尚未解析
                e.ParseAttrs();
            }

            if (e.Attributes == null || e.Attributes.Diamond == null)
            {
                MakeEmpty();
                return;
            }

            for (int i = 0; i < DiamondSlotCount; i++)
            {
                Data[i].ChangeItem(e.Attributes.Diamond[i]);

            }

            HasDiamond = true;
        }

        // 当未选择装备/装备不带空的时候，需要把所有的槽位置为空
        private void MakeEmpty()
        {
            var empty = DiamondTabItem.EmptyItem;
            for (int i = 0; i < DiamondSlotCount; i++)
            {
                Data[i].ChangeItem(empty);
            }

            HasDiamond = false;
        }


        public void ChangeEquip(Equip e)
        {
            ActionUpdateOnce(_ChangeEquip, e);
        }

        protected override void _Update()
        {
            GetLevels();
        }

        protected void GetLevels()
        {
            for (int i = 0; i < DiamondSlotCount; i++)
            {
                Levels[i] = Data[i].Level;
                Items[i] = Data[i].ShowItem;
            }
        }

        protected override void _RefreshCommands()
        {
        }

        public void DropDiamond()
        {
            // 取下五行石
            for (int i = 0; i < DiamondSlotCount; i++)
            {
                Data[i].Load(0);
            }
        }


        // 载入五行石孔
        protected void _Load(int[] levels)
        {
            if (levels == null)
            {
                DropDiamond();
            }
            else
            {
                for (int i = 0; i < DiamondSlotCount; i++)
                {
                    Data[i].Load(levels[i]);
                }
            }
        }

        public void Load(int[] levels)
        {
            ActionUpdateOnce<int[]>(_Load, levels);
        }

        public void Load(JBPZEquipSnapshot s)
        {
            if (s == null)
            {
                DropDiamond();
            }
            else
            {
                Load(s.embedding);
            }
        }
    }
}