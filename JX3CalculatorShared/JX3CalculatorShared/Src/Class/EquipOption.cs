using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;
using Newtonsoft.Json;
using Syncfusion.Windows.Shared;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace JX3CalculatorShared.Src.Class
{
    public class EquipOption
    {

        public static DamageTypeEnum DamageType;

        #region 成员

        public static readonly ImmutableDictionary<DamageTypeEnum, string> AttrMap;
        public readonly string Name;
        public string ItemName { get; private set; }
        public readonly string Tag;
        public string ToolTip { get; protected set; }
        public readonly int Order;

        public readonly int Value;
        public readonly int Level;
        public readonly int DLCLevel;
        public string RawIDs;


        public string[] EquipIDs { get; protected set; } // 记录对应的装备ID

        public readonly string DamageTypeDesc; // 内功/外功文字说明

        public int IconID { get; }
        public int Quality { get; }
        public string IconPath { get; }

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
            IconPath = BindingTool.IconID2Path(IconID);
            Order = order;

            Value = value;
            Level = level;
            Quality = quality;


            DamageTypeDesc = DamageType == DamageTypeEnum.Magic ? "内功" : "外功";
        }

        public EquipOption(EquipOptionItem item)
            : this(item.Name, item.ItemName, item.Tag,
                item.ToolTip, item.IconID, item.Order,
                item.Value, item.Level, item.Quality)
        {
            DLCLevel = item.DLCLevel;
            RawIDs = item.RawIDs;
        }

        #endregion

        #region 方法

        protected virtual string GetS_ID()
        {
            var res = AttrMap.GetValueOrDefault(DamageType);
            return res;
        }

        /// <summary>
        /// 获取属性值，可以与F_Char对象直接相加
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> GetSAtDict()
        {
            var res = new Dictionary<string, double>();

            if (Value > 0)
            {
                var key = GetS_ID();
                res.Add(key, Value);
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
            }
            else if (RawIDs.StartsWith("["))
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
    }

    public class WPOption : EquipOption
    {
        public const int TabID = 6;
        public readonly CharAttrCollection SCharAttr;
        public readonly NamedAttrs AttrsDesc;

        public readonly bool IsBigCW; // 是否为大橙武

        public ImmutableArray<SkillEventItem> SkillEvents { get; private set; } // 关联的触发事件

        public new static readonly ImmutableDictionary<DamageTypeEnum, string> AttrMap =
            new Dictionary<DamageTypeEnum, string>()
            {
                {DamageTypeEnum.Magic, "M_Base_AP"},
                {DamageTypeEnum.Physics, "P_Base_AP"}
            }.ToImmutableDictionary();

        public WPOption(EquipOptionItem item)
            : base(item)
        {
            SCharAttr = new CharAttrCollection(GetSAtDict(), null, true);
            AttrsDesc = SCharAttr.ToNamed(ItemName);
            IsBigCW = Name == "Big_CW";
            GetToolTip();
            ParseEquipIDs();
        }

        protected override string GetS_ID()
        {
            var res = AttrMap.GetValueOrDefault(DamageType);
            return res;
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
                if (_.Type == "武器" && _.Associate == Tag)
                {
                    res.Add(_);
                }
            }

            SkillEvents = res.ToImmutableArray();
        }

        // 获取新的提示
        public void GetToolTip()
        {
            if (Value > 0)
            {
                var newstr = $"特效提升{Value}{DamageTypeDesc}攻击力";
                var res = $"{ToolTip}\n{newstr}";
                ToolTip = res;
            }
        }
    }

    public class YZOption : EquipOption
    {
        public const int TabID = 8;
        public readonly bool IsNormal; // 是否为普通腰坠

        public readonly CharAttrCollection SCharAttr;
        public readonly NamedAttrs AttrsDesc;

        public new static readonly ImmutableDictionary<DamageTypeEnum, string> AttrMap =
            new Dictionary<DamageTypeEnum, string>()
            {
                {DamageTypeEnum.Magic, "M_Base_OC"},
                {DamageTypeEnum.Physics, "P_Base_OC"}
            }.ToImmutableDictionary();

        public YZOption(EquipOptionItem item)
            : base(item)
        {
            SCharAttr = new CharAttrCollection(GetSAtDict(), null, true);
            AttrsDesc = SCharAttr.ToNamed(ItemName);
            IsNormal = (Value == 0);
            GetToolTip();
            ParseEquipIDs();
        }

        protected override string GetS_ID()
        {
            var res = AttrMap.GetValueOrDefault(DamageType);
            return res;
        }

        public void ParseEquipIDs()
        {
            ParseEquipIDs(TabID);
        }

        // 获取新的提示
        public void GetToolTip()
        {
            if (Value > 0)
            {
                var newstr = $"使用：提升自身{Value}{DamageTypeDesc}破防，持续15秒，冷却时间180秒。";
                var res = $"{ToolTip}\n{newstr}";
                ToolTip = res;
            }
        }
    }
}