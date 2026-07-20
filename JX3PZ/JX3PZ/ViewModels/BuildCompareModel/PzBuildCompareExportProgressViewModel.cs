using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;

namespace JX3PZ.ViewModels
{
    public class PzBuildCompareExportProgressViewModel : ObservableObject
    {
        public int TotalSlots { get; }
        public PzBuildCompareSlotViewModel CurrentSlot { get; private set; }
        [AlsoNotifyFor(nameof(OverallProgressText))]
        public int CompletedSlots { get; private set; }
        public string Message { get; private set; }
        public string OverallProgressText => $"{CompletedSlots} / {TotalSlots}";

        public PzBuildCompareExportProgressViewModel(int totalSlots,
            string message = "准备导出装备对比报告...")
        {
            TotalSlots = totalSlots;
            Message = message;
        }

        public void BeginSlot(PzBuildCompareSlotViewModel slot, int index)
        {
            CurrentSlot = slot;
            Message = $"正在计算 {slot.Label} [{index}/{TotalSlots}]";
        }

        public void CompleteSlot()
        {
            CompletedSlots++;
        }

        public void BeginExportFile()
        {
            Message = "正在写入 Excel 文件...";
            CurrentSlot = null;
        }
    }
}
