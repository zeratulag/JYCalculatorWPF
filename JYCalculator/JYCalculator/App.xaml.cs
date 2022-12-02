using JYCalculator.Globals;
using System.Windows;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;

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
            AtLoader.Load(JYAppStatic.AT_PATH);
            EquipOption.DamageType = JYConsts.CurrentDamnagType; // 设定全局伤害类型
        }

    }
}
