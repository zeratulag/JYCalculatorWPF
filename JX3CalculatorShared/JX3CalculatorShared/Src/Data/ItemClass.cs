using JX3CalculatorShared.Class;
using JX3CalculatorShared.Common;
using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JX3PZ.Globals;


namespace JX3CalculatorShared.Data
{
    public abstract class AbsGeneralItem
    {
        public string Name { get; set; }
    }

    public abstract class AbsToolTipItem : AbsGeneralItem
    {
        public string ToolTip { get; set; }
    }

    public class TargetItem : AbsToolTipItem
    {
        public int Level { get; set; }
        public int Def { get; set; }
        public string ItemName { get; set; }
    }

    /// <summary>
    /// 描述JB奇穴编码的类
    /// </summary>
    public class JBQiXueItem
    {
        public string version { get; set; }
        public string xf { get; set; }
        public string sq { get; set; }
    }

    public class AbilityItem
    {
        public int Rank { get; set; }
        public string ItemName { get; set; }
        public string Desc { get; set; }
        public string ToolTip { get; set; }
    }

    public class AbsSkillDataItem : AbsGeneralItem
    {
        public string SkillName { get; set; }
    }

    public partial class SkillEventItem : AbsGeneralItem
    {
        public string EventName { get; set; }
        public int ID { get; set; }
        public string Type { get; set; }
        public string Associate { get; set; }
        public string DescName { get; set; }
        public SkillEventTypeEnum EventType { get; set; }
        public int Odds { get; set; }
        public int SkillID { get; set; }
        public int SkillLevel { get; set; }
        public ulong EventMask1 { get; set; }
        public ulong EventMask2 { get; set; }
        public int EventSkillID { get; set; }
        public int EventSkillLevel { get; set; }
        public double Prob { get; private set; } // 触发概率
        public double Time { get; set; } // 持续时间
        public double CD { get; set; } // CD时间
    }

    public class RecipeSkillMapItem
    {
        public string Key { get; set; }
        public string rawIDs { get; set; }

        public ImmutableArray<string> SkillIDs;

        public void Parse()
        {
            var res = from skillId in rawIDs.Split(",") select skillId.Trim();
            SkillIDs = res.ToImmutableArray();
        }
    }

    public class UselessAttr
    {
        public string ID { get; set; }
    }

    public class SetOptionItem
    {
        public int SetID { get; set; }
        public string SetName { get; set; }
        public string EID_Str { get; set; }
        public string Effect2 { get; set; }
        public string Effect4 { get; set; }
    }

    public abstract class AbsIconItem : AbsGeneralItem
    {
        public int IconID { get; set; } = -1;
    }

    public abstract class AbsIconToolTipItem : AbsToolTipItem
    {
        public int IconID { get; set; }
    }

    public class AbilitySkillNumItemBase
    {
        public string key { get; set; }
        public int XW { get; set; }
        public int Rank { get; set; }
        public double Time { get; set; }
        public string Genre { get; set; } // 流派

        public static string GetKey(string genre, int xw, int rank)
        {
            var res = $"{genre}#{xw}_{rank}";
            return res;
        }

        public string GetKey()
        {
            return GetKey(Genre, XW, Rank);
        }
    }

    public abstract class AbsBuffItem : AbsIconToolTipItem, ILuaTable
    {
        public static TabParser Parser =
            new TabParser("At_key{0:D}", "At_value{0:D}", AttributeIDLoader.AttributeIsValue);

        public AttrCollection ParseItem()
        {
            return Parser.ParseItem(this);
        }

        public string DescName { get; set; }
        public string ItemName { get; set; }
        public string ToolTipDesc { get; set; }
        public string Source { get; set; }

        public string At_key1 { get; set; }
        public int? At_value1 { get; set; }

        public string At_key2 { get; set; }
        public int? At_value2 { get; set; }
    }

    public class Buff_dfItem : AbsBuffItem
    {
        public string BuffID { get; set; }
        public string Associate { get; set; }
        public double DefaultCover { get; set; }
        public int DefaultStackNum { get; set; }
        public int MaxStackNum { get; set; }
        public int Order { get; set; }
        public int AppendType { get; set; }
        public int Intensity { get; set; }
        public BuffTypeEnum Type { get; set; }
        public bool IsTarget => (this.Type == BuffTypeEnum.DeBuff_Normal);
        public int Interval { get; set; }
        public string At_key3 { get; set; }
        public int? At_value3 { get; set; }
        public string At_key4 { get; set; }
        public int? At_value4 { get; set; }
        public string At_key5 { get; set; }
        public int? At_value5 { get; set; }
        public string At_key6 { get; set; }
        public int? At_value6 { get; set; }
        public string At_key7 { get; set; }
        public int? At_value7 { get; set; }

