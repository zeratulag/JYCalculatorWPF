using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3CalculatorShared.Messages;
using MingYiTools;

namespace JX3CalculatorShared.ViewModels
{
    public class ImportDataMingYiTextViewModel : ObservableObject
    {
        public string Text { get; set; }

        public bool ParseSuccess { get; private set; } = false; // 是否解析成功;

        public MingYiEquipPlanParser ParsedPlan { get; private set; } = null;

        public RelayCommand ImportCmd { get; }

        public ImportDataMingYiTextViewModel()
        {
            ImportCmd = new RelayCommand(Import, () => ParseSuccess);
        }

        public void ParseText()
        {
            ParsedPlan = new MingYiEquipPlanParser(Text);
            ParsedPlan.Parse();
            ParseSuccess = ParsedPlan.ParseSuccess;
            ImportCmd.NotifyCanExecuteChanged();
        }

        public void OnTextChanged()
        {
            ParseText();
        }

        public void Import()
        {
            var msg = new ImportDataMessage(ImportDataType.MingYiPZ, ParsedPlan.Data);
            WeakReferenceMessenger.Default.Send(msg);
        }
    }
}