namespace JX3PZ.ViewModels
{
    public class IconAttributeEntryViewModel: AttributeEntryViewModel
    {
        // 带有图标的VM
        public string IconPath { get; set; }
        public IconAttributeEntryViewModel(string desc = "", string color = "#000000", string iconPath = null) : base(desc, color)
        {
            IconPath = iconPath;
        }
    }
}