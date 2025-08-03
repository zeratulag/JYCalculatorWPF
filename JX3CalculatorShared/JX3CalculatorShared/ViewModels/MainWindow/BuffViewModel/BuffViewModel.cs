using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Class;
using PropertyChanged;

namespace JX3CalculatorShared.ViewModels
{
    public class BuffViewModel : CheckBoxViewModel
    {
        /// <summary>
        /// 通用描述单个Buff（包括单选框，覆盖率，叠加层数）ViewModel
        /// </summary>

        #region 成员

        public bool IsEnabled { get; set; } = true;

        [DoNotNotify] public string Name { get; protected set; }

        [DoNotNotify] public int IconID { get; protected set; }

        public string ToolTip { get; protected set; }

        public readonly KBuff _Buff; // 基础模板

        public BaseBuff EmitedBaseBuff { get; private set; } // 生成的Buff

        public string DescName { get; }

        public int Stack { get; set; }
        public int MaxStack { get; }
        public double Cover { get; set; }

        public bool IsTarget;

        public bool HasStackInput { get; } // 是否需要输入叠加层数
        public bool HasCoverInput { get; } // 是否需要输入覆盖率

        public bool CanStack { get; } // 是否有多层叠加

        public RelayCommand CancelThisCmd { get; }
        #endregion

        #region 构造

        public BuffViewModel(KBuff buff)
        {
            _Buff = buff;

            Name = buff.Name;
            IconID = buff.IconID;
            DescName = buff.DescName;
            Stack = buff.DefaultStack;
            MaxStack = buff.MaxStack;
            Cover = buff.DefaultCover;
            CanStack = MaxStack > 1;

            IsTarget = buff.IsTarget;

            HasStackInput = _Buff.HasStackInput();
            HasCoverInput = _Buff.HasCoverInput();

            ExtendInputNames(nameof(Stack), nameof(Cover));

            CancelThisCmd = new RelayCommand(CancelThis);

            PostConstructor();
            EmitBaseBuff();
        }

        #endregion

        #region 方法

        public void Reset()
        {
            Stack = _Buff.DefaultStack;
            Cover = _Buff.DefaultCover;
            IsChecked = false;
        }

        protected override void _Update()
        {
            EmitBaseBuff();
        }

        protected void EmitBaseBuff()
        {
            EmitedBaseBuff = _Buff.Emit(Cover, Stack);
            ToolTip = EmitedBaseBuff.ToolTip;
        }

        public void CancelThis()
        {
            IsChecked = false;
        }

        #endregion


        #region 导入导出

        public BuffVMSav Export()
        {
            var res = new BuffVMSav(_Buff.BuffID, IsChecked, Stack, Cover);
            return res;
        }

        protected void _Load(BuffVMSav sav)
        {
            if (_Buff.BuffID != sav.BuffID) return;
            Reset();
            IsChecked = sav.IsChecked;
            Stack = sav.Stack;
            Cover = sav.Cover;
        }

        public void Load(BuffVMSav sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        #endregion
    }

    public class BuffVMSav
    {
        public readonly string BuffID;
        public readonly bool IsChecked;
        public readonly int Stack;
        public readonly double Cover;

        public BuffVMSav(string buffID, bool isChecked, int stack, double cover)
        {
            BuffID = buffID;
            IsChecked = isChecked;
            Stack = stack;
            Cover = cover;
        }
    }
}