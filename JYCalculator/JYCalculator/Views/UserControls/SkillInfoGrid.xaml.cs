using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace JYCalculator.Views.UserControls
{
    /// <summary>
    /// SkillInfoGrid.xaml 的交互逻辑
    /// </summary>
    public partial class SkillInfoGrid : UserControl
    {
        public SkillInfoGrid()
        {
            InitializeComponent();
        }

        public string[] UnwantedProperties = {"RawBasicSkillNames", "RawAppliedRecipes", "IsPZ", "IsMultiChannel"};
        public string[] SetProperties = {"AppliedRecipes", "AppliedSkillModifiers", "SkillNameTags"};

        private void SfDataGrid_AutoGeneratingColumn(object sender, AutoGeneratingColumnArgs e)
        {
            if (UnwantedProperties.Contains(e.Column.MappingName))
            {
                e.Cancel = true; // 取消生成该列
                return;
            }
        }
    }

}