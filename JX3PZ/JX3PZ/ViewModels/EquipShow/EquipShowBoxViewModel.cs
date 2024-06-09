using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Models;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EquipShowBoxViewModel : ObservableObject
    {
        public readonly EquipShowBoxModel Model = new EquipShowBoxModel();
        public FlowDocument FDocument => Model.FDocument;
        public string XamlText => Model.XamlText;

        public readonly EquipShowViewModel EquipShowVM;

        public EquipShowExtraViewModel Extra => EquipShowVM.Extra;

        //FDocument = new FlowDocument();

        public EquipShowBoxViewModel(EquipShowViewModel equipShowVM)
        {
            EquipShowVM = equipShowVM;
        }

        public void UpdateWhenNotHasEquip()
        {
            Model.ClearDocument();
            SyncXamlText();
        }

        public void Update()
        {
            UpdateFrom(EquipShowVM.Head, EquipShowVM.Magic, EquipShowVM.Stone, EquipShowVM.Enhance,
                EquipShowVM.Set, EquipShowVM.Tail, EquipShowVM.Extra);
        }

        public void UpdateFrom(EquipShowHeadViewModel head, EquipShowMagicViewModel magic, EquipShowStoneViewModel stone,
            EquipShowEnhanceViewModel enhance, EquipShowSetViewModel set, EquipShowTailViewModel tail, EquipShowExtraViewModel extra)
        {
            Model.UpdateFrom(head, magic, stone, enhance, set, tail);
            OnPropertyChanged(nameof(Extra));
            SyncXamlText();
        }

        // 同步Model结果，触发Changed事件
        public void SyncXamlText()
        {
            OnPropertyChanged(nameof(XamlText));
        }

    }
}