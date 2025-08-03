using JX3CalculatorShared.Globals;
using JX3PZ.Src;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3CalculatorShared.Src.Class;
using static JX3CalculatorShared.Utils.ImportTool;

namespace JX3CalculatorShared.Data
{
    public class DataLoader
    {
        protected string DataFile; // TL_Data.xlsx
        protected string OutFile; // TL_Output.xlsx
        protected string ZhenFaFile; // TL_zhenfa.json
        protected string SettingFile; // TL_zhenfa.json

        public ImmutableArray<TargetItem> Target { get; protected set; }
        public ImmutableArray<RecipeItem> RecipeMJ { get; protected set; }
        public ImmutableArray<RecipeItem> RecipeOther { get; protected set; }
        public ImmutableArray<ZhenFa_dfItem> ZhenFa_df { get; protected set; }
        public ImmutableDictionary<string, ZhenFa> ZhenFa_dict { get; protected set; }
        public ImmutableArray<ItemDTItem> ItemDT { get; protected set; }
        public ImmutableArray<Buff_dfItem> Buff_df { get; protected set; }
        public ImmutableDictionary<string, QiXueItem> QiXue { get; protected set; }

        public ImmutableArray<Enchant> BigFM { get; protected set; }

        //public ImmutableArray<BottomsFMItem> BottomsFM;
        public ImmutableArray<EquipOptionItem> EquipOptionWP { get; protected set; }
        public ImmutableArray<EquipOptionItem> EquipOptionYZ { get; protected set; }
        public ImmutableArray<UselessAttr> UselessAttrItems { get; protected set; }
        public ImmutableHashSet<string> UselessAttrs { get; protected set; }
        public ImmutableDictionary<string, SkillModifier> SkillModifier { get; protected set; }
        public ImmutableDictionary<int, SetOptionItem> SetOption { get; protected set; }
        public ImmutableArray<AbilityItem> Ability { get; protected set; }
        public ImmutableDictionary<string, SkillEventItem> SkillEvent { get; protected set; }
        public ImmutableArray<EquipSpecialEffectItem> EquipSpecialEffectItems { get; protected set; }
        public ImmutableArray<EquipSpecialEffectEntry> EquipSpecialEffectEntries { get; protected set; }
        public static string GetName(AbsGeneralItem item) => item.Name;

        protected void LoadTarget()
        {
            Target = ReadSheetAsArray<TargetItem>(OutFile, "Target");
        }

        protected void LoadRecipe()
        {
            RecipeMJ = ReadSheetAsArray<RecipeItem>(OutFile, "Recipe_MJ");

            foreach (var recipe in RecipeMJ)
            {
                recipe.Type = RecipeTypeEnum.秘籍;
            }

            RecipeOther = ReadSheetAsArray<RecipeItem>(OutFile, "Recipe_Other");
        }

        protected void LoadZhenFa()
        {
            ZhenFa_df = ReadSheetAsArray<ZhenFa_dfItem>(OutFile, "ZhenFa");
            var res = ReadJSON<Dictionary<string, ZhenFa>>(ZhenFaFile);
            ZhenFa_dict = res.ToImmutableDictionary();
        }

        protected void LoadItemDT()
        {
            ItemDT = ReadSheetAsArray<ItemDTItem>(OutFile, "Item_DT");
        }

        protected void LoadBuff_df()
        {
            Buff_df = ReadSheetAsArray<Buff_dfItem>(OutFile, "Buff");
            Buff_df.ForEach(e => e.ParseIDLevel());
        }

        protected void LoadQiXue()
        {
            QiXue = ReadSheetAsDict<string, QiXueItem>(OutFile, "QiXue", _ => _.key);
        }

        protected void LoadBigFM()
        {
            if (StaticPzData.Data.Enchant != null)
            {
                BigFM = StaticPzData.Data.Enchant.Values.ToImmutableArray();
            }
            else
            {
                BigFM = ReadSheetAsArray<Enchant>(OutFile, "Big_FM");
            }

            //BottomsFM = ReadSheetAsArray<BottomsFMItem>(OutFile, "Bottoms_FM");
        }

        protected void LoadEquipOption()
        {
            var equipOptions = ReadSheetAsArray<EquipOptionItem>(OutFile, "Equip_Option");
            List<EquipOptionItem> WP = new List<EquipOptionItem>();
            List<EquipOptionItem> YZ = new List<EquipOptionItem>();
            foreach (var equip_option in equipOptions)
            {
                switch (equip_option.Type)
                {
                    case EquipOptionType.WP:
                        WP.Add(equip_option);
                        break;
                    case EquipOptionType.YZ:
                        YZ.Add(equip_option);
                        break;
                }
            }

            EquipOptionWP = WP.ToImmutableArray();
            EquipOptionYZ = YZ.ToImmutableArray();
        }

        protected void LoadUselessAttrs()
        {
            UselessAttrItems = ReadSheetAsArray<UselessAttr>(DataFile, "UselessAttrs");
            var res = from _ in UselessAttrItems select _.ID;
            UselessAttrs = res.ToImmutableHashSet();
        }

        protected void LoadSkillModifier()
        {
            SkillModifier = ReadSheetAsDict<string, SkillModifier>(DataFile, "SkillModifier", GetName);
            foreach (var _ in SkillModifier.Values)
            {
                _.Parse();
            }
        }

        protected void LoadSetOption()
        {
            SetOption = ReadSheetAsDict<int, SetOptionItem>(OutFile, "Set_Option", _ => _.SetID);
        }

        protected void LoadAbility()
        {
            Ability = ReadSheetAsArray<AbilityItem>(OutFile, "Ability");
        }

        protected void LoadSkillEvent()
        {
            SkillEvent = ReadSheetAsDict<string, SkillEventItem>(OutFile, "Skill_Event", GetName);
            foreach (var _ in SkillEvent.Values)
            {
                _.Parse();
            }
        }

        public void LoadEquipSpecialEffectTable()
        {
            EquipSpecialEffectItems =
                ReadSheetAsArray<EquipSpecialEffectItem>(OutFile, "EquipSpecialEffectTable");
            EquipSpecialEffectItems.ForEach(e => e.Parse());
        }

        public void LoadEquipSpecialEffectEntries()
        {
            EquipSpecialEffectEntries = ReadSheetAsArray<EquipSpecialEffectEntry>(OutFile, "EquipSpecialEffectEntry");
            EquipSpecialEffectEntries.ForEach(e => e.Parse());
        }
    }
}