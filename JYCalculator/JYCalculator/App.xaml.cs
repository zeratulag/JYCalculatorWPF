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
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using System;

namespace JYCalculator
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public string LogFilePath { get; private set; }
        public string LogDir { get; private set; }

        public App()
        {
            MakeLogger();
            PreLoad();
        }

        public void MakeLogger()
        {
            string appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            LogDir = Path.Combine(Path.GetTempPath(), appName); // 加子目录
            Directory.CreateDirectory(LogDir); // 确保目录存在

            LogFilePath = Path.Combine(LogDir, $"log-{timestamp}.txt"); // 文件名
            //LogFilePath = Path.Combine(Path.GetTempPath(), $"{appName}-log.txt");
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(LogFilePath,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
            Log.Information($"日志路径：{LogFilePath}");
        }

        // 在界面呈现之前的预加载
        public void PreLoad()
        {
            XFAppStatic.SyncToAppStatic();
            FuncTool.RunTime(LoadData, nameof(LoadData));
            EquipOption.DamageType = XFConsts.CurrentDamageType; // 设定全局伤害类型
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

            Task.Run(() => FuncTool.RunTime(AfterLoad, nameof(AfterLoad)));
        }

        public void AfterLoad()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            EquipStoneSelectSources.Load();
            StaticXFData.MakeStoneAttrFilter();
            //StaticPzData.GetEquipDefaultShow();
            StaticPzData.Data.AttachEquipOptions(StaticXFData.DB.EquipOption);

            watch.Stop();
        }
    }
}