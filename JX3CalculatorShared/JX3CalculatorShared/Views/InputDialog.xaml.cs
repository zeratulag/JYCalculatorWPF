using System;
using System.Windows;

namespace JX3CalculatorShared.Views
{
    /// <summary>
    /// InputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputDialog : Window
    {
        #region 成员

        public string Answer => txtAnswer.Text;

        #endregion
        public InputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            PasteFromClipboard();
        }

        private void PasteFromClipboard()
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                txtAnswer.Text = clipboardText;
            }
        }

    }
}
