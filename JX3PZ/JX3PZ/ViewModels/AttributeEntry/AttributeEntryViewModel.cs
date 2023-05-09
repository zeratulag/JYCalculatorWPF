namespace JX3PZ.ViewModels
{
    public class AttributeEntryViewModel
    {
        // 词条属性的VM
        public string Desc { get; protected set; }
        public string Color { get; protected set; } = "#000000";
        public AttributeEntryViewModel(string desc = "", string color = "#000000")
        {
            Desc = desc;
            Color = color;
        }
    }
}