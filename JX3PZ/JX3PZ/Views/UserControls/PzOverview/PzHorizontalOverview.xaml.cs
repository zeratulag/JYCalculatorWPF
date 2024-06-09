using JX3CalculatorShared.Views;
using Microsoft.Win32;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace J3PZ.Views
{
    /// <summary>
    /// PzOverView.xaml 的交互逻辑
    /// </summary>
    public partial class PzHorizontalOverview : UserControl
    {

        public static readonly DependencyProperty ShowDPSProperty = DependencyProperty.Register(
            nameof(ShowDPS), typeof(bool), typeof(PzHorizontalOverview), new PropertyMetadata(default(bool)));

        public bool ShowDPS
        {
            get { return (bool)GetValue(ShowDPSProperty); }
            set { SetValue(ShowDPSProperty, value); }
        }

        public PzHorizontalOverview()
        {
            InitializeComponent();
        }

        public void CaptureRenderImg()
        {
            var fileName = Title_Txt.Text;
            var saveFileDialog = new SaveFileDialog
            {
                Filter = $"PNG|*.png",
                FileName = fileName,
                AddExtension = true,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                CaptureRenderImg(saveFileDialog.FileName);
            }
        }

        public void CopyRenderImg()
        {
            // 复制到剪贴板
            var res = RenderTool.GetBitmapFrame(this);
            Clipboard.SetImage(res);
        }



        private void CaptureRenderImg(string fileName)
        {
            RenderTool.SaveToPng(this, fileName);
        }
    }
}
