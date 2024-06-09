using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JX3CalculatorShared.Views.UserControls
{
    /// <summary>
    /// ItemDTItem.xaml 的交互逻辑
    /// </summary>
    public partial class BuffItemSmallView : UserControl
    {
        public BuffItemSmallView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RightClickCommandProperty = DependencyProperty.Register(
            nameof(RightClickCommand), typeof(ICommand), typeof(BuffItemSmallView),
            new PropertyMetadata(default(ICommand)));

        public ICommand RightClickCommand
        {
            get { return (ICommand)GetValue(RightClickCommandProperty); }
            set { SetValue(RightClickCommandProperty, value); }
        }

        private void BuffItemSmallView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 检查命令是否存在和是否可以执行
            if (RightClickCommand != null && RightClickCommand.CanExecute(e))
            {
                RightClickCommand.Execute(e);
            }
        }
    }

}