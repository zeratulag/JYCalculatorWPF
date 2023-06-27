using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JX3CalculatorShared.Views.UserControls
{
    /// <summary>
    /// DPSView.xaml 的交互逻辑
    /// </summary>
    public partial class DPSView : UserControl
    {
        public static readonly DependencyProperty FinalDPSProperty = DependencyProperty.Register(
            nameof(FinalDPS), typeof(double), typeof(DPSView), new PropertyMetadata(default(double)));

        public double FinalDPS
        {
            get { return (double) GetValue(FinalDPSProperty); }
            set { SetValue(FinalDPSProperty, value); }
        }

        public DPSView()
        {
            InitializeComponent();
        }

        private void CopyFinalDPS(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(String.Format("{0:F2}", FinalDPS));
            CopyTextblock_pop.IsOpen = true;
        }
    }
}
