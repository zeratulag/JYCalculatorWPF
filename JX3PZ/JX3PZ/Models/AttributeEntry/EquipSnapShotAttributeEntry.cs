using JX3CalculatorShared.Common;
using JX3PZ.Class;
using System.Collections.Generic;
using System.Linq;

namespace JX3PZ.Models
{
    public class EquipSnapShotAttributeEntry : IModel
    {
        /// <summary>
        /// 表示一件装备（强化后），五行石，附魔的属性集合
        /// </summary>
        public readonly AttributeEntry[] EquipBase; // 装备基础属性（外防）

        public readonly AttributeEntry[] Equip; // 装备词条

        public readonly AttributeEntry[] Diamond; // 五行石词条
        public readonly AttributeEntry Enhance; // 附魔词条
        public readonly AttributeEntry[] BigFM; // 大附魔

        public List<AttributeEntry> AllEntries; // 所有词条之和
        public AttributeEntryCollection EntryCollection; // 词条属性集合


        public EquipSnapShotAttributeEntry(
            IEnumerable<AttributeEntry> equipBase,
            IEnumerable<AttributeEntry> equip,
            AttributeEntry[] diamond,
            AttributeEntry enhance,
            IEnumerable<AttributeEntry> bigfm)
        {
            EquipBase = equipBase?.ToArray();
            Equip = equip?.ToArray();
            Diamond = diamond;
            Enhance = enhance;
            BigFM = bigfm?.ToArray();
        }

        public void GetSum()
        {
            var len = EquipBase?.Length ?? 0 + Equip?.Length ?? 0 + Diamond?.Length ?? 0 + 3 + 5;
            AllEntries = new List<AttributeEntry>(len);
            if (EquipBase != null)
            {
                AllEntries.AddRange(EquipBase);
            }

            if (Equip != null)
            {
                AllEntries.AddRange(Equip);
            }

            if (Diamond != null)
            {
                AllEntries.AddRange(Diamond);
            }

            if (Enhance != null)
            {
                AllEntries.Add(Enhance);
            }

            if (BigFM != null)
            {
                AllEntries.AddRange(BigFM);
            }
        }

        public void GetCollection()
        {
            EntryCollection = new AttributeEntryCollection(AllEntries);
            EntryCollection.Calc();
        }

        public void Calc()
        {
            GetSum();
            GetCollection();
        }
    }
}