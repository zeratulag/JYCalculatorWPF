using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Annotations;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;


namespace JX3CalculatorShared.ViewModels
{
    public enum InputPropertyNameType
    {
        None = 0,
        All = 1,
    }

    public abstract class AbsViewModel : ObservableObject
    {
        #region 全局静态成员

        /// <summary>
        /// 默认成员缓存
        /// </summary>
        internal static class InternalConstCache
        {
            public const string OUTPUT = "__OUTPUT__";
            public const string INPUT = "__INPUT__";
            public const string ALL = "__ALL__";
            public static readonly HashSet<string> EmptyHashSet = new HashSet<string>();
            public static readonly HashSet<string> AllHashSet = new HashSet<string>() { ALL };

            public static readonly PropertyChangedEventArgs OutChangedArg =
                new PropertyChangedEventArgs(OUTPUT);
        }

        #endregion

        #region 成员

        public event PropertyChangedEventHandler InputChanged; // 表示输入数据改变
        public event PropertyChangedEventHandler OutputChanged; // 表示输出数据改变

        protected HashSet<string> InputPropertyNames = new HashSet<string>(); // 表示输入变量的名称，当这些变量改变时触发 InputChanged 事件
        protected HashSet<string> ExcludePropertyNames = new HashSet<string>(); // 表示需要排除的变量名称

        protected readonly bool _AllPropertiesAreInput = false; // 表示所有Property都是输入变量

        [JsonIgnore]
        protected bool _AutoUpdate = true;

        [JsonIgnore]
        public bool _RaiseOutChanged = true; // 是否发送OutputChanged事件

        [JsonIgnore]
        private string _OutName; // 表示触发OutputChanged时的参数名

        [JsonIgnore]
        public string _OutChangedArgPropertyName; // 表示触发OutputChanged时的参数名

        private PropertyChangedEventArgs OutChangedArg = InternalConstCache.OutChangedArg;

        [JsonIgnore]
        public bool _SendMessage { get; protected set; } = false;

        [JsonIgnore]
        public bool _RecieveMessage { get; protected set; } = false;

        #endregion

        #region 构造

        /// <summary>
        /// 指定输入属性的名称
        /// </summary>
        /// <param name="inputPropertyNames"></param>
        protected AbsViewModel(IEnumerable<string> inputPropertyNames)
        {
            InputPropertyNames = inputPropertyNames.ToHashSet();
            InputChanged += UpdateAndRefresh;
            PropertyChanged += HandlePropertyChanged;
            SetOutName();
        }

        protected AbsViewModel(params string[] inputNames): this(inputPropertyNames: inputNames)
        {
        }

        /// <summary>
        /// 特殊情况，只有一个输入属性名
        /// </summary>
        /// <param name="inputPropertyName"></param>
        protected AbsViewModel(string inputPropertyName) : this(new string[1] { inputPropertyName })
        {
        }

        /// <summary>
        /// 不指定输入属性名，而是假设属性名都是/都不是输入属性
        /// </summary>
        protected AbsViewModel(InputPropertyNameType nameType)
        {
            InputPropertyNames = InternalConstCache.EmptyHashSet.Clone();
            switch (nameType)
            {
                case InputPropertyNameType.None:
                    {
                        break;
                    }

                case InputPropertyNameType.All:
                    {
                        InputPropertyNames = InternalConstCache.AllHashSet.Clone();
                        _AllPropertiesAreInput = true;
                        break;
                    }
            }

            InputChanged += UpdateAndRefresh;
            PropertyChanged += HandlePropertyChanged;
            SetOutName();
        }


        ~AbsViewModel()
        {
            DisConnect();
        }

        /// <summary>
        /// 没有任何属性为输入属性，不建议使用
        /// </summary>
        protected AbsViewModel() : this(InputPropertyNameType.None)
        {
        }

        // 设定触发OutputChanged时的参数名
        protected void SetOutName(string name)
        {
            _OutName = name;
            _OutChangedArgPropertyName = InternalConstCache.OUTPUT + _OutName;
            OutChangedArg = new PropertyChangedEventArgs(_OutChangedArgPropertyName);
        }

        // 默认以类名为参数名
        protected void SetOutName()
        {
            SetOutName(GetType().Name);
        }

        /// <summary>
        /// 当派生类所有构建工作均完成时调用
        /// </summary>
        protected virtual void PostConstructor()
        {
            //UpdateAndRefresh();
        }

        // 取消所有外部连接
        public void DisConnect()
        {
            if (OutputChanged == null)
            {
                return;
            }
            foreach (var d in OutputChanged.GetInvocationList())
            {
                OutputChanged -= (PropertyChangedEventHandler) d;
            }
        }


        #endregion

        #region 方法

