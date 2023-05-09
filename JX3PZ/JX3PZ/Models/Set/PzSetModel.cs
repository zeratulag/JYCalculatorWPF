using JX3PZ.Class;
using JX3PZ.Src;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3PZ.ViewModels;
using JX3PZ.Data;

namespace JX3PZ.Models
{
    public class PzSetModel : IModel
    {
        // 用于计算套装激活效果的类
        public List<SetCountItem> Items;
        public Dictionary<int, SetCountItem[]> SetCountItems;
        public Dictionary<int, PzSet> Sets; // SetID到套装特效
        public Dictionary<int, PzSetResult> Results; // SetID到套装结果

        public Dictionary<int, EquipShowSetViewModel> VMs; // SetID 到 VM 的映射

        public Dictionary<int, int[]> SetID2Position; // SetID到Position的映射
        public int[] Position2SetID; // 每个部位对应的SetID

        public List<AttributeEntry> SetEntries = null; // 生效的套装属性效果
        public List<string> SetKeys; // 生效的神力技能特效Key

        public PzSetModel(List<SetCountItem> items)
        {
            Items = items;
        }

        public void Calc()
        {
            GetSets();
            GetSetResults();
            GetActivatedAttributeEntries();
        }

        public void GetSets()
        {
            var res = Items.GroupBy(_ => _.SetID).ToDictionary(_ => _.Key, _ => _.ToArray());
            SetCountItems = res;
            SetID2Position = res.ToDictionary(_ => _.Key, _ => _.Value.Select(e => e.Position).ToArray());
            Sets = res.ToDictionary(_ => _.Key, _ => StaticPzData.GetPzSet(_.Key));
            GetPosition2SetID();
        }

        public void GetPosition2SetID()
        {
            Position2SetID = new int[EquipMapLib.MAX_POSITION + 1];
            foreach (var kvp in SetID2Position)
            {
                foreach (var pos in kvp.Value)
                {
                    Position2SetID[pos] = kvp.Key;
                }
            }
        }

        public void GetSetResults()
        {
            Results = Sets.ToDictionary(_ => _.Key, _ => _.Value.GetResult(SetCountItems[_.Key]));
        }

        public void GetActivatedAttributeEntries()
        {
            SetEntries = new List<AttributeEntry>(Results.Count * 4);
            SetKeys = new List<string>(4);
            foreach (var kvp in Results)
            {
                var r = kvp.Value.GetActivatedAttributeEntries();
                SetEntries.AddRange(r.Enties);
                SetKeys.AddRange(r.Keys);
            }
        }

        public Dictionary<int, EquipShowSetViewModel> GetUpdateViewModels()
        {
            var vms = Sets.ToDictionary(_ => _.Key, _ => _.Value.VM);
            foreach (var kvp in vms)
            {
                kvp.Value.UpdateFrom(Results[kvp.Key]);
            }

            VMs = vms;
            return vms;
        }
    }
}