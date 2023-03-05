using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JYCalculator.Globals;
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
            AtLoader.Load(XFAppStatic.AT_PATH);
            EquipOption.DamageType = XFConsts.CurrentDamnagType; // 设定全局伤害类型
        }
    }
}