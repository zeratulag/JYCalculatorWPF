using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace JX3CalculatorShared.DB
{
    public class EquipOptionDBBase : IDB<string, EquipOption>
    {
        public ImmutableDictionary<string, WPOption> WPData;
        public ImmutableDictionary<string, YZOption> YZData;
        public ImmutableArray<WPOption> WP;
        public ImmutableArray<YZOption> YZ;
        public ImmutableDictionary<string, EquipOptionType> TypeMap;
        public ImmutableDictionary<string, string> WPID2Name; // 武器原始ID到选项的映射
        public ImmutableDictionary<string, string> YZID2Name; // 腰坠原始ID到特效的映射

        public EquipOptionDBBase(IEnumerable<EquipOptionItem> wpitems, IEnumerable<EquipOptionItem> yzitems)
        {
            WPData = wpitems.ToImmutableDictionary(
                _ => _.Name, _ => new WPOption(_));
            YZData = yzitems.ToImmutableDictionary(
                _ => _.Name, _ => new YZOption(_));

            var wptypeMap = WPData.Keys.ToDictionary(
                key => key, key => EquipOptionType.WP);
            var yztypeMap = YZData.Keys.ToDictionary(
                key => key, key => EquipOptionType.YZ);
            var typemap = wptypeMap.Merge(yztypeMap);
            TypeMap = typemap.ToImmutableDictionary();

            WP = WPData.Values.OrderBy(_ => _.Order).ToImmutableArray();
            YZ = YZData.Values.OrderBy(_ => _.Order).ToImmutableArray();

            WPID2Name = GetEquipIDMap(WP).ToImmutableDictionary();
            YZID2Name = GetEquipIDMap(YZ).ToImmutableDictionary();

        }

        public enum EquipOptionType
        {
            WP,
            YZ,
        }

        /// <summary>
        /// 提取装备ID到特效ID的映射关系
        /// </summary>
        /// <param name="data">特效列表</param>
        /// <returns></returns>
        public Dictionary<string, string> GetEquipIDMap(IEnumerable<EquipOption> data)
        {
            var res = new Dictionary<string, string>();
            foreach (var option in data)
            {
                if (option.EquipIDs == null) continue;
                foreach (var equipID in option.EquipIDs)
                {
                    res.Add(equipID, option.Name);
                }
            }
            return res;
        }
        public string GetWPNameByEquipID(string equipID)
        {
            var res = WPID2Name.GetValueOrUseDefault(equipID, "Normal_WP");
            return res;
        }

        public string GetYZNameByEquipID(string equipID)
        {
            var res = YZID2Name.GetValueOrUseDefault(equipID, "Normal_YZ");
            return res;
        }

        public WPOption GetWP(string name)
        {
            return WPData[name];
        }

        public YZOption GetYZ(string name)
        {
            return YZData[name];
        }

        public EquipOption Get(string name)
        {
            EquipOption result = null;
            var type = TypeMap[name];

            switch (type)
            {
                case EquipOptionType.WP:
                    {
                        result = GetWP(name);
                        break;
                    }

                case EquipOptionType.YZ:
                    {
                        result = GetYZ(name);
                        break;
                    }
            }
            return result;
        }

        public EquipOption this[string name] => Get(name);
    }
}