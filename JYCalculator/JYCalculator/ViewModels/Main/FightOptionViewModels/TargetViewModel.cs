using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Src.Class;

namespace JYCalculator.ViewModels 
{
    public class TargetViewModel : ComboBoxViewModel<Target>
    {
        public readonly ImmutableDictionary<string, int> Item2Index;
        public TargetViewModel(IEnumerable<Target> data) : base(data)
        {

            var dict = new Dictionary<string, int>(ItemsSource.Length);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].DescName, i);
            }
            Item2Index = dict.ToImmutableDictionary();
            SelectedIndex = Length - 1;
        }


        public string Export()
        {
            return SelectedItem.DescName;
        }

        public void Load(string name)
        {
            SelectedIndex = Item2Index.GetValueOrDefault(name, 0);
        }
    }
}