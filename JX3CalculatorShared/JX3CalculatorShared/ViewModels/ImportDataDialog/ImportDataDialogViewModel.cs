using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MingYiTools;

namespace JX3CalculatorShared.ViewModels
{
    public class ImportDataDialogViewModel : ObservableObject
    {
        public int SelectedTabIndex { get; set; } = 0;
        public ImportDataJX3BoxViewModel JX3BoxVM { get; } 
        public ImportDataMingYiTextViewModel MingYiTextVM { get; }
        public ImportDataMingYiDatabaseViewModel MingYiDatabaseVM { get; }

        public ImportDataDialogViewModel(bool isImportJBPZ = false)
        {
            MingYiTextVM = new ImportDataMingYiTextViewModel();
            MingYiDatabaseVM = new ImportDataMingYiDatabaseViewModel();
            JX3BoxVM = new ImportDataJX3BoxViewModel
            {
                IsImportPZ = isImportJBPZ
            };
        }

    }

    public enum ImportDataType
    {
        JX3BOXPanel,
        JX3BOXPZ,
        MingYiPZ,
        MingYiDBPZ,
    }
}