        public int ID { get; private set; }
        public int Level { get; private set; }

        public void MakeIDLevel()
        {
            var res = BaseBuff.ParseIDLevel(BuffID);
            ID = res.ID;
            Level = res.Level;
        }

        public void ParseIDLevel() // [TODO] 数据中直接带上ID和Level，避免运行时解析
        {
            MakeIDLevel();
        }
    }

    public class ItemDTItem : AbsBuffItem
    {
        public int UIID { get; set; }
        public int Quality { get; set; }
        public string ItemNameM { get; set; }
        public string 类型 { get; set; }
        public int ExpansionPackLevel { get; set; }
        public ItemDTTypeEnum Type { get; set; }
        public int BuffEnchantID { get; set; }
        public int Level { get; set; }
    }

    public class QiXueItem : AbsIconToolTipItem
    {
        public string key { get; set; }
        public int position { get; set; }
        public int order { get; set; }
        public int SkillID { get; set; }
        public int Level { get; set; }
        public string ItemName { get; set; }
        public string ShortName => ItemName.Substring(0, 2);
    }

    public class ZhenFa_dfItem : AbsToolTipItem
    {
        public string ItemName { get; set; }
    }

    public class AbsEnchant : AbsIconToolTipItem
    {
        public int UIID { get; set; }
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int ExpansionPackLevel { get; set; } = StaticConst.CurrentLevel;
    }

    public class Enchant : AbsEnchant
    {
        public int DestItemSubType { get; set; }
        public EquipSubTypeEnum SubType { get; set; }
        public int Quality { get; set; }
        public int Score { get; set; }
        public int Level_Min { get; set; }
        public int Level_Max { get; set; }
        public int Rank { get; set; }
        public int Magic { get; set; }
        public int Physics { get; set; }
        public string EnhanceDesc { get; set; }
    }

    public class BottomsFMItem : AbsEnchant
    {
        public string DescName { get; set; }
        public int Quality { get; set; }
        public string Desc { get; set; }
        public int Level_Max { get; set; }
        public int Rank { get; set; }
    }

    public class EquipOptionItem : AbsIconToolTipItem
    {
        public EquipOptionType Type { get; set; }
        public string ItemName { get; set; }
        public int Value { get; set; }
        public string Tag { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public int Quality { get; set; }
        public int ExpansionPackLevel { get; set; }

        public int BuffID { get; set; }
        public int BuffLevel { get; set; }
        public string BuffRawID { get; set; }
        public string BuffAttributeID { get; set; }
        public int BuffAttributeValue { get; set; } = 0;
        public int MaxStackNum { get; set; } = 0;
        public int FinalBuffAttributeValue { get; set; } = 0;

        public string EIDs_Str { get; set; }
    }

    public class RecipeItem : AbsIconToolTipItem, ILuaTable
    {
        public static TabParser Parser =
            new TabParser("SAt_key{0:D}", "SAt_value{0:D}", AttributeIDLoader.SkillAttributeIsValue);

        public AttrCollection ParseItem()
        {
            var collect = Parser.ParseItem(this);
            return collect;
        }

        public int RecipeID { get; set; }
        public int RecipeLevel { get; set; }
        public int SkillRecipeType { get; set; }
        public int SkillID { get; set; }
        public string SAt_key1 { get; set; }
        public int SAt_value1 { get; set; }
        public string SAt_key2 { get; set; }
        public int SAt_value2 { get; set; }
        public string SAt_key3 { get; set; }
        public int SAt_value3 { get; set; }
        public string SAt_key4 { get; set; }
        public int SAt_value4 { get; set; }
        public string DescName { get; set; }
        public string RecipeName { get; set; }
        public string obj_name { get; set; }
        public string Skill_key { get; set; }
        public RecipeTypeEnum Type { get; set; }
        public string ShortDesc { get; set; }
        public int TypeID { get; set; }
        public int Quality { get; set; }
        public int ItemID { get; set; }
        public int DesignLevel { get; set; }
        public string Associate { get; set; }
        public bool IsExclude { get; set; } = false;
        public string SkillNameTag { get; set; }

        public string RawSkillNames { get; set; } = ""; // 当此项不为空时，会直接解析生效的技能名；否则需要对比SkillID进行求解生效技能名

