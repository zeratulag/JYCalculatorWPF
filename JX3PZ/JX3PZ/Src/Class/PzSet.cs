using System;
using JX3CalculatorShared.Utils;
using JX3PZ.Models;
using JX3PZ.ViewModels;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.Class
{
    public class PzSet
    {
        #region 自动生成的

        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public int UIID { get; set; } = -1;
        public int ForceMask { get; set; } = -1;
        public int Effect2_1 { get; set; } = -1;
        public int Effect2_2 { get; set; } = -1;
        public int Effect4_1 { get; set; } = -1;
        public int Effect4_2 { get; set; } = -1;
        public string SetName { get; set; }
        public int TotalNum { get; set; } = -1; // 套装包括的装备总数，等于套装备数量总和
        public int JN { get; set; } = -1;
        public int SL { get; set; } = -1;
        public string EquipName_Str { get; set; }
        public string EID_Str { get; set; }

        public int UniqueNum { get; set; } // 套装有效的装备数量，即子套装的装备数，以 西塞/孤漠 共同激活为例，套装装备总数为10，但是有效装备数量为5
        public int SubSetNum { get; set; } // 包含子套装的数量, UniqueNum * SubSetNum == Num;
        #endregion

        public ImmutableArray<string> EIDs { get; private set; }
        public ImmutableArray<string> EquipNames { get; private set; }

        public PzSetAttribute Attribute { get; private set; }
        public EquipShowSetViewModel VM { get; private set; }
        public List<PzSubSet> SubSets { get; } = new List<PzSubSet>(4); // 关联的子套装
        public ImmutableArray<PzSetEquipModel> SetEquipModels { get; private set; }

        public void Parse()
        {
            if (EID_Str == null)
            {
                EIDs = ImmutableArray<string>.Empty;
            }
            else
            {
                EIDs = StringTool.ParseStringList(EID_Str).ToImmutableArray();
                EquipNames = StringTool.ParseStringList(EquipName_Str).ToImmutableArray();
            }
            SetEquipModels = Enumerable.Range(0, UniqueNum).Select(_ => new PzSetEquipModel()).ToImmutableArray();
        }

        public void MakeViewModel()
        {
            Attribute = new PzSetAttribute(this);
            VM = new EquipShowSetViewModel(this);
        }

        public void AttachSubSet(PzSubSet pzSubSet)
        {
            if (pzSubSet.SetID != ID)
            {
                throw new ArgumentException("子套装ID错误！");
            }
            SubSets.Add(pzSubSet);

            for (int i = 0; i < UniqueNum; i++)
            {
                var e = SetEquipModels[i];
                e.EIDs.Add(pzSubSet.EIDs[i]);
                e.Names.Add(pzSubSet.EquipNames[i]);
            }
        }

        public PzSetResult CalcResult(SetCountItem[] items)
        {
            int totalNum = 0;
            var eids = new List<string>(TotalNum);
            var names = new List<string>(TotalNum);
            foreach (var _ in items)
            {
                if (_.SetID == ID)
                {
                    totalNum += 1;
                    eids.Add(_.EID);
                    names.Add(_.Name);
                }
            }
            var res = new PzSetResult(this, totalNum, eids, names);
            return res;
        }
    }


    public class PzSetEquipModel
    {
        // 用于表示多个子套装情况下，每个装备对应的可用装备词条的类
        public List<string> EIDs { get; } = new List<string>(4); // 7_101141, 7_101470
        public List<string> Names { get; } = new List<string>(4); // 西塞·江鸥衣, 孤漠·照江衣
        public int Num => EIDs.Count; // 该位置有几件不同的装备

        public bool Match(List<string> equipIDs)
        {
            // 判断装备列表是否匹配
            bool res = EIDs.Intersect(equipIDs).Any();
            return res;
        }
    }

    public class PzSetResult
    {
        // 用于描述 套装特效结果的类
        public readonly PzSet CPzSet;
        public readonly int TotalNum; // 拥有几件
        public readonly List<string> EIDs; // 对应的ID
        public readonly List<string> Names; // 对应的名称
        public readonly string[] Keys; // 套装神力的Key

        public PzSetResult(PzSet cPzSet, int totalNum, List<string> eIDs, List<string> names)
        {
            CPzSet = cPzSet;
            TotalNum = totalNum;
            EIDs = eIDs;
            Names = names;
        }

        public (List<AttributeEntry> Enties, string[] Keys) GetActivatedAttributeEntries()
        {
            var res = CPzSet.Attribute.GetActivatedAttributeEntries(TotalNum);
            return res;
        }


        public PzSetResult(Equip cEquip)
        {
            // 基于单件装备生成
            CPzSet = cEquip.GetPzSet();
            TotalNum = 1;
            EIDs = new List<string>() {cEquip.EID};
            Names = new List<string>() {cEquip.Name};
        }
    }
}