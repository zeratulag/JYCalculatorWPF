using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;

namespace JX3CalculatorShared.ViewModels
{
    public class SkillBuildManagerViewModelBase : ObservableObject
    {
        public RelayCommand SaveAsCmd { get; }
        public RelayCommand OpenFileCmd { get; }
        public RelayCommand ChooseBuildCmd { get; }

        public SkillBuild[] ItemsSource { get; protected set; }
        public SkillBuild SelectedItem { get; set; }

        public virtual void SaveAs()
        {
        }

        public virtual void OpenFile()
        {
        }

        public virtual void ChooseBuild()
        {
        }

        public SkillBuildManagerViewModelBase()
        {
            SaveAsCmd = new RelayCommand(SaveAs);
            OpenFileCmd = new RelayCommand(OpenFile);
            ChooseBuildCmd = new RelayCommand(ChooseBuild);
        }
    }
}