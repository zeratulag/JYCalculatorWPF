using JX3CalculatorShared.Utils;
using PropertyChanged;

namespace JX3CalculatorShared.ViewModels
{

    public class CheckBoxViewModel : AbsViewModel
    {
        /// <summary>
        /// 通用复选框VM
        /// </summary>
        public bool IsChecked { get; set; }

        protected CheckBoxViewModel() : base(nameof(IsChecked))
        {
        }

        protected override void _Update()
        {
        }

        protected override void _RefreshCommands()
        {
        }
    }


    public class IconToolTipCheckBoxViewModel : CheckBoxViewModel
    {
        /// <summary>
        /// 通用带有名称，图标和ToolTip的复选框VM
        /// </summary>

        [DoNotNotify]
        public string Name { get; protected set; }

        [DoNotNotify] public string ToolTip { get; protected set; }

        [DoNotNotify] public int IconID { get; protected set; }

    }
}