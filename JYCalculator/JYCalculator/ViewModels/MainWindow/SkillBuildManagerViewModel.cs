using System.IO;
using System.Linq;
using JX3CalculatorShared.ViewModels;
using JYCalculator.DB;
using JYCalculator.Globals;
using JYCalculator.Models;
using Newtonsoft.Json;
using System.Text;
using JX3CalculatorShared.Models;
using Microsoft.Win32;
using JX3CalculatorShared.Utils;
using JYCalculator.Data;

namespace JYCalculator.ViewModels
{
    public class SkillBuildManagerViewModel: SkillBuildManagerViewModelBase
    {
        public readonly MainWindowViewModels _MainVM;
        public SkillBuildManagerViewModel(MainWindowViewModels _mainVM): base()
        {
            _MainVM = _mainVM;
            ItemsSource = StaticXFData.DB.SkillBuild.Array.ToArray();
        }

        public void LoadDefault()
        {
            SelectedItem = StaticXFData.DB.SkillBuild.Default;
            ChooseBuild();
        }

        public override void ChooseBuild()
        {
            _MainVM.LoadSkillBuild(SelectedItem.Sav);
        }

        public SkillBuildSav GetSkillBuildSav()
        {
            var res = new SkillBuildSav()
            {
                AppMeta = Globals.XFAppStatic.CurrentAppMeta,
                SkillMiJi = _MainVM.Model.SkillMiJiVM.Export(),
                QiXue = _MainVM.Model.QiXueVM.Export(),
            };
            return res;
        }

        public override void SaveAs()
        {
            var sav = GetSkillBuildSav();
            var saveFileDialog = new SaveFileDialog
            {
                Filter = XFAppStatic.SkillBuildFileFilter,
                AddExtension = true,
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToFile(saveFileDialog.FileName, sav);
            }
        }

        public override void OpenFile()
        {
            var sav = OpenFileAsSave();
            if (sav != null)
            {
                _MainVM.LoadSkillBuild(sav);
            }
        }

        // 将技能Build Sav保存到文件
        public void SaveToFile(string filepath, SkillBuildSav sav)
        {
            var json = JsonConvert.SerializeObject(sav);
            File.WriteAllText(filepath, json, Encoding.UTF8);
        }

        public SkillBuildSav ReadFileAsSav(string filepath)
        {
            var jsontxt = File.ReadAllText(filepath);
            (bool success, SkillBuildSav sav) = ImportTool.TryDeJSON<SkillBuildSav>(jsontxt);
            if (success)
            {
                return sav;
            }
            else
            {
                return null;
            }
        }

        public SkillBuildSav OpenFileAsSave()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = XFAppStatic.SkillBuildFileFilter,
                AddExtension = true,
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var res = ReadFileAsSav(openFileDialog.FileName);
                return res;
            }
            return null;
        }
    }

}