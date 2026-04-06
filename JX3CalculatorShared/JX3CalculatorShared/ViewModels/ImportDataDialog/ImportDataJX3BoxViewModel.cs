using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Messages;

namespace JX3CalculatorShared.ViewModels
{
    public class ImportDataJX3BoxViewModel : ObservableObject
    {
        public string Text { get; set; }

        public bool IsImportPZ { get; set; } = false; // 是否为导入装备数据模式

        public bool ParseSuccess { get; private set; } = false; // 是否解析成功;

        public JBBB JBData { get; private set; } = null;

        public RelayCommand ImportCmd { get; }

        public ImportDataJX3BoxViewModel()
        {
            ImportCmd = new RelayCommand(Import, () => ParseSuccess);
        }

        public void ParseText()
        {
            var res = JBBB.TryReadFromJSON(Text);
            ParseSuccess = res.Success;
            JBData = res.JBData;
            ImportCmd.NotifyCanExecuteChanged();
        }

        public void OnTextChanged()
        {
            ParseText();
        }

        public void Import()
        {
            var importType = IsImportPZ ? ImportDataType.JX3BOXPZ : ImportDataType.JX3BOXPanel;
            var res = new ImportDataMessage(importType, JBData);
            WeakReferenceMessenger.Default.Send(res);
        }
    }
}