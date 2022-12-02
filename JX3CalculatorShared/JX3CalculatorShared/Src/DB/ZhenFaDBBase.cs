using JX3CalculatorShared.Common;
using JX3CalculatorShared.Src.Class;
using JX3CalculatorShared.Src.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JX3CalculatorShared.Src.DB
{
    public class ZhenFaDBBase: IDB<string, ZhenFa>
    {
        public ImmutableDictionary<string, ZhenFa> Data;
        public ImmutableArray<ZhenFa> ZhenFa;

        public ZhenFaDBBase(IDictionary<string, ZhenFa> zhenFaDict, IEnumerable<ZhenFa_dfItem> zhenFaDf)
        {
            Data = zhenFaDict.ToImmutableDictionary();

            ZhenFa = zhenFaDf.Select(zhenFaDfItem => zhenFaDfItem.Name).Select(name => Data[name]).ToImmutableArray();
        }

        public ZhenFa Get(string name)
        {
            return Data[name];
        }

        public ZhenFa this[string name] => Get(name);
    }
}