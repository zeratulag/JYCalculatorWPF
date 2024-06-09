using JX3PZ.Data;
using JX3PZ.Globals;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.Class
{
    public class PzSetAttribute
    {
        // 描述套装属性
        public readonly ImmutableArray<int> Effect2ID;
        public readonly ImmutableArray<int> Effect4ID;

        public readonly ImmutableDictionary<int, PzSetEffect> Effects;
        public PzSetAttribute(PzSet p)
        {
            var e2 = ImmutableArray.CreateBuilder<int>();
            if (p.Effect2_1 != 0)
            {
                e2.Add(p.Effect2_1);
            }

            if (p.Effect2_2 != 0)
            {
                e2.Add(p.Effect2_2);
            }

            Effect2ID = e2.ToImmutableArray();

            var e4 = ImmutableArray.CreateBuilder<int>();

            if (p.Effect4_1 != 0)
            {
                e4.Add(p.Effect4_1);
            }

            if (p.Effect4_2 != 0)
            {
                e4.Add(p.Effect4_2);
            }

            Effect4ID = e4.ToImmutableArray();

            var se2 = new PzSetEffect(2, Effect2ID);
            var se4 = new PzSetEffect(4, Effect4ID);

            var effects = new Dictionary<int, PzSetEffect>(4);
            if (Effect2ID.Any())
            {
                effects.Add(2, se2);
            }

            if (Effect4ID.Any())
            {
                effects.Add(4, se4);
            }

            Effects = effects.ToImmutableDictionary();

            if (p.JN > 0)
            {
                Effects[p.JN].Key = "JN";
            }

            if (p.SL > 0)
            {
                Effects[p.SL].Key = "SL";
            }

        }

        /// <summary>
        /// 在拥有n件套装情况下，激活了哪些属性
        /// </summary>
        /// <param name="n"></param>
        public List<PzSetEffect> GetActivatedEffects(int n)
        {
            var res = new List<PzSetEffect>(Effects.Count);
            res.AddRange(from kvp in Effects where kvp.Key <= n select kvp.Value);
            return res;
        }

        public (List<AttributeEntry> Enties, string[] Keys) GetActivatedAttributeEntries(int n)
        {
            var res = new List<AttributeEntry>(Effects.Count * 2);
            var effects = GetActivatedEffects(n);
            var keys = effects.Select(_ => _.Key).ToArray();
            foreach (var e in effects)
            {
                res.AddRange(e.Entries);
            }
            return (res, keys);
        }

    }

    public class PzSetEffect
    {
        // 套装属性模型
        public readonly int Num; // 激活此效果所需要的套装数
        public readonly ImmutableArray<int> EffectID; // TabID
        public readonly ImmutableArray<AttributeTabItem> TabItems;
        public readonly ImmutableArray<AttributeEntry> Entries;
        public readonly string Desc; // 效果描述
        public string Key; // 表示门派套装特效
        public PzSetEffect(int num, IEnumerable<int> effectID)
        {
            Num = num;
            EffectID = effectID.ToImmutableArray();
            TabItems = AttributeTabLib.Gets(EffectID);
            Entries = TabItems.Select(_ => _.GetAttributeEntry(AttributeEntryTypeEnum.Set)).ToImmutableArray();
            Desc = ParseDesc(Entries);
        }

        public static string ParseDesc(IEnumerable<AttributeEntry> entries)
        {
            var desc = new List<string>(2);

            foreach (var entry in entries)
            {
                if (entry.Attribute.IsValue)
                {
                    desc.Add(entry.GetDesc());
                }
                else
                {
                    desc.Add(entry.Desc);
                }
            }

            string res = String.Join("，", desc);
            return res;
        }
    }
}