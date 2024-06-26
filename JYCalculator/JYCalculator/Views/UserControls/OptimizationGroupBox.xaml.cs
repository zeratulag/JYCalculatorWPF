﻿using JX3CalculatorShared.Utils;
using JYCalculator.Globals;
using System.Windows;
using System.Windows.Controls;

namespace JYCalculator.Views.UserControls
{
    /// <summary>
    /// OptimizationGroupBox.xaml 的交互逻辑
    /// </summary>
    public partial class OptimizationGroupBox : UserControl
    {
        public OptimizationGroupBox()
        {
            InitializeComponent();
        }

        private void CopyTextBlock(object sender, RoutedEventArgs e)
        {
            CommandTool.CopyTextBlock(sender, e);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItem_MultiZhen.IsSelected)
            {
                GlobalContext.Views.Main.DpsView.Expander_CombatStat.IsExpanded = false;
            }
        }
    }
}
