using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using JX3PZ.Src;
using JX3PZ.ViewModels;
using JYCalculator.Data;
using JYCalculator.Globals;
using JYCalculator.Src;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace JYCalculator
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            PreLoad();
        }

        // 在界面呈现之前的预加载
        public void PreLoad()
        {
            XFAppStatic.SyncToAppStatic();
            FuncTool.RunTime(LoadData);
            EquipOption.DamageType = XFConsts.CurrentDamnagType; // 设定全局伤害类型
            JYXinFa.GetCurrentXinFa();
        }

        public void LoadData()
        {
            AttributeIDLoader.Load(AppStatic.AT_PATH);
            EquipMapLib.Load(AppStatic.EquipMap_Path);
            //AfterLoad();
            StaticXFData.Load();

            AttributeTabLib.Load(AppStatic.Pz_Path);
            AttributeTabLib.Parse();
            StaticPzData.SetPath(AppStatic.Pz_Path);
            StaticPzData.Load();

            Task.Run(AfterLoad);
        }

        public void AfterLoad()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            EquipStoneSelectSources.Load();
            StaticXFData.MakeStoneAttrFilter();
            //StaticPzData.GetEquipDefaultShow();
            StaticPzData.Data.AttachEquipOptions(StaticXFData.DB.EquipOption);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

#if DEBUG
            Trace.WriteLine($"解析配装数据耗时：{elapsedMs}ms");
#endif
        }
    }
}