using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Utils;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Models;
using JYCalculator.Src.Data;
using JYCalculator.Src.DB;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;


namespace JYCalculator.ViewModels
{
    public class BigFMConfigViewModel : CollectionViewModel<BigFMSlotViewModel>
    {
        #region 成员

        public readonly Dictionary<EquipSubTypeEnum, BigFMSlotViewModel> BigFMSlotDict;

        public readonly Dictionary<EquipSubTypeEnum, BigFM> EnabledItems;

        public readonly BigFMConfigModel Model;

        public readonly BigFMSlotConfig[] Config;
        public readonly int NSlot;

        public readonly Dictionary<string, double> SAttrs;
        public NamedAttrs AttrsDesc;

        public BigFMConfigArg Arg => Model.Arg;

        #endregion

        #region 构造

        public BigFMConfigViewModel(BigFMDB db)
        {
            BigFMSlotDict = db.TypeData.ToDictionary(
                kvp => kvp.Key,
                kvp => BigFMSlotViewModel.CreateCurrentLevelVM(kvp.Value));
            BigFMSlotDict.Remove(EquipSubTypeEnum.BOTTOMS);
            // 按照顺序添加大附魔位置
            var vms = new List<BigFMSlotViewModel>(BigFMDB.SubTypeOrder.Length);
            vms.AddRange(BigFMDB.SubTypeOrder.Select(subType => BigFMSlotDict[subType]));
            Data = vms.ToImmutableArray();

            EnabledItems = new Dictionary<EquipSubTypeEnum, BigFM>(BigFMDB.SubTypeOrder.Length);
            foreach (var subtype in BigFMDB.SubTypeOrder)
            {
                EnabledItems.Add(subtype, null); // 全置为空
            }

            Model = new BigFMConfigModel();
            SAttrs = Model.SAttrs;

            NSlot = Data.Length;
            Config = new BigFMSlotConfig[NSlot];

            AttachDependVMsOutputChanged();
            PostConstructor();
            _Update();
        }

        public BigFMConfigViewModel() : this(StaticJYData.DB.BigFM)
        {
        }

        #endregion

        #region 方法

        protected override void _Update()
        {
            foreach (var KVP in BigFMSlotDict)
            {
                EnabledItems[KVP.Key] = KVP.Value.EnabledItem;
            }

            Model.UpdateInput(this);
            AttrsDesc = Model.AttrsDesc;
            GetConfig();
        }

        protected override void _DEBUG()
        {
            var res = new Dictionary<EquipSubTypeEnum, string>(6);
            foreach (var KVP in EnabledItems)
            {
                if (KVP.Value != null)
                {
                    res.Add(KVP.Key, KVP.Value.DescName);
                }
            }
            res.TraceCat();

            foreach (var _ in Config)
            {
                Trace.Write($"{_.IsChecked}: {_.ItemID},  ");
            }
        }

        protected void GetConfig()
        {
            for (int i = 0; i < NSlot; i++)
            {
                Config[i] = Data[i].Config;
            }
        }

        protected void _Load(BigFMSlotConfig[] config)
        {
            for (int i = 0; i < NSlot; i++)
            {
                Data[i].Load(config[i]);
            }
        }

        public void Load(BigFMSlotConfig[] config)
        {
            ActionUpdateOnce(_Load, config);
        }

        #endregion
    }


    // 用于交换大附魔配置的Class
}