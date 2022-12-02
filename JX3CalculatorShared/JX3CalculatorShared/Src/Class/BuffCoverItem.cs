using CommunityToolkit.Mvvm.ComponentModel;

namespace JX3CalculatorShared.Src.Class
{
    public class BuffCoverItem : ObservableObject
    {
        public string Name { get; }

        public string DescName { get; }

        public double Normal { get; set; } = 0; // 常规覆盖率

        public double XW { get; set; } = 0; // 心无覆盖率

        public BuffCoverItem(string name, string descName)
        {
            Name = name;
            DescName = descName;
        }

        public void Reset()
        {
            Normal = 0;
            XW = 0;
        }
    }
}