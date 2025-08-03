using JX3CalculatorShared.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Globals;

namespace JX3CalculatorShared.ViewModels
{
    /// <summary>
    /// 描述武器和腰坠复选框的VM
    /// </summary>
    public class EquipOptionSlotViewModel : ComboBoxViewModel<EquipOption>
    {
        public readonly ImmutableDictionary<string, int> Name2IndexDict; // 名称到Index的映射，用于导入导出

        public EquipOptionSlotViewModel(IEnumerable<EquipOption> data) : base(FindValidEquipOptions(data))
        {
            var dict = new Dictionary<string, int>(ItemsSource.Length);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].Name, i);
            }

            Name2IndexDict = dict.ToImmutableDictionary();
        }

        // 筛选出当前等级的选项
        public static IEnumerable<EquipOption> FindValidEquipOptions(IEnumerable<EquipOption> data)
        {
            var res = data.Where(item => item.ExpansionPackLevel >= StaticConst.CurrentLevel);
            return res;
        }


        public string Export()
        {
            return SelectedItem.Name;
        }

        // 载入
        public int Load(string name)
        {
            var res = 0;
            if (name != null && Name2IndexDict.TryGetValue(name, out var value))
            {
                SelectedIndex = value;
                res = 0;
            }
            else
            {
                SelectedIndex = 0;
                res = 1;
            }

            return res;
        }
    }
}