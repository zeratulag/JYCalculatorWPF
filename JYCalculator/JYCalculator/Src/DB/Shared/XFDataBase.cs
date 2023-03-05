using JYCalculator.Data;
using System.Collections.Immutable;

namespace JYCalculator.DB
{
    public class XFDataBase
    {
        #region 成员

        public readonly TargetDB Target;
        public readonly AbilityDB Ability;
        public readonly ZhenFaDB ZhenFa;
        public EquipOptionDB EquipOption { get; set; }
        public readonly BigFMDB BigFM;

        public readonly BuffDB Buff;
        public readonly ItemDTDB ItemDT;

        public readonly SkillInfoDB SkillInfo;
        public readonly QiXueDB QiXue;
        public readonly RecipeDB Recipe;


        public static readonly ImmutableHashSet<string> UselessAttrs = StaticXFData.Data.UselessAttrs;

        public readonly SkillModifierDB SkillModifier;

        public readonly AttrWeightDB AttrWeight;

        public readonly SetOptionDB SetOption;

        public XFDataBase(TargetDB target, AbilityDB ability,
            ZhenFaDB zhenFa,
            EquipOptionDB equipOption, BigFMDB bigFM,
            BuffDB buff, ItemDTDB itemDT,
            SkillInfoDB skillInfo,
            QiXueDB qiXue, RecipeDB recipe,
            SkillModifierDB skillModifier, AttrWeightDB attrWeight,
            SetOptionDB setOption)
        {
            Target = target;
            Ability = ability;

            ZhenFa = zhenFa;

            EquipOption = equipOption;
            BigFM = bigFM;

            Buff = buff;
            ItemDT = itemDT;

            SkillInfo = skillInfo;
            QiXue = qiXue;
            Recipe = recipe;

            SkillInfo.AttachRecipeDB(Recipe);
            SkillInfo.Process();

            SkillModifier = skillModifier;

            EquipOption.AttachWPSkillEvents(SkillInfo);

            AttrWeight = attrWeight;
            SetOption = setOption;
        }

        #endregion

        #region 构造

        public XFDataBase() : this(
            new TargetDB(), new AbilityDB(),
            new ZhenFaDB(),
            new EquipOptionDB(), new BigFMDB(),
            new BuffDB(), new ItemDTDB(),
            new SkillInfoDB(),
            new QiXueDB(), new RecipeDB(),
            new SkillModifierDB(),
            new AttrWeightDB(),
            new SetOptionDB())
        {
        }

        #endregion
    }
}