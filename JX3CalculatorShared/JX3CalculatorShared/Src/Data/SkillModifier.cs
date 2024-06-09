using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Data
{
    public class SkillModifier : AbsGeneralItem, ILuaTable
    {
        public static TabParser Parser = new TabParser("SAt_key{0:D}", "SAt_value{0:D}", AttributeIDLoader.SkillAttributeIsValue);

        public string RawSkillNames { get; set; }
        public string Type { get; set; }
        public string DescName { get; set; }
        public string Associate { get; set; }
        public string SAt_key1 { get; set; }
        public double SAt_value1 { get; set; }
        public string SAt_key2 { get; set; }
        public double SAt_value2 { get; set; }
        public string SAt_key3 { get; set; }
        public double SAt_value3 { get; set; }

        public ImmutableHashSet<string> SkillNames { get; private set; } // 可以被此Mod修饰的技能Name
        public Dictionary<string, double> SAttrs { get; private set; } // 数值属性

        public SkillModifier() : base()
        {

        }


        public SkillModifier(SkillModifier old, double k = 1)
        {
            // 复制构造函数
            RawSkillNames = old.RawSkillNames;
            Name = old.Name;
            Type = old.Type;
            string descName = old.DescName;
            DescName = old.DescName;
            Associate = old.Associate;
            SkillNames = old.SkillNames.ToImmutableHashSet();
            SAttrs = old.SAttrs.Copy();

            if (k != 1)
            {
                descName += $"[x{k:F2}]";
                MultiplyEffect(k);
            }

            DescName = descName;
        }


        public AttrCollection ParseItem()
        {
            return Parser.ParseItem(this);
        }

        public void ParseNames()
        {
            var res = StringTool.ParseStringList(RawSkillNames);
            SkillNames = res.ToImmutableHashSet();
        }

        public void PasteValueDictionary()
        {
            SAttrs = Parser.ParseValueDictItem(this);
        }

        public void Parse()
        {
            ParseNames();
            PasteValueDictionary();
        }

        /// <summary>
        /// 效果变为k倍
        /// </summary>
        /// <param name="k">倍率</param>
        public void MultiplyEffect(double k)
        {
            SAttrs.MultiplyEffect(k);
        }

        public SkillModifier Emit(double k)
        {
            var res = new SkillModifier(this, k);
            return res;
        }
    }
}