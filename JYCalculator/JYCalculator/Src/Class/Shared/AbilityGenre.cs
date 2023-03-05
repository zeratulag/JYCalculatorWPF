using JX3CalculatorShared.Class;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JYCalculator.Class
{
    public class AbilityGenre : AbilityGenreBase
    {
        /// <summary>
        /// 用于存储一系列流派手法技能数的类
        /// </summary>
        public new ImmutableDictionary<int, XFAbility> Data;

        public AbilityGenre(IEnumerable<XFAbility> abilities) : base(abilities)
        {
            Data = abilities.ToImmutableDictionary(_ => _.Rank, _ => _);
        }

        public new XFAbility this[int index] => Data[index];

    }
}