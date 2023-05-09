using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3PZ.Src;
using static JX3CalculatorShared.Utils.ImportTool;

namespace JX3CalculatorShared.Data
{
    public class DataLoader
    {
        protected string DataFile; // TL_Data.xlsx
        protected string OutFile; // TL_Output.xlsx
        protected string ZhenFaFile; // TL_zhenfa.json
        protected string SettingFile; // TL_zhenfa.json

        public ImmutableArray<TargetItem> Target;
        public ImmutableArray<RecipeItem> RecipeMJ;
        public ImmutableArray<RecipeItem> RecipeOther;
        public ImmutableArray<ZhenFa_dfItem> ZhenFa_df;
        public ImmutableDictionary<string, ZhenFa> ZhenFa_dict;
        public ImmutableArray<ItemDTItem> ItemDT;
        public ImmutableArray<Buff_dfItem> Buff_df;
        public ImmutableDictionary<string, QiXueItem> QiXue;
        public ImmutableArray<Enchant> BigFM;
        public ImmutableArray<BottomsFMItem> BottomsFM;
        public ImmutableArray<EquipOptionItem> EquipOptionWP;
        public ImmutableArray<EquipOptionItem> EquipOptionYZ;
        public ImmutableArray<UselessAttr> UselessAttrItems;
        public ImmutableHashSet<string> UselessAttrs;
        public ImmutableDictionary<string, SkillModifier> SkillModifier;
        public ImmutableDictionary<int, SetOptionItem> SetOption;
        public ImmutableArray<AbilityItem> Ability;
        public ImmutableDictionary<string, SkillEventItem> SkillEvent;

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
            ZhenFa_df = ReadSheetAsArray<ZhenFa_dfItem>(OutFile, "ZhenFa_df");
            var res = ReadJSON<Dictionary<string, ZhenFa>>(ZhenFaFile);
            ZhenFa_dict = res.ToImmutableDictionary();
        }

        protected void LoadItemDT()
        {
            ItemDT = ReadSheetAsArray<ItemDTItem>(OutFile, "Item_DT_df");
        }

        protected void LoadBuff_df()
        {
            Buff_df = ReadSheetAsArray<Buff_dfItem>(OutFile, "Buff_df");
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
            
            BottomsFM = ReadSheetAsArray<BottomsFMItem>(OutFile, "Bottoms_FM");
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
                    case "WP":
                        WP.Add(equip_option);
                        break;
                    case "YZ":
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
            SkillModifier = ReadSheetAsDict<string, SkillModifier>(DataFile, "Skill_Modifier", GetName);
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
    }
}