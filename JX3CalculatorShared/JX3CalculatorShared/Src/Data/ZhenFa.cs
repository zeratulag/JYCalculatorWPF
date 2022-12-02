using JX3CalculatorShared.Class;
using JX3CalculatorShared.Utils;
using NForza.Memoization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static JX3CalculatorShared.Globals.Strings;

namespace JX3CalculatorShared.Src.Class
{
    public class ZhenFa
    {
        #region 成员

        public string Name { get; }
        public string ItemName { get; } = "";
        public string DescName { get; } = "";
        public string Desc { get; } = "";
        public string KungFu { get; } = "";
        public int Z5_max_stack { get; } = 0; // 五阵最大叠加层数
        public int Z5_default_stack { get; } = 0; // 五阵默认叠加层数
        public double Z5_cover { get; } = 0; // 五阵默认覆盖率
        public int IconID { get; } = -1;

        public string BuffID { get; }
        public string BuffZ5ID { get; }

        public string IconPath { get; }

        public ImmutableDictionary<string, double> Buff_dict { get; set; } = null;
        public ImmutableDictionary<string, double> BuffZ5_dict { get; set; } = null;

        // 以上属性均为从JSON文件中读取，
        // 以下属性均为构造器生成

        public readonly BaseBuff Buff; // 阵法本体BUFF（全程覆盖）
        public readonly Buff BuffZ5; // 五阵BUFF（需要叠加，有层数）
        public string ToolTip { get; }

        public Func<double, int?, BaseBuffGroup> Emit; // 缓存版本的Emit

        public BaseBuffGroup EmitedBaseBuffGroup { get; private set; } // 输出的汇总
        public NamedAttrs AttrsDesc { get; private set; }

        #endregion

        public bool IsNone => Name == "None";

        public bool IsOwn => Name.EndsWith("#Self"); // 是否为自己开阵

        public bool HasZ5 => BuffZ5 != null; // 是否有五阵效果

        #region 构造

        public string GetToolTipTail()
        {
            var res = "";
            if (!string.IsNullOrEmpty(BuffID))
            {
                res = $"\n\nID: {BuffID}";
                var res2 = string.IsNullOrEmpty(BuffZ5ID) ? "" : $"\t五阵ID: {BuffZ5ID}";
                res += res2;
            }
            return res;
        }

        public string GetToolTip()
        {
            if (IsNone)
            {
                return DescName;
            }

            var sb = new StringBuilder();
            sb.Append($"{DescName} ~ {KungFu}");
            if (IsOwn)
            {
                sb.Append("\n（自己开阵）");
            }

            sb.Append(TooltipDivider);
            sb.Append(Desc);
            sb.Append(GetToolTipTail());

            return sb.ToString();
        }

        public (BaseBuff buff, Buff buffZ5) ParseBuff()
        {
            BaseBuff buff = null;
            Buff buffZ5 = null;

            if (Buff_dict != null)
            {
                buff = new BaseBuff(name: Name + "_Buff", descName: DescName, data: Buff_dict);
            }

            if (BuffZ5_dict != null)
            {
                buffZ5 = new Buff(name: Name + "_Z5Buff", descName: DescName + "_五阵", data: BuffZ5_dict,
                    maxStack: Z5_max_stack);
            }

            return (buff, buffZ5);
        }

        public ZhenFa(string name, string itemName, string descName,
            string desc, string kungFu,
            int z5_max_stack, int z5_default_stack, double z5_cover,
            int iconID,
            string buffID, string buffZ5ID,
            IDictionary<string, double> buff_dict,
            IDictionary<string, double> buffZ5_dict)

        {
            Name = name;
            ItemName = itemName;
            DescName = descName;
            Desc = desc;
            KungFu = kungFu;

            Z5_max_stack = z5_max_stack;
            Z5_default_stack = z5_default_stack;
            Z5_cover = z5_cover;

            IconID = iconID;
            BuffID = buffID;
            BuffZ5ID = buffZ5ID;

            Buff_dict = buff_dict?.ToImmutableDictionary();
            BuffZ5_dict = buffZ5_dict?.ToImmutableDictionary();

            (Buff, BuffZ5) = ParseBuff();
            ToolTip = GetToolTip();

            Emit = FuncExtensions.Memoize<double, int?, BaseBuffGroup>(Emit0);

            IconPath = BindingTool.IconID2Path(IconID);
            EmitDefault();
        }

        #endregion

        #region 方法


        /// <summary>
        /// 根据五阵覆盖率和叠加层数，生成汇总的BUFF
        /// </summary>
        /// <param name="cover"></param>
        /// <param name="z5Stack"></param>
        /// <returns></returns>
        public BaseBuffGroup Emit0(double cover, int? z5Stack = null)
        {
            BaseBuffGroup result = null;
            if (IsNone)
            {
                return result;
            }

            var buffList = new List<BaseBuff>() {Buff};

            if (HasZ5)
            {
                var realZ5Stack = Math.Min(z5Stack ?? Z5_default_stack, Z5_max_stack);
                var realBuffZ5 = BuffZ5.Emit(cover, realZ5Stack);
                buffList.Add(realBuffZ5);
            }

            result = new BaseBuffGroup(buffList);
            result.Calc();

            return result;
        }


        /// <summary>
        /// 产生默认的数值
        /// </summary>
        public void EmitDefault()
        {
            EmitedBaseBuffGroup = IsNone ? BaseBuffGroup.Empty : Emit(Z5_cover, Z5_default_stack);
            AttrsDesc = IsNone ? NamedAttrs.Empty : EmitedBaseBuffGroup.ToNamedAttrs();
        }

        #endregion
    }
}