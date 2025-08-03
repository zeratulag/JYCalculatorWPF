using JYCalculator.Data;
using System.Collections.Immutable;
using System.Runtime.InteropServices.ComTypes;
using JX3CalculatorShared.DB;
using JYCalculator.Src.Class;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JYCalculator.Class;

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

        public readonly SkillInfoDB BaseSkillInfo; // 基础技能信息
        public SkillInfoDB AllSkillInfo { get; private set; }

        public readonly QiXueDB QiXue;
        public readonly RecipeDB Recipe;

        public static readonly ImmutableHashSet<string> UselessAttrs = StaticXFData.Data.UselessAttrs;

        public readonly SkillModifierDB SkillModifier;

        public readonly AttrWeightDB AttrWeight;

        public readonly SetOptionDB SetOption;

        public readonly SkillBuildDB SkillBuild;

        public readonly SkillEventDB SkillEvent;
        public readonly SkillEventTypeMaskDB SkillEventTypeMasks;

        public readonly EquipSpecialEffectItemDB EquipSpecialEffectItems;

        public readonly EquipSpecialEffectEntryDB EquipSpecialEffectEntries;

        public void ProcessSkillInfoDB(SkillInfoDB db)
        {
            db.AttachRecipeDB(Recipe);
            db.Process();
            EquipOption.AttachWPSkillEvents(db);
        }

        #endregion

        #region 构造

        public XFDataBase()
        {
            Target = new TargetDB();
            Ability = new AbilityDB();
            ZhenFa = new ZhenFaDB();

            EquipOption = new EquipOptionDB();
            BigFM = new BigFMDB();

            Buff = new BuffDB();
            ItemDT = new ItemDTDB();

            QiXue = new QiXueDB();
            Recipe = new RecipeDB();

            BaseSkillInfo = new SkillInfoDB();
            //BaseSkillInfo.AttachRecipeDB(Recipe);
            //BaseSkillInfo.Process();
            //EquipOption.AttachWPSkillEvents(BaseSkillInfo);
            ProcessSkillInfoDB(BaseSkillInfo);

            SkillModifier = new SkillModifierDB();

            AttrWeight = new AttrWeightDB();
            SetOption = new SetOptionDB();

            SkillBuild = new SkillBuildDB();
            EquipSpecialEffectItems = new EquipSpecialEffectItemDB();
            EquipSpecialEffectEntries = new EquipSpecialEffectEntryDB();

            SkillEventTypeMasks = new SkillEventTypeMaskDB();
            SkillEvent = new SkillEventDB();
            AttachSkillEventToEquipSpecialEffectEntry();
            AttachEquipSpecialEffectEntryToEquipSpecialEffectItems();
        }

        public void AttachSkillEventToEquipSpecialEffectEntry()
        {
            EquipSpecialEffectEntries.AttachSkillEvent(SkillEvent);
        }

        #endregion

        /// <summary>
        /// 为BuffToRecipeItem对象添加Recipe和SkillModifier关联
        /// </summary>
        /// <param name="item"></param>
        public void AttachBuffToRecipeItem(BuffToRecipeItem item)
        {
            if (item.RecipeID > 0)
            {
                item.CRecipe = Recipe.GetRecipeByIdLevel(item.RecipeID, item.RecipeLevel);
            }

            if (item.SkillModifierName != null)
            {
                item.CModifier = SkillModifier[item.SkillModifierName];
            }
        }

        // 构建全量技能信息库
        public void BuildAllSkillInfo()
        {
            AllSkillInfo = new SkillInfoDB(StaticXFData.Data.AllSkillInfoData, StaticXFData.Data.SkillEvent);
            ProcessSkillInfoDB(AllSkillInfo);
        }

        public void AttachEquipSpecialEffectEntryToEquipSpecialEffectItems()
        {
            EquipSpecialEffectItems.AttachEquipSpecialEffectEntry(EquipSpecialEffectEntries);
        }

    }
}