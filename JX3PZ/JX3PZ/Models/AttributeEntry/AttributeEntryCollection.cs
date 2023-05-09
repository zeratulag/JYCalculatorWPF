using System;
using System.Collections.Generic;
using System.Linq;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;

namespace JX3PZ.Models
{
    public class AttributeEntryCollection : IModel
    {
        public readonly List<AttributeEntry> Data;
        public List<AttributeEntry> ValueEntries { get; }
        public List<AttributeEntry> OtherEntries { get; }
        public Dictionary<string, int> ValueDict { get; }

        public AttributeEntryCollection(int capacity)
        {
            Data = new List<AttributeEntry>(capacity);
            ValueDict = new Dictionary<string, int>(capacity);
            ValueEntries = new List<AttributeEntry>(capacity);
            OtherEntries = new List<AttributeEntry>(Math.Min(capacity, 20));
        }


        public AttributeEntryCollection(IList<AttributeEntry> data) : this(data.Count + 10)
        {
            Data.AddRange(data);
        }

        public AttributeEntryCollection(List<AttributeEntry> data, List<AttributeEntry> valueEntries,
            List<AttributeEntry> otherEntries, Dictionary<string, int> valueDict)
        {
            Data = data;
            ValueEntries = valueEntries;
            OtherEntries = otherEntries;
            ValueDict = valueDict;
        }

        public AttributeEntryCollection(AttributeEntryCollection old)
        {
            Data = old.Data.Copy();
            ValueEntries = old.ValueEntries.Copy();
            OtherEntries = old.OtherEntries.Copy();
            ValueDict = old.ValueDict.ToDict();
        }

        public void Calc()
        {
            SplitValueOther();
        }

        public void SplitValueOther()
        {
            AddValueOther(Data);
        }

        public void AddValueOther(IEnumerable<AttributeEntry> data)
        {
            // 根据是否为数值类进行拆分，并且把数值类总结字典
            foreach (var entry in data)
            {
                if (entry == null) continue;

                if (entry.Attribute.IsValue)
                {
                    ValueEntries.Add(entry);
                    ValueDict.TryGetValue(entry.ModifyType, out int v);
                    ValueDict[entry.ModifyType] = v + entry.Value;
                }
                else
                {
                    OtherEntries.Add(entry);
                }
            }
        }


        public void Append(AttributeEntryCollection other)
        {
            // 就地加入到自身
            if (other == null) return;
            if (other.Data != null)
            {
                Data.AddRange(other.Data);
            }

            if (other.OtherEntries != null)
            {
                OtherEntries.AddRange(other.OtherEntries);
            }

            if (other.ValueEntries != null)
            {
                ValueEntries.AddRange(other.ValueEntries);
            }

            if (!other.ValueDict.Any())
            {
                ValueDict.ValueDictAppend(other.ValueDict);
            }
        }

        public void Append(IEnumerable<AttributeEntry> data)
        {
            if (data == null) return;
            Data.AddRange(data);
            AddValueOther(data);
        }


        public static AttributeEntryCollection Append(AttributeEntryCollection a, AttributeEntryCollection b)
        {
            var res = new AttributeEntryCollection(a);
            res.Append(b);
            return res;
        }

        public static AttributeEntryCollection Sum(AttributeEntryCollection[] items)
        {
            int n = items.Length;
            var otherEntries = new List<AttributeEntry>(n * 1);
            var valueEntries = new List<AttributeEntry>(n * 12);
            var data = new List<AttributeEntry>(n * 13 + 10);
            var dict = new Dictionary<string, int>(30);
            foreach (var _ in items)
            {
                if (_ == null)
                {
                    continue;
                }

                if (_.Data != null)
                {
                    data.AddRange(_.Data);
                }
                else
                {
                    continue;
                }

                if (_.ValueEntries != null)
                {
                    valueEntries.AddRange(_.ValueEntries);
                }

                if (_.OtherEntries != null)
                {
                    otherEntries.AddRange(_.OtherEntries);
                }

                dict.ValueDictAppend(_.ValueDict);
            }

            return new AttributeEntryCollection(data, valueEntries, otherEntries, dict);
        }
    }
}