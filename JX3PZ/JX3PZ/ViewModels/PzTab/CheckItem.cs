namespace JX3PZ.ViewModels
{
    public class CheckItem
    {
        // 表示筛选器单选框的VM
        public string Name { get; }
        public string Description { get; }
        public CheckItem(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public CheckItem(string name) : this(name, name) { }
    }
}