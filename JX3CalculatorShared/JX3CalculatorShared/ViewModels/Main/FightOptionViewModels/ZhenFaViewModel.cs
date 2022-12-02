using System.Collections.Generic;
using System.Collections.Immutable;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Src.Class;

namespace JX3CalculatorShared.ViewModels 
{
    public class ZhenFaViewModel : ComboBoxViewModel<ZhenFa>
    {

        #region 成员

        public readonly ImmutableDictionary<string, int> Item2Index;
        public BaseBuffGroup EmitedBaseBuffGroup => SelectedItem.EmitedBaseBuffGroup; // 输出的汇总
        public NamedAttrs AttrsDesc => SelectedItem.AttrsDesc;

        #endregion


        public ZhenFaViewModel(IEnumerable<ZhenFa> data) : base(data)
        {
            var dict = new Dictionary<string, int>(ItemsSource.Length);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].Name, i);
            }

            Item2Index = dict.ToImmutableDictionary();
        }



        protected override void _Update()
        {
        }


        #region 导入导出

        public string Export()
        {
            return SelectedItem.Name;
        }

        public void Load(string name)
        {
            SelectedIndex = Item2Index.GetValueOrDefault(name, 0);
        }

        #endregion


    }
}