        /// <summary>
        /// 扩充输入属性名称
        /// </summary>
        /// <param name="names">名称列表，建议用nameof()</param>
        public void ExtendInputNames(params string[] names)
        {
            InputPropertyNames.AddRange(names);
        }

        /// <summary>
        /// 排除输入属性名称
        /// </summary>
        /// <param name="names">属性名称列表，建议用nameof()</param>
        public void ExcludeInputNames(params string[] names)
        {
            ExcludePropertyNames.AddRange(names);
        }


        /// <summary>
        /// 基于改变的参数名，判断触发对应的事件
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsInput(e))
            {
                InputChanged?.Invoke(sender, e);
            }
        }

        // 判定属性是否为输入属性，如果是输入属性则触发更新
        public bool IsInput(PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (ExcludePropertyNames.Contains(name)) return false;
            if (_AllPropertiesAreInput) return true;

            bool res = (name.StartsWith(InternalConstCache.OUTPUT)) ||
                       InputPropertyNames.Contains(e.PropertyName); // 当上一级的VM传出Output改变时也更新
            return res;
        }

        protected void RaiseOutputChanged()
        {
            OutputChanged?.Invoke(this, OutChangedArg);
        }

        protected void RaiseInputChanged(object sender, PropertyChangedEventArgs e)
        {
            InputChanged?.Invoke(sender, e);
        }

        protected void RaisePropertyChanged([CallerMemberName][CanBeNull] string PropertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual void UpdateAndRefresh(object sender, PropertyChangedEventArgs e)
        {
            if (_AutoUpdate)
            {
                UpdateAndRefresh();
            }
        }

        /// <summary>
        /// 更新，刷新命令，并且引发OutputChanged事件
        /// </summary>
        protected virtual void UpdateAndRefresh()
        {
            _Update();
            _RefreshCommands();
            if (_RaiseOutChanged) RaiseOutputChanged();

#if DEBUG
            _DEBUG();
#endif
        }

        public virtual void DisableAutoUpdate()
        {
            //InputChanged -= UpdateAndRefresh;
            _AutoUpdate = false;
        }

        public virtual void EnableAutoUpdate()
        {
            //InputChanged += UpdateAndRefresh;
            _AutoUpdate = true;
        }

        /// <summary>
        /// 启用后立刻更新一次
        /// </summary>
        protected void EnableAutoUpdateAndRefresh()
        {
            EnableAutoUpdate();
            UpdateAndRefresh();
        }

        /// <summary>
        /// 在不触发更新事件的情况下进行一次Action
        /// </summary>
        /// <param name="act"></param>
        public void ActionWithOutUpdate(Action act)
        {
            var autoUpdate = _AutoUpdate; // 如果处于自动更新状态则关闭
            var send = _SendMessage;
            var recieve = _RecieveMessage;

            _SendMessage = false;
            _RecieveMessage = false;
            DisableAutoUpdate();
            act();
            if (autoUpdate)
            {
                EnableAutoUpdate();
            }

            _RecieveMessage = recieve;
            _SendMessage = send;
        }

        public void ActionWithOutUpdate<T>(Action<T> act, T param)
        {

            ActionWithOutUpdate(() => act(param));
        }


        /// <summary>
        /// 确保在执行Action之后只更新一次，Action往往会修改多个输入变量
        /// </summary>
        /// <param name="act">无参数的Action</param>
        public void ActionUpdateOnce(Action act)
        {
            DisableAutoUpdate();
            act();
            EnableAutoUpdateAndRefresh();
        }

        /// <summary>
        /// 确保在执行Action之后只更新一次，Action(T param)往往会修改多个输入变量
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="act">仅有一个参数的Action</param>
        /// <param name="param">参数</param>
        public void ActionUpdateOnce<T>(Action<T> act, T param)
        {
            ActionUpdateOnce(() => act(param));
        }

        /// <summary>
        /// 加入TryCatch，防止Action失败
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="act">仅有一个参数的Action</param>
        /// <param name="param">参数</param>
        public bool TryActionUpdateOnce<T>(Action<T> act, T param)
        {
            DisableAutoUpdate();
            bool succ = false;
            try
            {
                act(param);
                succ = true;
            }
            catch (Exception e)
            {
                succ = false;
                Trace.WriteLine(e);
                throw;
            }
            finally
            {
                EnableAutoUpdateAndRefresh();
            }

            return succ;
        }

        //public virtual bool IsInitialized()
        //{
        //    return _Initialized;
        //}

        #endregion

        #region 方法（需要覆盖）

        protected abstract void _Update(); // 更新状态的真实方法，需要手动重写
        protected abstract void _RefreshCommands(); // 刷新命令，如果有需要的话

        protected virtual void _DEBUG() // DEBUG状态下运行的方法，一般为Trace打印信息
        {

        }

        #endregion
    }
}