using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Data;
using JX3PZ.Views;

namespace JX3PZ.ViewModels
{
    public class EquipEmbedViewModel : AbsDependViewModel<AbsViewModel>
    {
        public EquipDiamondViewModel DiamondVM { get; }
        public EquipStoneSelectViewModel StoneSelectVM { get; }

        public EquipEmbed _View;

        public bool HasDiamond => DiamondVM.HasDiamond;
        public bool HasStoneSlot { get; private set; } // 是否有五彩石孔
        public bool ShowStone => HasStoneSlot && SelectedStone != null;
        public bool StoneSelectDrawerIsOpen { get; set; } = false;
        public Stone SelectedStone { get; set; } = null;
        public bool HasSelectedStone => SelectedStone != null; // 是否有选中的五彩石
        public int[] DiamondLevels => DiamondVM.Levels;
        public RelayCommand DropStoneCmd { get; }

        public int DiamondCount { get; set; } // 五行石总数
        public int DiamondIntensity { get; set; } // 等级总和 

        /// <summary>
        /// 构建VM
        /// </summary>
        /// <param name="n">五行石孔个数</param>
        /// <param name="hasStoneSlot">是否有五彩石</param>
        public EquipEmbedViewModel(int n, bool hasStoneSlot) : base(new[] { nameof(SelectedStone) })
        {
            DiamondVM = new EquipDiamondViewModel(n);
            StoneSelectVM = new EquipStoneSelectViewModel(hasStoneSlot)
            {
                _EmbedVM = this
            };
            HasStoneSlot = hasStoneSlot;

            SetDependVMs(DiamondVM);

            DropStoneCmd = new RelayCommand(DropStone);

            SetOutName("Embed");
        }

        private void DropStone()
        {
            StoneSelectVM.DropStone();
        }

        public void UpdateDiamond(int diamondCount, int diamondIntensity)
        {
            DiamondCount = diamondCount;
            DiamondIntensity = diamondIntensity;
        }

        protected override void _Update()
        {
        }

        protected override void _RefreshCommands()
        {
        }

    }
}