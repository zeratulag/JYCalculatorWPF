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
            // ���Դ򿪴��ڣ���ȡ�ַ���������ת��ΪJBBB��ʽ
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
                        var result = MessageBox.Show("���ݸ�ʽ����", "����", buttons, MessageBoxImage.Warning);
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
                    //    Growl.Success("���ݵ���ɹ�");
                    //}

                    return (success, j);
                }
            }

            return (success, j);
        }
    }
}