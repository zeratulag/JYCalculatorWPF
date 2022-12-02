using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Class;

namespace JX3CalculatorShared.ViewModels
{
    public struct BigFMSlotConfig
    {
        public bool IsChecked;
        public int ItemID;

        public BigFMSlotConfig(bool isChecked, int itemId)
        {
            IsChecked = isChecked;
            ItemID = itemId;
        }
    }

    public class BigFMConfigArg
    {
        public BigFM Belt = null; // 如果为-1或者0，表示没有
        public BigFM Shoes = null;
        public BigFM Wrist = null;

        public void Reset()
        {
            Belt = null;
            Shoes = null;
            Wrist = null;
        }

        public BigFMConfigArg(BigFM belt, BigFM shoes, BigFM wrist)
        {
            Belt = belt;
            Shoes = shoes;
            Wrist = wrist;
        }

        public BigFMConfigArg()
        {
        }
    }

    public class BigFMSlotViewModel : ComboBoxViewModel<BigFM>
    {
        public readonly EquipSubTypeEnum SubType;
        public bool IsChecked { get; set; } = true;
        public bool CheckBoxIsEnabled { get; set; } = true; // 复选框是否可用
        public string TypeDesc { get; }

        public readonly ImmutableDictionary<int, int> ItemIDMap; // 通过ItemID导入与导出, ItemID到Index的映射

        public BigFMSlotConfig Config;

        public BigFM EnabledItem => IsChecked ? ItemsSource[SelectedIndex] : null;
        public int EnabledRank => EnabledItem?.Rank ?? -1;

        public int DefaultIndex; // 默认选项


        /// <summary>
        /// 构建VM
        /// </summary>
        /// <param name="data"></param>
        /// <param name="onlyCurrent">是否仅使用当前版本的大附魔</param>
        public BigFMSlotViewModel(IEnumerable<BigFM> data) : base(data)
        {
            SubType = ItemsSource[0].SubType;
            TypeDesc = SubType.ToShortDescString();
            ExtendInputNames(nameof(IsChecked));

            DefaultIndex = Length - 1;
            if (SubType == EquipSubTypeEnum.BOTTOMS)
            {
                DefaultIndex -= 1; // 下装默认用残卷
            }

            SelectedIndex = DefaultIndex;

            var map = new Dictionary<int, int>(6);
            for (int i = 0; i < ItemsSource.Length; i++)
            {
                map.Add(ItemsSource[i].ItemID, i);
            }

            ItemIDMap = map.ToImmutableDictionary();

            PostConstructor();
        }


        /// <summary>
        /// 如果onlyCurrent，则筛选出当前版本的大附魔，反之所有
        /// </summary>
        /// <param name="data">输入数据</param>
        /// <returns></returns>
        public static IEnumerable<BigFM> FilterBigFms(IEnumerable<BigFM> data)
        {
            IEnumerable<BigFM> res;
            res = from _ in data where _.DLCLevel == StaticData.CurrentLevel select _;
            if (res.Any())
            {
                return res;
            }
            else
            {
                return data;
            }
        }

        public static BigFMSlotViewModel CreateCurrentLevelVM(IEnumerable<BigFM> data)
        {
            var newData = FilterBigFms(data);
            return new BigFMSlotViewModel(newData);
        }


        protected override void _Update()
        {
            GetConfig();
        }

        protected void GetConfig()
        {
            Config = new BigFMSlotConfig(IsChecked, SelectedItem.ItemID);
        }

        public bool Load(bool isChecked, int enchantId)
        {
            IsChecked = isChecked;
            var res = false;

            if (ItemIDMap.ContainsKey(enchantId))
            {
                SelectedIndex = ItemIDMap[enchantId];
                res = true;
            }
            else
            {
                SelectedIndex = DefaultIndex;
            }

            return res;
        }

        public bool Load(BigFMSlotConfig config)
        {
            return Load(config.IsChecked, config.ItemID);
        }


        // 设定与初始属性的关联（若设置初始属性已经包括，则不再可以自由勾选）
        public void SetInitCharConnect(bool had)
        {
            if (had)
            {
                IsChecked = true;
                CheckBoxIsEnabled = false;
            }
            else
            {
                CheckBoxIsEnabled = true;
            }
        }
    }
}