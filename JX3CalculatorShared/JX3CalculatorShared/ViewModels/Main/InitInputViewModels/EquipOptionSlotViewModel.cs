using JX3CalculatorShared.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.ViewModels
{
    /// <summary>
    /// 描述武器和腰坠复选框的VM
    /// </summary>
    public class EquipOptionSlotViewModel : ComboBoxViewModel<EquipOption>
    {
        public readonly ImmutableDictionary<string, int> Name2IndexDict; // 名称到Index的映射，用于导入导出

        public EquipOptionSlotViewModel(IEnumerable<EquipOption> data) : base(data)
        {
            var dict = new Dictionary<string, int>(ItemsSource.Length);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                dict.Add(ItemsSource[i].Name, i);
            }

            Name2IndexDict = dict.ToImmutableDictionary();
        }

        public string Export()
        {
            return SelectedItem.Name;
        }

        // 载入
        public int Load(string name)
        {
            var res = 0;
            if (Name2IndexDict.ContainsKey(name))
            {
                SelectedIndex = Name2IndexDict[name];
                res = 0;
            }
            else
            {
                res = 1;
            }

            return res;
        }
    }

    public class EquipOptionConfigSav
    {
        public bool JN { get; set; }
        public bool SL { get; set; }
        public string WPName { get; set; } = "";
        public string YZName { get; set; } = "";


        public EquipOptionConfigSav()
        {
        }

        public EquipOptionConfigSav(bool jn, bool sl, string wpName, string yzName)
        {
            JN = jn;
            SL = sl;
            WPName = wpName;
            YZName = yzName;
        }
    }
}