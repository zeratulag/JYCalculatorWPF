using JX3CalculatorShared.Class;

namespace JX3CalculatorShared.ViewModels
{
    public class ItemDTViewModel
    {
        public ItemDT ItemDT { get; }
        public ProfitItemInfo ItemDTProfit { get; } = new ProfitItemInfo();

        public ItemDTViewModel(ItemDT itemDT)
        {
            ItemDT = itemDT;
        }

        public string ToolTip => ItemDT.ToolTip;
    }
}
