using JX3CalculatorShared.Class;
using JX3CalculatorShared.Views;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace JX3CalculatorShared.ViewModels
{
    public static class ImportJX3BOXViewModel
    {
        public static (bool Success, JBBB J) TryReadFromDialog()
        {
            // 尝试打开窗口，读取字符串，并且转换为JBBB形式
            bool end = false;
            JBBB j = null;
            bool success = false;
            string jbString;

            while (!end)
            {
                ImportJBDialog dialog = new ImportJBDialog("");
                if (dialog.ShowDialog() == true)
                {
                    jbString = dialog.Answer;
                    var res = JBBB.TryReadFromJSON(jbString);
                    if (res.Success)
                    {
                        end = true;
                        success = true;
                        j = res.J;
                    }
                    else
                    {
                        MessageBoxButton buttons = MessageBoxButton.OKCancel;
                        var result = MessageBox.Show("数据格式有误！", "错误", buttons, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.Cancel)
                        {
                            end = true;
                            success = false;
                        }
                    }
                }
                else
                {
                    end = true;
                }

                if (end)
                {
                    //if (success)
                    //{
                    //    Growl.Success("数据导入成功");
                    //}

                    return (success, j);
                }
            }

            return (success, j);
        }
    }
}