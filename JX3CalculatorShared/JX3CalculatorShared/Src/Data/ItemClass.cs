using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Linq;
using JX3CalculatorShared.Common;
using Newtonsoft.Json;


namespace JX3CalculatorShared.Src.Data
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

    public class AbsSkillDataItem
    {
        public string Name { get; set; }
        public string Skill_Name { get; set; }
    }

    public partial class SkillInfoItem : AbsSkillDataItem
    {
        public string Fight_Name { get; set; }
        public double Interval { get; set; }
        public int Frame { get; set; }
        public int nCount { get; set; } = 1;
        public double CostEnergy { get; set; } = 0;
        public double CD { get; set; } = 0;
        public double Fixed_Min { get; set; } = 0;
        public double Fixed_Max { get; set; } = 0;
        public double Fixed_Dmg { get; set; } = 0;
        public double nChannelInterval { get; set; }
        public double AP_Coef { get; set; } = 0;
        public double WP_Coef { get; set; } = 0;
        public double Add_Dmg { get; set; } = 0;
        public double Add_CT { get; set; } = 0;
        public double Add_CF { get; set; } = 0;
        public double IgnoreB { get; set; } = 0;
        public int IconID { get; set; }
        public int SkillID { get; set; }
        public int Level { get; set; }
        public int RecipeType { get; set; }
        public ulong SkillEventMask1 { get; set; }
        public ulong SkillEventMask2 { get; set; }
        public int Cast_SkillID { get; set; }
        public ulong Cast_SkillEventMask1 { get; set; }
        public ulong Cast_SkillEventMask2 { get; set; }

        public SkillDataTypeEnum Type { get; set; }
    }

    public partial class SkillEventItem : AbsGeneralItem
    {
        public string EventName { get; set; }
        public int ID { get; set; }
        public string Type { get; set; }
        public string Associate { get; set; }
        public string DescName { get; set; }
        public string EventType { get; set; }
        public int Odds { get; set; }
        public int SkillID { get; set; }
        public int SkillLevel { get; set; }
        public ulong EventMask1 { get; set; }
        public ulong EventMask2 { get; set; }
        public int EventSkillID { get; set; }
        public int EventSkillLevel { get; set; }
        public HashSet<string> TriggerSkillNames { get; private set; } // 可以触发事件的技能Name
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
        public string RawEquipIDs { get; set; }
        public string Effect2 { get; set; }
        public string Effect4 { get; set; }
    }

    public abstract class AbsIconItem : AbsGeneralItem
    {
        public int IconID { get; set; } = -1;
        public string IconPath => BindingTool.IconID2Path(IconID);
    }

    public abstract class AbsIconToolTipItem : AbsToolTipItem
    {
        public int IconID { get; set; }
        public string IconPath => BindingTool.IconID2Path(IconID);
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
        public static TabParser Parser = new TabParser("At_key{0:D}", "At_value{0:D}", AtLoader.At_is_Value);

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
    }

    public class ItemDTItem : AbsBuffItem
    {
        public int ItemID { get; set; }
        public int Quality { get; set; }
        public string ItemNameM { get; set; }
        public string 类型 { get; set; }
        public int DLCLevel { get; set; }
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
        public string ItemNameP => StringTool.PadQiXueItemName(ItemName);
        public string ShortName => ItemName.Substring(0, 2);
    }

    public class ZhenFa_dfItem : AbsToolTipItem
    {
        public string ItemName { get; set; }
    }

    public class AbsBigFMItem : AbsIconToolTipItem
    {
        public int ItemID { get; set; }
        public int Enchant_ID { get; set; }
        public string ItemName { get; set; }
        public int DLCLevel { get; set; } = StaticData.CurrentLevel;
    }

    public class BigFMItem : AbsBigFMItem
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
    }

    public class BottomsFMItem : AbsBigFMItem
    {
        public string DescName { get; set; }
        public int Quality { get; set; }
        public string Desc { get; set; }
        public int Level_Max { get; set; }
        public int Rank { get; set; }
    }

    public class EquipOptionItem : AbsIconToolTipItem
    {
        public string Type { get; set; }
        public string ItemName { get; set; }
        public int Value { get; set; }
        public string Tag { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public int Quality { get; set; }
        public int DLCLevel { get; set; }
        public string RawIDs { get; set; }
    }

    public class RecipeItem : AbsIconToolTipItem, ILuaTable
    {
        public static TabParser Parser = new TabParser("SAt_key{0:D}", "SAt_value{0:D}", AtLoader.SkillAt_is_Value);

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

        public string RawSkillNames { get; set; } = ""; // 当此项不为空时，会直接解析生效的技能名；否则需要对比SkillID进行求解生效技能名

        public static string GetToolTipTail(int ID, int Level)
        {
            string tail = "";
            var id = Funcs.MergeIDLevel(ID, Level);
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
        public int AP { get; set; }
        public int OC { get; set; }
        public int CF { get; set; }
        public int CT { get; set; }
        public int PZ { get; set; }
        public int WS { get; set; }
    }


    public class CalcSetting
    {
        /// <summary>
        /// 计算器设置
        /// </summary>
        public string RawEssentialQiXues { get; set; }

        public string RawBannedQiXues { get; set; }

        public string RawEssentialMiJis { get; set; }

        // 以下为生成属性

        [JsonIgnore] public ImmutableHashSet<string> EssentialQiXues { get; protected set; }

        [JsonIgnore] public ImmutableHashSet<string> BannedQiXues { get; protected set; }

        [JsonIgnore] public ImmutableHashSet<string> EssentialMiJis { get; protected set; }

        public void Parse()
        {
            EssentialQiXues = StringTool.ParseStringList(RawEssentialQiXues).ToImmutableHashSet();
            BannedQiXues = StringTool.ParseStringList(RawBannedQiXues).ToImmutableHashSet();
            EssentialMiJis = StringTool.ParseStringList(RawEssentialMiJis).ToImmutableHashSet();
        }
    }
}