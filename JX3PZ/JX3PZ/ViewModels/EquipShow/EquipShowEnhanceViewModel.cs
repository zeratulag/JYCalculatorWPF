using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3PZ.Data;

namespace JX3PZ.ViewModels
{
    public class EquipShowEnhanceViewModel : ObservableObject
    {
        public EnhanceAttributeEntryViewModel Enhance { get; set; }
        public EnhanceAttributeEntryViewModel BigFM { get; set; }

        public bool HasBigFMSlot { get; set; } = false;
        public bool HasBigFM { get; set; } = false;

        public EquipShowEnhanceViewModel(bool hasBigFMSlot)
        {
            HasBigFMSlot = hasBigFMSlot;
            Enhance = null;
            BigFM = null;
        }

        public void UpdateFrom(Enhance e, BigFM b)
        {
            if (e == null)
            {
                Enhance = EnhanceAttributeEntryViewModel.EmptyEnhance;
            }
            else
            {
                Enhance = e.VM;
            }

            if (b == null)
            {
                BigFM = null;
                HasBigFM = false;
            }
            else
            {
                BigFM = b.VM;
                HasBigFM = true;
            }
        }
    }
}