using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Data;
using JX3PZ.Models;

namespace JX3PZ.ViewModels
{
    public class EquipShowStoneViewModel : ObservableObject
    {
        public bool HasStone { get; private set; } = false; //是否有五彩石
        public bool HasStoneSlot { get; set; } = false; // 是否有五彩石孔（仅有主武器才有五彩石孔）
        public Stone CStone { get; private set; }
        public int DiamondCount { get; private set; } = 0;
        public int DiamondIntensity { get; private set; } = 0;
        public int IconID { get; private set; } = -1000; // 未激活五彩石时图标位为-1000

        public bool[] IsActive; // 词条激活状态
        public ImmutableArray<StoneAttributeEntryViewModel> Entries { get; set; }

        public static ImmutableArray<StoneAttributeEntryViewModel> EmptyEntries { get; }

        public static EquipShowStoneViewModel Empty = new EquipShowStoneViewModel();

        static EquipShowStoneViewModel()
        {
            EmptyEntries = (new StoneAttributeEntryViewModel[] { StoneAttributeEntryViewModel.Empty }).ToImmutableArray();
        }

        public EquipShowStoneViewModel()
        {
            Entries = ImmutableArray<StoneAttributeEntryViewModel>.Empty;
        }

        public void ChangeStone(Stone stone)
        {
            CStone = stone;
            HasStone = CStone != null;
            if (HasStone)
            {
                Entries = stone.Attributes.GetAttributeEntryViewModels();
                IconID = stone.IconID;
            }
            else
            {
                Entries = EmptyEntries;
                IconID = -1000;
            }
            CheckActivate();
        }

        public void CheckActivate()
        {
            if (HasStone)
            {
                IsActive = CStone.Attributes.IsActive(DiamondCount, DiamondIntensity);
                for (int i = 0; i < IsActive.Length; i++)
                {
                    Entries[i].IsActive = IsActive[i];
                }
            }
        }

        public void UpdateActive(bool[] isActive)
        {
            IsActive = isActive;
            if (HasStone)
            {
                for (int i = 0; i < IsActive.Length; i++)
                {
                    Entries[i].IsActive = IsActive[i];
                }
            }
        }

        public void UpdateActive(EquipStoneModel model)
        {
            UpdateActive(model.IsActive);
        }

        /// <summary>
        /// 更新五行石计数状态
        /// </summary>
        /// <param name="diamondCount">个数</param>
        /// <param name="diamondIntensity">等级和</param>
        public void UpdateDiamond(int diamondCount, int diamondIntensity)
        {
            DiamondCount = diamondCount;
            DiamondIntensity = diamondIntensity;
            CheckActivate();
        }
    }
}