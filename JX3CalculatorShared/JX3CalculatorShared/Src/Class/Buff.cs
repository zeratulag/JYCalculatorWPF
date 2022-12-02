using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace JX3CalculatorShared.Src.Class
{
    public class Buff : BaseBuff
    {
        #region 成员

        public int DefaultStack;
        public int MaxStack;
        public double DefaultCover;

        public readonly BuffTypeEnum Type;
        public readonly int Order;

        public static readonly ImmutableDictionary<BuffTypeEnum, string> Type2Header =
            new Dictionary<BuffTypeEnum, string>()
            {
                {BuffTypeEnum.Buff_Self, "自身"},
                {BuffTypeEnum.Buff_Normal, "常见"},
                {BuffTypeEnum.DeBuff_Normal, "目标"},
                {BuffTypeEnum.Buff_Banquet, "宴席"},
                {BuffTypeEnum.Buff_Extra, "额外"},
                {BuffTypeEnum.Buff_ExtraStack, "特殊"},
            }.ToImmutableDictionary(); // 表示类型到描述的字典（自身，常见）

        #endregion

        #region 构造

        /// <summary>
        /// 用于描述实际buff的类
        /// </summary>
        /// <param name="name">唯一名称（编码）</param>
        /// <param name="descName">描述名</param>
        /// <param name="iconID">图标ID（可选）</param>
        /// <param name="isTarget">如果为True表示这是对目标生效的DeBuff</param>
        /// <param name="data">属性字典</param>
        /// <param name="maxStack">最大叠加层数</param>
        public Buff(string name, string descName, int iconID = -1, bool isTarget = false,
            IDictionary<string, double> data = null,
            int maxStack = 1) : base(name, descName, iconID, isTarget, data)
        {
            MaxStack = maxStack;
        }

        public Buff(string name, string descName, int iconID = -1, bool isTarget = false,
            int maxStack = 1,
            AttrCollection attrCollect = null) : base(name, descName, iconID: iconID, isTarget: isTarget,
            attrCollect: attrCollect)
        {
            MaxStack = maxStack;
        }

        public Buff(Buff_dfItem item) : this(item.Name, item.DescName, item.IconID, item.IsTarget, item.MaxStackNum,
            item.ParseItem())
        {
            BuffID = item.BuffID;
            DefaultStack = item.DefaultStackNum;
            DefaultCover = item.DefaultCover;

            ToolTipDesc = item.ToolTipDesc;
            Source = item.Source;

            Type = item.Type;

            Order = item.Order;
            AppendType = item.AppendType;
            Intensity = item.Intensity;
        }

        #endregion

        #region 显示

        #endregion

        #region 方法

        /// <summary>
        /// 同步Buff信息
        /// </summary>
        /// <param name="res">目标BaseBuff</param>
        protected void SyncInfo(BaseBuff res)
        {
            res.ToolTipDesc = ToolTipDesc;
            res.Source = Source;
            res.BuffID = BuffID;
            res.Order = Order;
            res.MakeToolTip();
        }

        /// <summary>
        /// 根据叠加层数和覆盖率生成BaseBuff
        /// </summary>
        /// <param name="cover">覆盖率</param>
        /// <param name="stack">叠加层数</param>
        /// <returns></returns>
        protected BaseBuff _Emit(double cover, double stack)
        {
            double nstack = Math.Min(stack, MaxStack);
            double fcover = Math.Min(cover, 1.0);

            string stackstr;
            if (nstack == (int) nstack)
            {
                stackstr = nstack > 1 ? $"[x{nstack:F0}]" : "";
            }
            else
            {
                stackstr = nstack > 1 ? $"[x{nstack:F1}]" : "";
            }

            string newDescName = $"{DescName}{stackstr}";
            string newName = $"{Name}{stackstr}";

            if (fcover < 1)
            {
                newDescName = newDescName + $"({fcover:P1})";
                newName = newName + $"({100 * fcover:F1}%)";
            }

            var newCollect = CharAttrs.Multiply(fcover * stack);

            var res = new BaseBuff(newName, newDescName, iconID: IconID, isTarget: IsTarget,
                appendType: AppendType, intensity: Intensity,
                attrCollect: newCollect);
            SyncInfo(res);
            return res;
        }


        /// <summary>
        /// 号令三军特殊处理
        /// </summary>
        /// <param name="fullcover">整体覆盖率（一鼓+二鼓），一共持续60秒</param>
        /// <param name="firststack">一鼓层数</param>
        /// <returns>生成的BaseBuff</returns>
        protected BaseBuff _EmitHaoLingSanJun(double fullcover, double firststack)
        {
            double nstack = Math.Min(firststack, MaxStack);
            
            double fcover = Math.Min(fullcover, 1.0);
            int second_stack = (int)(nstack / 2);

            double equivalent_stack = (nstack  + second_stack ) / 2.0; // 等效一鼓叠加层数

            double partcover = fcover / 2.0;

            string stackstr = $"[x{nstack:F0}|x{second_stack:D}]";

            string newDescName = $"{DescName}{stackstr}";
            string newName = $"{Name}{stackstr}";

            string coverDescName = $"({partcover:P1}|{partcover:P1})";
            string coverName = $"({100 * partcover:F1}|{100 * partcover:F1}%)";

            newDescName = newDescName + coverDescName;
            newName = newName + coverName;

            var newCollect = CharAttrs.Multiply(fcover * equivalent_stack);
            var res = new BaseBuff(newName, newDescName, iconID: IconID, isTarget: IsTarget,
                appendType: AppendType, intensity: Intensity,
                attrCollect: newCollect);
            SyncInfo(res);
            return res;
        }


        public BaseBuff Emit(double cover, double stack)
        {
            BaseBuff res = null;
            if (BuffID == "23107_1")
            {
                res = _EmitHaoLingSanJun(cover, stack);
            }
            else
            {
                res = _Emit(cover, stack);
            }

            return res;
        }

        /// <summary>
        /// 按照100%覆盖率生成
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public BaseBuff Emit(int stack)
        {
            return Emit(1, stack);
        }

        #endregion
    }
}