using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Models;
using System.Windows.Documents;

namespace JX3PZ.ViewModels
{
    public class EquipShowBoxViewModel : ObservableObject
    {
        public readonly EquipShowBoxModel Model = new EquipShowBoxModel();
        public FlowDocument FDocument { get; private set; }

#if DEBUG
        public string FlowDocumentText =>
            new System.Windows.Documents.TextRange(FDocument.ContentStart, FDocument.ContentEnd).Text;
#endif

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
        }

        public void Update()
        {
            UpdateFrom(EquipShowVM.Head, EquipShowVM.Magic, EquipShowVM.Stone, EquipShowVM.Enhance,
                EquipShowVM.Set, EquipShowVM.Tail, EquipShowVM.Extra);
        }

        public void UpdateFrom(EquipShowHeadViewModel head, EquipShowMagicViewModel magic,
            EquipShowStoneViewModel stone,
            EquipShowEnhanceViewModel enhance, EquipShowSetViewModel set, EquipShowTailViewModel tail,
            EquipShowExtraViewModel extra)
        {
            Model.UpdateFrom(head, magic, stone, enhance, set, tail);
            AfterModelUpdate();
        }

        public void AfterModelUpdate()
        {
            OnPropertyChanged(nameof(Extra));
            FDocument = Model.FDocument;
        }

    }
}