        public static string GetToolTipTail(int ID, int Level)
        {
            string tail = "";
            var id = GlobalFunctions.MergeIDLevel(ID, Level);
            if (ID > 0)
            {
                tail = $"\n\nID: {id}";
            }

            return tail;
        }

        public string GetToolTipTail()
        {
            return GetToolTipTail(RecipeID, RecipeLevel);
        }
    }

    public class DiamondValueItemBase
    {
        public int Level { get; set; }
        public int BaseAttackPower { get; set; } // 基础攻击
        public int BaseOvercome { get; set; } // 基础破防
        public int CriticalStrike { get; set; } // 会心等级
        public int CriticalPower { get; set; } // 会效等级
        public int BaseSurplus { get; set; } // 基础破招值
        public int BaseStrain { get; set; } // 基础无双值
    }

    public class SkillBuildItem : AbsGeneralItem
    {
        public string Build { get; set; }
        public string GameVersion { get; set; }
        public int IsDefault { get; set; } = 0;
        public int Priority { get; set; } = 0;
        public string RawEssentialQiXues { get; set; }
        public string RawBannedQiXues { get; set; }
        public string RawEssentialMiJis { get; set; }
        public string RawSav { get; set; }
    }

    public partial class EquipSpecialEffectItem : AbsIconToolTipItem
    {
        public string EID { get; set; }
        public int ID { get; set; } = -1;
        public string EquipName { get; set; }
        public int EquipSubType { get; set; } = -1;
        public EquipSubTypeEnum SubType { get; set; }
        public int Level { get; set; } = -1;
        public int UIID { get; set; } = -1;
        public int Quality { get; set; } = -1;
        public int EquipSpecialEffectEventID { get; set; } = -1;
        public string EquipSpecialEffectEventDesc { get; set; }
        public string EquipSpecialEffectName { get; set; }
        public EquipSpecialEffectTypeEnum SpecialEffectType { get; set; }
        public EquipSpecialEffectBaseTypeEnum SpecialEffectBaseType { get; set; }
        public string DescName { get; set; }
        public string EventName { get; set; }
        public bool NeedCalcEventFreq { get; set; }
        public string ItemName { get; set; }
        // 以下属性由解析得到
        public bool IsValid { get; private set; }
        public string SpecialEffectColor { get; private set; }
        public EquipSpecialEffectEntry SpecialEffectEntry { get; private set; }

        public static bool IsValidItem(EquipSpecialEffectItem item)
        {
            return item != null && item.Level > 0;
        }

        public void Parse()
        {
            IsValid = IsValidItem(this);
            SpecialEffectColor = IsValid ? ColorConst.Orange : ColorConst.Default;
        }

        public void AttachEquipSpecialEffectEntry(EquipSpecialEffectEntry entry)
        {
            SpecialEffectEntry = entry;
        }

    }

    public partial class EquipSpecialEffectEntry: AbsGeneralItem
    {
        public int EquipLevel { get; set; } = -1;
        public int SkillEventID { get; set; } = -1;
        public int SkillID { get; set; } = -1;
        public int SkillLevel { get; set; } = -1;
        public EquipSpecialEffectTypeEnum SpecialEffectType { get; set; }
        public EquipSpecialEffectBaseTypeEnum SpecialEffectBaseType { get; set; }
        public string SkillScriptFileName { get; set; }
        public string Sample { get; set; }
        public string Comment { get; set; }
        public int CD { get; set; } = -1;
        public int DamageSkillID { get; set; } = -1;
        public int DamageSkillILevel { get; set; } = -1;
        public int BuffTime1 { get; set; } = -1;
        public int BuffID1 { get; set; } = -1;
        public int BuffLevel1 { get; set; } = -1;
        public int BuffStack1 { get; set; } = -1;
        public int BuffCover1 { get; set; } = -1;
        public int BuffID2 { get; set; } = -1;
        public int BuffLevel2 { get; set; } = -1;
        public int BuffStack2 { get; set; } = -1;
        public int BuffCover2 { get; set; } = -1;
        public int BuffID3 { get; set; } = -1;
        public int BuffLevel3 { get; set; } = -1;
        public int BuffStack3 { get; set; } = -1;
        public int BuffCover3 { get; set; } = -1;
        public string DescName { get; set; }
        public bool NeedCalcEventFreq { get; set; }
        public string EventName { get; set; }

        public bool IsSnapAdaptiveBuff { get; private set; }

        public SkillEventItem SkillEvent { get; private set; }
    }
}