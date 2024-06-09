using JX3CalculatorShared.Class;
using JX3CalculatorShared.DB;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3PZ.Data
{
    public class pzEnhanceData
    {
        public ImmutableArray<Enhance>[] Enhances; // 小附魔基于SubType的分表
        public ImmutableArray<Enhance>[] UseFulEnhances; // 有用的小附魔
        public ImmutableDictionary<EquipSubTypeEnum, ImmutableArray<BigFM>> BigFM; // 大附魔

        public pzEnhanceData()
        {
            var n = EquipMapLib.MAX_SUB_TYPE;
            Enhances = new ImmutableArray<Enhance>[n + 1];
            UseFulEnhances = new ImmutableArray<Enhance>[n + 1];
        }

        public void LoadEnhance(IEnumerable<Enhance> enhance)
        {
            var q = from _ in enhance group _ by _.DestItemSubType;
            foreach (var _ in q)
            {
                var l = _.OrderBy(e => e.Score).ThenBy(e => e.ID).ToImmutableArray();
                Enhances[_.Key] = l;
                UseFulEnhances[_.Key] = l.Where(e => e.Useful.ToBool() && e.Quality > 0).ToImmutableArray();
            }
        }

        public void LoadBigFM(BigFMDBBase db)
        {
            BigFM = db.TypeData;
        }

    }
}