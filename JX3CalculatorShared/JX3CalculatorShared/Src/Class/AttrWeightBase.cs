using JX3CalculatorShared.Data;
using System;
using System.Collections.Generic;

namespace JX3CalculatorShared.Class
{
    public class AttrWeightBase
    {
        public string Name { get; }
        public double AP { get; set; }
        public double OC { get; set; } = 1;
        public double CT_Point { get; set; } = 1;
        public double CF_Point { get; set; } = 1;
        public double WS_Point { get; set; } = 1;
        public double PZ { get; set; } = 1;
        public double WP { get; set; } = 0;
        public double Final_AP { get; set; } = double.NaN;
        public double Final_OC { get; set; } = double.NaN;
        public string ToolTip { get; }

        private const string DiamondSuffix = "级孔";
        public bool IsDiamond = false;

        public AttrWeightBase(DiamondValueItemBase item)
        {
            Name = $"{item.Level}{DiamondSuffix}";
            AP = item.AP;
            OC = item.OC;
            CF_Point = item.CF;
            CT_Point = item.CT;

            PZ = item.PZ;
            WS_Point = item.WS;
            WP = Double.NaN;

            ToolTip = $"{item.Level}级五行石镶嵌孔增加的DPS";

            IsDiamond = Name.EndsWith(DiamondSuffix);
        }

        public AttrWeightBase(string name, string toolTip = "")
        {
            Name = name;
            ToolTip = toolTip;
            IsDiamond = false;
        }


        /// <summary>
        /// 返回字典形式
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(AP), AP},
                {nameof(OC), OC},
                {nameof(CT_Point), CT_Point},
                {nameof(CF_Point), CF_Point},
                {nameof(WS_Point), WS_Point},
                {nameof(PZ), PZ},
                {nameof(WP), WP},
                {nameof(Final_AP), Final_AP},
                {nameof(Final_OC), Final_OC},
            };
            // res.Add(nameof(L), L);
            return res;
        }
    }
}