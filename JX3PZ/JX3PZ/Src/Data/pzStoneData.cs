using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Utils;

namespace JX3PZ.Data
{
    public class pzStoneData
    {
        public ImmutableArray<Stone>[] Stones; // 基于Level的分表
        public ImmutableArray<Stone>[] UsefulStones; // 基于Level的匹配五彩石分表

        public pzStoneData()
        {
            Stones = new ImmutableArray<Stone>[6 + 1];
            UsefulStones = new ImmutableArray<Stone>[6 + 1];
        }

        public void Load(IEnumerable<Stone> stones)
        {
            var q = from _ in stones group _ by _.Level;
            foreach (var _ in q)
            {
                var v = _.OrderBy(e => e.Level).ThenBy(e => e.ID).Where(e => e.IsValid).ToImmutableArray();
                Stones[_.Key] = v == null ? ImmutableArray<Stone>.Empty : v;
                UsefulStones[_.Key] = Stones[_.Key].Where(e => e.Useful.ToBool()).ToImmutableArray();
            }
        }

        public ImmutableArray<Stone> this[int level] => Stones[level];
    }
}