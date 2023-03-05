using JX3CalculatorShared.Common;
using JX3CalculatorShared.Data;
using JYCalculator.Class;
using JYCalculator.Data;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JYCalculator.DB
{
    public class AbilityDB
    {
        #region 成员

        public readonly ImmutableArray<AbilityItem> AbilityItem;
        public readonly ImmutableArray<XFAbility> Ability;

        public readonly ImmutableArray<AbilitySkillNumItem> Normal;
        public readonly ImmutableArray<AbilitySkillNumItem> BigXW;

        public readonly ImmutableDictionary<string, AbilitySkillNumItem> SkillNumData;

        public readonly ImmutableDictionary<string, AbilityGenre> GenreData; // 存储了不同流派的基础技能数

        #endregion

        #region 构造

        public static ImmutableArray<AbilitySkillNumItem> GetSkillNum(
            IEnumerable<AbilitySkillNumItem> abilitySkillNum,
            int isXW = 0)
        {
            var res = from skillnum in abilitySkillNum
                      orderby skillnum.Rank
                      where skillnum.XW == isXW
                      select skillnum;
            return res.ToImmutableArray();
        }

        public static Period<Dictionary<int, AbilitySkillNumItem>> GetPeriodData(
            IEnumerable<AbilitySkillNumItem> abilitySkillNum)
        {
            var Normal = new Dictionary<int, AbilitySkillNumItem>();
            var XW = new Dictionary<int, AbilitySkillNumItem>();
            foreach (var row in abilitySkillNum)
            {
                switch (row.XW)
                {
                    case 0:
                        Normal.Add(row.Rank, row);
                        break;
                    case 1:
                        XW.Add(row.Rank, row);
                        break;
                }
            }

            var Res = new Period<Dictionary<int, AbilitySkillNumItem>>(Normal, XW);
            return Res;
        }


        public AbilityDB(IEnumerable<AbilityItem> abilitydata,
            IEnumerable<AbilitySkillNumItem> abilitySkillNum)
        {
            var AllSkillNum = new Dictionary<string, AbilitySkillNumItem>();
            var AllAbility = new List<XFAbility>();

            var genresB = ImmutableDictionary.CreateBuilder<string, AbilityGenre>();

            var groups = abilitySkillNum.GroupBy(_ => _.Genre);
            foreach (var g in groups)
            {
                var periodAbilitySkillNum = GetPeriodData(g);
                var res = new Dictionary<int, XFAbility>();
                foreach (var abilityItem in abilitydata)
                {
                    var i = abilityItem.Rank;
                    var normal = periodAbilitySkillNum.Normal[i];
                    var bigxw = periodAbilitySkillNum.XW[i];
                    var value = new XFAbility(abilityItem, normal, bigxw);
                    res.Add(i, value);


                    AllSkillNum.Add(normal.GetKey(), normal);
                    AllSkillNum.Add(bigxw.GetKey(), bigxw);
                    AllAbility.Add(value);
                }


                var abilityGenre = new AbilityGenre(res.Values);
                genresB.Add(abilityGenre.Genre, abilityGenre);
            }

            GenreData = genresB.ToImmutable();
            AbilityItem = abilitydata.OrderBy(_ => _.Rank).ToImmutableArray();

            Ability = AllAbility.OrderBy(_ => _.Rank).ThenBy(_ => _.Rank).ToImmutableArray();

            SkillNumData = AllSkillNum.ToImmutableDictionary();

            Normal = GetSkillNum(abilitySkillNum, 0);
            BigXW = GetSkillNum(abilitySkillNum, 1);
        }

        public AbilityDB(XFDataLoader xfDataLoader) : this(xfDataLoader.Ability,
            xfDataLoader.AbilitySkillNum)
        {
        }

        public AbilityDB() : this(StaticXFData.Data)
        {
        }

        #endregion

        #region 取出

        public AbilitySkillNumItem Get(string name)
        {
            return SkillNumData[name];
        }

        #endregion
    }
}