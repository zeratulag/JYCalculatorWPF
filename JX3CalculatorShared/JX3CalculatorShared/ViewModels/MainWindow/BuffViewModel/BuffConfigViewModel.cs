using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public BuffViewModel[] ValidBuffViewModels = null;
        #endregion

        #region 构造

        public BuffConfigViewModel(IDictionary<string, KBuff> baseBuffDict)
        {
            BuffVMDict = baseBuffDict.ToImmutableDictionary(_ => _.Key,
                _ => new BuffViewModel(_.Value));
            Data = BuffVMDict.Values.OrderBy(_ => _._Buff.Order).ToImmutableArray();
            IsTarget = Data[0].IsTarget;
            BuffType = Data[0]._Buff.BuffType;

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
            GetValidBuffViewModels();
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
            res = KBuff.Type2Header[buffvm._Buff.BuffType];
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

        public void GetValidBuffViewModels()
        {
            // 在buff叠加规则下，获取有效的VM
            var IDs = EmitedBaseBuffGroup.Items.Select(_ => _.BuffID).ToHashSet();
            var res = new List<BuffViewModel>(BuffIDVMDict.Count);
            foreach (var kvp in BuffIDVMDict)
            {
                if (IDs.Contains(kvp.Value._Buff.BuffID))
                {
                    res.Add(kvp.Value);
                }
            }
            ValidBuffViewModels = res.ToArray();
        }


        /// <summary>
        /// 获得飘黄覆盖率
        /// </summary>
        /// <returns></returns>
        public BuffSpecialArg GetPiaoHuangArg()
        {
            var res = new BuffSpecialArg();
            if (BuffType != BuffTypeEnum.Buff_ExtraStack) return res;
            var vm = BuffIDVMDict["20854_1"];
            if (vm.IsChecked)
            {
                res = new BuffSpecialArg(vm.Cover, vm.Stack);
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