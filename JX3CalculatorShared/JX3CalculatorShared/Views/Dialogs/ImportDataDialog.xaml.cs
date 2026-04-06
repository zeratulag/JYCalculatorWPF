using System;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Messages;
using JX3CalculatorShared.ViewModels;

namespace JX3CalculatorShared.Views
{
    /// <summary>
    /// InputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImportDataDialog : Window, IRecipient<ImportDataMessage>
    {
        #region 成员

        public ImportDataDialogViewModel ViewModel;

        public ImportDataDialog()
        {
            InitializeComponent();
            ViewModel = new ImportDataDialogViewModel();
            DataContext = ViewModel;
            WeakReferenceMessenger.Default.Register<ImportDataMessage>(this);
        }

        #endregion

        public ImportDataDialog(bool isImportJBPZ = false) : this()
        {
            ViewModel.JX3BoxVM.IsImportPZ = isImportJBPZ;
        }

        public void Receive(ImportDataMessage message)
        {
            this.Close();
        }
    }
}