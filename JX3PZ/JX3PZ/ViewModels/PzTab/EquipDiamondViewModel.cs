using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using Syncfusion.Windows.Tools.Controls;

namespace JX3PZ.ViewModels
{
    public class EquipDiamondViewModel : CollectionViewModel<DiamondSlotViewModel>
    {
        // 描述装备上五行石组的VM

        public bool HasDiamond { get; private set; }
        public int DiamondNumber; // 五行石个数
        public int[] Levels;
        public DiamondLevelItem[] Items;
        public RelayCommand<string> OneKeyEmbedCmd { get; private set; } // 一键镶嵌

        public EquipDiamondViewModel(Equip equip) : base(DiamondSlotViewModel.GetDiamondViewModels(equip))
        {
            HasDiamond = Data.Any();
            DiamondNumber = Data.Length;
            PostConstruct();
        }

        public EquipDiamondViewModel(int n) : base(DiamondSlotViewModel.GetEmptyDiamondViewModels(n))
        {
            DiamondNumber = n;
            HasDiamond = Data.Any();
            PostConstruct();
        }

        public void PostConstruct()
        {
            Levels = new int[DiamondNumber];
            Items = new DiamondLevelItem[DiamondNumber];
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
            for (int i = 0; i < DiamondNumber; i++)
            {
                if (e == null)
                {
                    Data[i].ChangeItem(DiamondTabItem.EmptyItem);
                }
                else
                {
                    Data[i].ChangeItem(e.Attributes.Diamond[i]);
                }
            }
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
            for (int i = 0; i < DiamondNumber; i++)
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
            for (int i = 0; i < DiamondNumber; i++)
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
                for (int i = 0; i < DiamondNumber; i++)
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