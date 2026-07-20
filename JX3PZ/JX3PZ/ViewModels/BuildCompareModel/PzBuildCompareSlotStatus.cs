namespace JX3PZ.ViewModels
{
    public enum PzBuildCompareSlotStatusText
    {
        未计算,
        等待,
        计算中,
        完成,
        计算失败
    }

    public static class PzBuildCompareSlotStatusColor
    {
        public const string 完成 = "#229954";
        public const string 计算中 = "#1976D2";
        public const string 未计算 = "Gray";
        public const string 计算失败 = "#C0392B";
    }

    public static class PzBuildCompareSlotStatusTextExtension
    {
        public static string GetStatusColor(this PzBuildCompareSlotStatusText statusTextKind)
        {
            switch (statusTextKind)
            {
                case PzBuildCompareSlotStatusText.完成:
                    return PzBuildCompareSlotStatusColor.完成;
                case PzBuildCompareSlotStatusText.计算中:
                    return PzBuildCompareSlotStatusColor.计算中;
                case PzBuildCompareSlotStatusText.未计算:
                case PzBuildCompareSlotStatusText.等待:
                    return PzBuildCompareSlotStatusColor.未计算;
                default:
                    return PzBuildCompareSlotStatusColor.计算失败;
            }
        }
    }
}
