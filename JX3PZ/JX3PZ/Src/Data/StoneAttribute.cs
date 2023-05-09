using JX3CalculatorShared.Class;
using JX3PZ.Class;
using JX3PZ.Globals;
using System.Collections.Immutable;
using System.Linq;
using JX3PZ.ViewModels;

namespace JX3PZ.Data
{
    public class StoneAttribute
    {
        public ImmutableArray<string> AttributeIDs;
        public ImmutableArray<int> AttributeValues;
        public ImmutableArray<AttributeEntry> AttributeEntries;
        public ImmutableArray<StoneCondition> Conditions;
        public readonly bool IsValid; // 是否不为空
        public readonly int ValidAttributesCount; // 有效属性数

        public StoneAttribute(Stone s)
        {
            IsValid = s.Quality > 0;
            var attrs = new string[] {s.Attribute1ID, s.Attribute2ID, s.Attribute3ID};
            AttributeIDs = attrs.Where(_ => _.IsNotEmptyOrWhiteSpace()).ToImmutableArray();
            ValidAttributesCount = AttributeIDs.Length;
            var values = new int[] {s.Attribute1Value, s.Attribute2Value, s.Attribute3Value};

            var validValues = ImmutableArray.CreateBuilder<int>();

            var entries = ImmutableArray.CreateBuilder<AttributeEntry>();

            for (int i = 0; i < AttributeIDs.Length; i++)
            {
                validValues.Add(values[i]);
                var e = new AttributeEntry(AttributeIDs[i], values[i], AttributeEntryTypeEnum.Stone);
                e.GetDesc();
                entries.Add(e);
            }

            AttributeValues = validValues.ToImmutableArray();
            AttributeEntries = entries.ToImmutableArray();

            Conditions = ParseConditions(s, ValidAttributesCount);
        }

        public static ImmutableArray<StoneCondition> ParseConditions(Stone s, int validAttributes)
        {
            var full = new StoneCondition[3];
            full[0] = new StoneCondition(s.DiamondType1, s.Compare1, s.DiamondCount1, s.DiamondIntensity1);
            full[1] = new StoneCondition(s.DiamondType2, s.Compare2, s.DiamondCount2, s.DiamondIntensity2);
            full[2] = new StoneCondition(s.DiamondType3, s.Compare3, s.DiamondCount3, s.DiamondIntensity3);
            var res = full.Take(validAttributes).ToImmutableArray();
            return res;
        }

        /// <summary>
        /// 判断各个词条的激活状态
        /// </summary>
        /// <param name="diamondCount">全身五行石个数</param>
        /// <param name="diamondIntensity">全身五行石等级和</param>
        /// <returns></returns>
        public bool[] IsActive(int diamondCount, int diamondIntensity)
        {
            var res = Conditions.Select(_ => _.IsActive(diamondCount, diamondIntensity)).ToArray();
            return res;
        }

        public ImmutableArray<StoneAttributeEntryViewModel> GetAttributeEntryViewModels()
        {
            var res = AttributeEntries.Select(_ => new StoneAttributeEntryViewModel(_)).ToImmutableArray();
            return res;
        }

        public bool HasHaste => AttributeEntries.Any(entry => entry.IsHaste); // 是否有加速

    }
}