using JX3CalculatorShared.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Class
{

    public class AbilityGenreBase
    {
        /// <summary>
        /// 用于存储一系列流派手法技能数的类
        /// </summary>
        public ImmutableDictionary<int, Ability> Data;
        public readonly string Genre;

        public AbilityGenreBase(IEnumerable<Ability> abilities)
        {
            Genre = abilities.First().Genre;

            Data = abilities.ToImmutableDictionary(_ => _.Rank, _ => _);
        }

        public Ability this[int index] => Data[index];

    }
}