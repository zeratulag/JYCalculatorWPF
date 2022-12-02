using JX3CalculatorShared.Class;
using JYCalculator.Src.Class;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public class AbilityGenre : AbilityGenreBase
    {
        /// <summary>
        /// 用于存储一系列流派手法技能数的类
        /// </summary>
        public new ImmutableDictionary<int, JYAbility> Data;

        public AbilityGenre(IEnumerable<JYAbility> abilities) : base(abilities)
        {
            Data = abilities.ToImmutableDictionary(_ => _.Rank, _ => _);
        }

        public new JYAbility this[int index] => Data[index];

    }
}