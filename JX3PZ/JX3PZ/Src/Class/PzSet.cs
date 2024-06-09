using JX3CalculatorShared.Utils;
using JX3PZ.Models;
using JX3PZ.ViewModels;
using System.Collections.Generic;
using System.Collections.Immutable;

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
        public int Num { get; set; } = -1;
        public int JN { get; set; } = -1;
        public int SL { get; set; } = -1;
        public string EquipName_Str { get; set; }
        public string EID_Str { get; set; }

        #endregion

        public ImmutableArray<string> EIDs { get; private set; }
        public ImmutableArray<string> EquipNames { get; private set; }

        public PzSetAttribute Attribute { get; private set; }
        public EquipShowSetViewModel VM { get; private set; }

        public void Parse()
        {
            if (EID_Str == null)
            {
                EIDs = ImmutableArray<string>.Empty;
            }
            else
            {
                EIDs = StringTool.ParseStringList(EID_Str, ",").ToImmutableArray();
                EquipNames = StringTool.ParseStringList(EquipName_Str, ",").ToImmutableArray();
                Attribute = new PzSetAttribute(this);
                VM = new EquipShowSetViewModel(this);
            }
        }

        public PzSetResult GetResult(SetCountItem[] items)
        {
            int count = 0;
            var eids = new List<string>(Num);
            var names = new List<string>(Num);
            foreach (var _ in items)
            {
                if (_.SetID == ID)
                {
                    count += 1;
                    eids.Add(_.EID);
                    names.Add(_.Name);
                }
            }

            var res = new PzSetResult(this, count, eids, names);
            return res;
        }
    }

    public class PzSetResult
    {
        // 用于描述 套装特效结果的类
        public readonly PzSet CPzSet;
        public readonly int Count; // 拥有几件
        public readonly List<string> EIDs; // 对应的ID
        public readonly List<string> Names; // 对应的名称
        public readonly string[] Keys; // 套装神力的Key

        public PzSetResult(PzSet cPzSet, int count, List<string> eIDs, List<string> names)
        {
            CPzSet = cPzSet;
            Count = count;
            EIDs = eIDs;
            Names = names;
        }

        public (List<AttributeEntry> Enties, string[] Keys) GetActivatedAttributeEntries()
        {
            var res = CPzSet.Attribute.GetActivatedAttributeEntries(Count);
            return res;
        }


        public PzSetResult(Equip cEquip)
        {
            // 基于单件装备生成
            CPzSet = cEquip.GetPzSet();
            Count = 1;
            EIDs = new List<string>() { cEquip.EID };
            Names = new List<string>() { cEquip.Name };
        }

    }
}