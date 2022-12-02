using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;

namespace JX3CalculatorShared.Src.DB
{
    public class SetOptionDBBase : IDB<int, SetOption>
    {
        public ImmutableDictionary<int, SetOption> Data;
        public ImmutableDictionary<string, int> EquipID2SetID; // 装备ID到套装ID的映射

        public SetOptionDBBase(IEnumerable<SetOptionItem> itemdata)
        {
            Data = itemdata.ToImmutableDictionary(_ => _.SetID, _ => new SetOption(_));

            var equipID2SetID = new Dictionary<string, int>(Data.Count * 6);
            foreach (var _ in Data.Values)
            {
                foreach (var eID in _.EquipIDs)
                {
                    equipID2SetID.Add(eID, _.SetID);
                }
            }

            EquipID2SetID = equipID2SetID.ToImmutableDictionary();
        }

        public SetOption Get(int id)
        {
            var res = Data[id]; // 防止意外修改
            return res;
        }

        public SetOption this[int id] => Get(id);

        /// <summary>
        /// 根据装备ID，获得套装统计
        /// </summary>
        /// <param name="equipIDs">装备ID集合</param>
        /// <returns></returns>
        public Dictionary<int, int> GetSetCounts(IEnumerable<string> equipIDs)
        {
            var setids = new List<int>();
            foreach (var _ in equipIDs)
            {
                if (EquipID2SetID.ContainsKey(_))
                {
                    var setid = EquipID2SetID[_];
                    setids.Add(setid);
                }
            }

            var res = setids.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            return res;
        }

        /// <summary>
        /// 计算激活的套装效果
        /// </summary>
        /// <param name="equipIDs"></param>
        /// <returns></returns>
        public HashSet<string> GetSetEffects(IEnumerable<string> equipIDs)
        {
            var res = new HashSet<string>();
            var setcounts = GetSetCounts(equipIDs);
            foreach (var KVP in setcounts)
            {
                var setOption = Get(KVP.Key);
                if (KVP.Value >= 2)
                {
                    res.Add(setOption.Effect2);
                }

                if (KVP.Value >= 4)
                {
                    res.Add(setOption.Effect4);
                }
            }

            return res;
        }
    }
}