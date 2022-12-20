using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace JX3CalculatorShared.Utils
{
    public static class CommandTool
    {
        public static void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        public static void OpenDictUrl(IDictionary<string, string> dict, string key)
        {
            var url = dict.GetValueOrUseDefault(key, "");
            if (url != "")
            {
                OpenUrl(url);
            }
        }

        /// <summary>
        /// 复制TextBlock的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CopyTextBlock(object sender, RoutedEventArgs e)
        {
            var obj = (TextBlock)sender;
            Clipboard.SetText(obj.Text);
        }
    }
}