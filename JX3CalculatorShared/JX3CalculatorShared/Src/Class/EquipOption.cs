using HandyControl.Controls;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using Newtonsoft.Json;
using Syncfusion.Windows.Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Class
{
    public class EquipOption
    {
        public static DamageTypeEnum DamageType;

        #region 成员

        public readonly string Name;
        public string ItemName { get; private set; }
        public readonly EquipOptionType OptionType;
        public readonly string Tag;
        public string ToolTip { get; protected set; }
        public readonly int Order;

        public readonly int Value;
        public readonly int Level;
        public readonly int ExpansionPackLevel;
        public string RawIDs;

        public int BuffID { get; }
        public int BuffLevel { get; }
        public string BuffRawID { get; }
        public string BuffAttributeID { get; }
        public int BuffAttributeValue { get; }
        public int MaxStackNum { get; }
        public int FinalBuffAttributeValue { get; }

        public string[] EquipIDs { get; protected set; } // 记录对应的装备ID

        public readonly string DamageTypeDesc; // 内功/外功文字说明

        public int IconID { get; }
        public int Quality { get; }

        public KAttributeID BuffKAttributeID { get; private set; }

        public readonly bool IsWater; // 是否为水特效
        public readonly bool IsWind; // 是否为风特效
        public readonly bool IsBigCW; // 是否为大橙武

        public CharAttrCollection SCharAttr { get; protected set; }
        public NamedAttrs AttrsDesc { get; protected set; }

        #endregion

        #region 构造

        /// <summary>
        /// 用于描述武器/腰坠类型的类
        /// </summary>
        /// <param name="name">唯一名称</param>
        /// <param name="itemName">物品名称</param>
        /// <param name="tag">计算器Tag</param>
        /// <param name="toolTip"></param>
        /// <param name="iconID"></param>
        /// <param name="order"></param>
        /// <param name="value">数值</param>
        /// <param name="level">装备品级</param>
        /// <param name="quality"></param>
        public EquipOption(string name, string itemName, string tag,
            string toolTip, int iconID, int order,
            int value, int level, int quality)
        {
            Name = name;
            ItemName = itemName;
            Tag = tag;
            ToolTip = toolTip;

            IconID = iconID;
            Order = order;

            Value = value;
            Level = level;
            Quality = quality;

            DamageTypeDesc = DamageType == DamageTypeEnum.Magic ? "内功" : "外功";

            IsWater = Tag == "WaterWeapon";
            IsWind = Tag == "WindYZ";
            IsBigCW = Tag.StartsWith("BigChengWu");
        }

        public EquipOption(EquipOptionItem item)
            : this(item.Name, item.ItemName, item.Tag,
                item.ToolTip, item.IconID, item.Order,
                item.Value, item.Level, item.Quality)
        {
            OptionType = item.Type;
            ExpansionPackLevel = item.ExpansionPackLevel;
            RawIDs = item.EIDs_Str;

            BuffID = item.BuffID;
            BuffLevel = item.BuffLevel;
            BuffRawID = item.BuffRawID;
            BuffAttributeID = item.BuffAttributeID;
            BuffAttributeValue = item.BuffAttributeValue;
            MaxStackNum = item.MaxStackNum;
            FinalBuffAttributeValue = item.FinalBuffAttributeValue;

            if (FinalBuffAttributeValue != 0)
            {
                Value = FinalBuffAttributeValue;
            }

            GetAttributeID();
        }

        #endregion

        #region 方法

        protected virtual string Get_S_ID()
        {
            var res = BuffKAttributeID?.SID;
            return res;
        }

        /// <summary>
        /// 获取属性值，可以与F_Char对象直接相加
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> GetSAtDict()
        {
            var res = new Dictionary<string, double>();

            if (IsWater || IsWind || IsBigCW)
            {
                var key = Get_S_ID();
                if (key != null)
                {
                    res.Add(key, Value);
                }
            }

            return res;
        }

        #endregion

        /// <summary>
        /// 解析RawID列表
        /// </summary>
        /// <param name="TabID">装备表ID，武器为6，服饰为7，饰品为8</param>
        public virtual void ParseEquipIDs(int TabID = 6)
        {
            if (RawIDs.IsNullOrWhiteSpace())
            {
                EquipIDs = null;
                return;
            }

            if (RawIDs.Contains(',') && !RawIDs.StartsWith("["))
            {
                RawIDs = $@"[{RawIDs}]";
            }

            if (RawIDs.StartsWith("["))
            {
                var arr = JsonConvert.DeserializeObject<int[]>(RawIDs);
                var strs = from _ in arr select GetEquipID(TabID, _);
                EquipIDs = strs.ToArray();
            }
            else
            {
                var Id = int.Parse(RawIDs);
                var equipId = GetEquipID(TabID, Id);
                EquipIDs = new[] {equipId};
            }
        }

        public static string GetEquipID(int TabID, string ID)
        {
            return $"{TabID}_{ID}";
        }

        public static string GetEquipID(int TabID, int ID)
        {
            return $"{TabID}_{ID}";
        }

        public void GetAttributeID()
        {
            var res = BuffAttributeID;
            if (res != null)
            {
                BuffKAttributeID = KAttributeID.Get(res);
            }
        }

        public string GetDesc()
        {
            string res = null;
            if (BuffKAttributeID != null)
            {
                if (IsWater)
                {
                    res = $"（每层{Value / 10}{BuffKAttributeID.EquipTag}）";
                }

                if (IsWind)
                {
                    res = $"（{Value}{BuffKAttributeID.EquipTag}）";
                }
            }

            return res;
        }
    }

    public class WPOption : EquipOption
    {
        public const int TabID = 6;


        public readonly bool IsLongMen; // 是否为龙门飞剑

        public ImmutableArray<SkillEventItem> SkillEvents { get; private set; } // 关联的触发事件

        public WPOption(EquipOptionItem item)
            : base(item)
        {
            SCharAttr = new CharAttrCollection(GetSAtDict(), null, true);
            AttrsDesc = SCharAttr.ToNamed(ItemName);
            IsLongMen = Tag.StartsWith("LongMen");
            GetToolTip();
            ParseEquipIDs();
        }

        public void ParseEquipIDs()
        {
            ParseEquipIDs(TabID);
        }

        public void GetEvents(IEnumerable<SkillEventItem> data)
        {
            var res = new List<SkillEventItem>();
            foreach (var _ in data)
            {
                if (_.Type == "武器" && Tag.StartsWith(_.Associate))
                {
                    res.Add(_);
                }
            }

            SkillEvents = res.Distinct(item => item.Name).ToImmutableArray();
        }

        // 获取新的提示
        public void GetToolTip()
        {
            string newstr = "";
            if (Value > 0 && IsWater)
            {
                newstr = $"特效提升{Value}{DamageTypeDesc}攻击力";
            }

            if (Value > 0 && IsBigCW)
            {
                newstr = $"特效提升{Value}无双等级";
            }

            ToolTip = $"{ToolTip}\n{newstr}";
            ;
        }
    }

    public class YZOption : EquipOption
    {
        public const int TabID = 8;
        public readonly bool IsNormal; // 是否为普通腰坠

        public YZOption(EquipOptionItem item)
            : base(item)
        {
            SCharAttr = new CharAttrCollection(GetSAtDict(), null, true);
            AttrsDesc = SCharAttr.ToNamed(ItemName);
            IsNormal = (Value == 0);
            GetToolTip();
            ParseEquipIDs();
        }

        public void ParseEquipIDs()
        {
            ParseEquipIDs(TabID);
        }

        // 获取新的提示
        public void GetToolTip()
        {
            if (Value > 0 && IsWind)
            {
                var newstr = $"使用：提升自身{Value}{DamageTypeDesc}破防，持续15秒，冷却时间180秒。";
                var res = $"{ToolTip}\n{newstr}";
                ToolTip = res;
            }
        }
    }
}