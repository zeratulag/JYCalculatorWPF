using System.Windows;
using System.Windows.Controls;

namespace JX3PZ.Views
{
    /// <summary>
    /// EquipShowBox.xaml 的交互逻辑
    /// </summary>
    public partial class EquipShowBox : UserControl
    {
        //public EquipShowBoxViewModel ViewModel => (EquipShowBoxViewModel) DataContext;

        public static readonly DependencyProperty XamlTextProperty = DependencyProperty.Register(
            "XamlText",
            typeof(string),
            typeof(EquipShowBox),
            new PropertyMetadata(default(string), OnXamlTextChanged));

        // 依赖属性的CLR属性包装器
        public string XamlText
        {
            get { return (string)GetValue(XamlTextProperty); }
            set { SetValue(XamlTextProperty, value); }
        }

        // 属性改变时的回调方法
        private static void OnXamlTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EquipShowBox control = (EquipShowBox)d;
            control.UpdateXamlText();
        }

        // 当XamlText属性改变时，将调用此方法
        private void UpdateXamlText()
        {
            //// 在这里实现对新值的处理
            //// 例如，你可以更新控件的显示或处理新的XAML文本
            //// 这里只是打印到控制台作为示例
            //Trace.WriteLine($"XamlText updated: {XamlText}");
            ////var res = (FlowDocument) FlowDocumentTool.ConvertXamlText(XamlText);
        }


        public EquipShowBox()
        {
            InitializeComponent();
        }

    }
}
