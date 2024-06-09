using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.ViewModels;
using JX3PZ.Data;
using JX3PZ.Messages;
using JX3PZ.Models;
using System.ComponentModel;

namespace JX3PZ.ViewModels
{
    public class PzTabItemViewModel : AbsViewModel
    {
        public readonly EquipMapItem Map;
        public EquipSelectViewModel EquipSelectVM { get; }
        public EquipEnhanceViewModel EquipEnhanceVM { get; }

        public EquipEmbedViewModel EquipEmbedVM { get; }
        public EquipDiamondViewModel EquipDiamondVM => EquipEmbedVM.DiamondVM;

        public EquipStoneSelectViewModel EquipStoneSelectVM => EquipEmbedVM.StoneSelectVM;

        // 总结
        public EquipSnapShotModel EquipSnapShot { get; }

        public EquipShowViewModel EquipShowVM { get; }

        public EquipShowSetViewModel EquipShowSetVM
        {
            get => EquipShowVM.Set;
            set => EquipShowVM.Set = value;
        }

        public PzEquipSummaryViewModel Summary { get; }

        public string Header { get; set; }
        public int Position { get; set; }
        public int DiamondCount { get; } // 有几个镶嵌孔
        //public bool _SendMessage { get; private set; } = true; // 是否对外发消息

        public PzTabItemViewModel(int position)
        {
            Position = position;
            Map = EquipMapLib.GetEquipMapItem(position);
            DiamondCount = Map.DiamondSlotCount;
            Header = Map.Label;
            EquipSelectVM = new EquipSelectViewModel(position);
            EquipEnhanceVM = new EquipEnhanceViewModel(position);
            EquipEmbedVM = new EquipEmbedViewModel(DiamondCount, Map.SubTypeEnum == EquipSubTypeEnum.PRIMARY_WEAPON);

            EquipSelectVM.OutputChanged += ChangeSelectedEquip;
            EquipDiamondVM.OutputChanged += EquipSnapShotInputChanged;
            EquipEnhanceVM.OutputChanged += EquipSnapShotInputChanged;
            EquipStoneSelectVM.OutputChanged += EquipSnapShotInputChanged;

            EquipSnapShot = new EquipSnapShotModel(position);
            EquipShowVM = new EquipShowViewModel();

            Summary = new PzEquipSummaryViewModel(position);

            _SendMessage = false;
            _Update();
            _SendMessage = true;

        }

        private void _ChangeSelectedEquip()
        {
            // 同步当前已选中附魔信息
            EquipEnhanceVM._RaiseOutChanged = false;
            EquipEnhanceVM.ChangeEquip(EquipSelectVM.SelectedItem);
            EquipEnhanceVM._RaiseOutChanged = true;

            EquipDiamondVM._RaiseOutChanged = false;
            EquipDiamondVM.ChangeEquip(EquipSelectVM.SelectedItem);
            EquipDiamondVM._RaiseOutChanged = true;

            _Update();
        }

        protected void ChangeSelectedEquip()
        {
            // 同步当前已选中附魔信息
            DisableAutoUpdate();
            _ChangeSelectedEquip();
            EnableAutoUpdate();
            if (_SendMessage)
            {
                PzStaticMessager.Send(PzChangedEnum.Equip);
            }
        }

        public void ChangeSelectedEquip(object sender, PropertyChangedEventArgs e)
        {
            ChangeSelectedEquip();
        }

        /// <summary>
        /// 当装备、镶嵌，附魔改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EquipSnapShotInputChanged(object sender, PropertyChangedEventArgs e)
        {
            _Update();

            if (!_SendMessage) return;

            if (e.PropertyName == EquipEnhanceVM._OutChangedArgPropertyName)
            {
                PzStaticMessager.Send(PzChangedEnum.Enhance);
                return;
            }

            if (e.PropertyName == EquipDiamondVM._OutChangedArgPropertyName)
            {
                PzStaticMessager.Send(PzChangedEnum.Diamond);
                return;
            }

            if (e.PropertyName == EquipStoneSelectVM._OutChangedArgPropertyName)
            {
                PzStaticMessager.Send(PzChangedEnum.Stone);
                return;
            }
        }

        public void UpdatePzSummary()
        {
            Summary.EquipShowVM = EquipShowVM.GetCopyWithOutExtra();
        }

        protected override void _Update()
        {
            EquipSnapShot.UpdateFrom(this);
            EquipShowVM.UpdateFrom(EquipSnapShot);
            Summary.UpdateFrom(EquipSnapShot);
        }

        protected void _Load(JBPZEquipSnapshot s)
        {
            EquipSelectVM.Load(s);
            EquipEnhanceVM.Load(s);
            EquipDiamondVM.Load(s);
            EquipStoneSelectVM.Load(s);
        }

        public void Load(JBPZEquipSnapshot s)
        {
            _SendMessage = false;
            ActionUpdateOnce(_Load, s);
            _SendMessage = true;
        }

        protected override void _RefreshCommands()
        {
        }
    }
}