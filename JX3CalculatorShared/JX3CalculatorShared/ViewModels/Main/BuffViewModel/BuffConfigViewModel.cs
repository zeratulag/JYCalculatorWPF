using JX3CalculatorShared.Class;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Globals;

namespace JX3CalculatorShared.ViewModels
{
    public class BuffConfigViewModel : CollectionViewModel<BuffViewModel>
    {
        #region 成员

        public readonly ImmutableDictionary<string, BuffViewModel> BuffVMDict;
        public readonly ImmutableDictionary<string, BuffViewModel> BuffDescNameVMDict; // 基于Buff名称检索的字典，例如 "秋风散影"

        private readonly ImmutableDictionary<string, BuffViewModel> BuffIDVMDict; // 基于RawID检索的字典

        public readonly bool IsTarget; // 是否为目标DebuffVM
        public readonly BuffTypeEnum BuffType; // Buff类型

        public readonly Dictionary<string, BuffViewModel> CheckedBuffVMs; // 已启用的Buff
        public BaseBuffGroup EmitedBaseBuffGroup { get; private set; } // 输出的汇总
        public NamedAttrs AttrsDesc { get; private set; } // DEBUG用的名称

        public readonly string HeaderType; // Header类型文字提示（自身/宴席）
        public string Header { get; set; }
        #endregion

        #region 构造

        public BuffConfigViewModel(IDictionary<string, Buff> baseBuffDict)
        {
            BuffVMDict = baseBuffDict.ToImmutableDictionary(_ => _.Key,
                _ => new BuffViewModel(_.Value));
            Data = BuffVMDict.Values.OrderBy(_ => _._Buff.Order).ToImmutableArray();
            IsTarget = Data[0].IsTarget;
            BuffType = Data[0]._Buff.Type;

            BuffDescNameVMDict = BuffVMDict.ToImmutableDictionary(_ => _.Value.DescName, _ => _.Value);
            BuffIDVMDict = BuffVMDict.ToImmutableDictionary(_ => _.Value._Buff.BuffID, _ => _.Value);

            CheckedBuffVMs = new Dictionary<string, BuffViewModel>(Data.Length);

            HeaderType = GetHeaderType();

            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();
        }

        #endregion

        #region 方法

        protected override void _Update()
        {
            UpdateEnabledBuffVMs();
            UpdateEmitBaseBuffGroup();
            UpdateHeader();
        }

        protected void UpdateEnabledBuffVMs()
        {
            CheckedBuffVMs.Clear();
            foreach (var KVP in BuffVMDict)
            {
                if (KVP.Value.IsChecked)
                {
                    CheckedBuffVMs.Add(KVP);
                }
            }
        }

        protected void UpdateEmitBaseBuffGroup()
        {
            var basebuffs = from _ in CheckedBuffVMs.Values select _.EmitedBaseBuff;
            if (basebuffs.Any())
            {
                EmitedBaseBuffGroup = new BaseBuffGroup(basebuffs, IsTarget, true);
            }
            else
            {
                EmitedBaseBuffGroup = IsTarget ? BaseBuffGroup.TargetEmpty : BaseBuffGroup.Empty;
            }
            
        }

        protected override void _DEBUG()
        {
            AttrsDesc = EmitedBaseBuffGroup.ToNamedAttrs();
        }

        public string GetHeaderType()
        {
            string res;
            var buffvm = BuffVMDict.Values.First();
            res = Buff.Type2Header[buffvm._Buff.Type];
            return res;
        }

        protected void UpdateHeader()
        {
            var res = HeaderType;
            if (!EmitedBaseBuffGroup.IsEmpty)
            {
                var names = EmitedBaseBuffGroup.DescNames;
                var content = string.Join(", ", names);
                res = $"{res}：{content}";
            }
            Header = res;
        }


        /// <summary>
        /// 获得飘黄覆盖率
        /// </summary>
        /// <returns></returns>
        public double GetPiaoHuangCover()
        {
            double res = 0.0;
            if (BuffType == BuffTypeEnum.Buff_Extra)
            {
                var vm = BuffIDVMDict["20854_1"];
                if (vm.IsChecked)
                {
                    res = vm.Cover;
                }
            }
            return res;
        }

        #endregion


        #region 导入导出

        public Dictionary<string, BuffVMSav> Export()
        {
            var res = BuffIDVMDict.ToDictionary(_ => _.Key, _ => _.Value.Export());
            return res;
        }

        protected void _Load(Dictionary<string, BuffVMSav> sav)
        {

            foreach (var KVP in BuffIDVMDict)
            {
                if (sav.ContainsKey(KVP.Key))
                {
                    KVP.Value.Load(sav[KVP.Key]);
                }
                else
                {
                    KVP.Value.Reset();
                }
            }

        }

        public void Load(Dictionary<string, BuffVMSav> sav)
        {
            ActionUpdateOnce(_Load, sav);
        }

        #endregion

    }
}