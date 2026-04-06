using HandyControl.Controls;
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
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            RegisterGlobalExceptionHandlers();
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
            EquipStoneSelectSources.Load();
            StaticXFData.MakeStoneAttrFilter();
            //StaticPzData.GetEquipDefaultShow();
            StaticPzData.Data.AttachEquipOptions(StaticXFData.DB.EquipOption);
        }

        private void RegisterGlobalExceptionHandlers()
        {
            // UI 线程
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            // 非 UI/后台线程
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // 任务未观察异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                Log.Error(e.Exception, "UI thread unhandled exception");
                Growl.Error("发生了一个错误，但程序仍可继续使用。");
            }
            catch { /* 避免再次抛出 */ }
            finally
            {
                // 防止默认行为导致进程退出
                e.Handled = true;
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            try
            {
                Log.Error(ex, "Non-UI thread unhandled exception. IsTerminating={IsTerminating}", e.IsTerminating);
                if (!e.IsTerminating)
                {
                    Growl.Error("后台出现异常，但程序仍可继续使用。");
                }
            }
            catch
            {
                /* swallow */
            }
            // 注意：当 IsTerminating=true 时，.NET 可能仍会结束进程，这里只能做善后
            finally
            {
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                Log.Error(e.Exception, "Task unobserved exception");
            }
            catch { }
            finally
            {
                // 标记为已观察，避免触发默认终止
                e.SetObserved();
            }
        }


    }
}