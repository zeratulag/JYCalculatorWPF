using JYCalculator.Src.Data;
using System;
using System.Collections.Generic;

namespace JYCalculator.Class
{
    public class AttrWeight
    {
        public string Name { get; }
        public double AP { get; set; }
        public double L { get; set; }
        public double OC { get; set; } = 1;
        public double CT { get; set; } = 1;
        public double CF { get; set; } = 1;
        public double WS { get; set; } = 1;
        public double PZ { get; set; } = 1;
        public double WP { get; set; } = 0;

        public double Final_AP { get; set; } = double.NaN;
        public double Final_L { get; set; } = double.NaN;
        public double Final_OC { get; set; } = double.NaN;

        private const string DiamondSuffix = "级孔";

        public string ToolTip { get; }

        public AttrWeight(DiamondValueItem item)
        {
            Name = $"{item.Level}{DiamondSuffix}";
            AP = item.AP;
            OC = item.OC;
            CF = item.CF;
            CT = item.CT;
            L = item.L;
            PZ = item.PZ;
            WS = item.WS;
            WP = Double.NaN;

            ToolTip = $"{item.Level}级五行石镶嵌孔增加的DPS";
        }

        public AttrWeight(string name, string toolTip = "")
        {
            Name = name;
            ToolTip = toolTip;
        }

        public bool IsDiamond => Name.EndsWith(DiamondSuffix);

        /// <summary>
        /// 返回字典形式
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, double> ToDict()
        {
            var res = new Dictionary<string, double>()
            {
                {nameof(AP), AP},
                {nameof(L), L},
                {nameof(OC), OC},
                {nameof(CT), CT},
                {nameof(CF), CF},
                {nameof(WS), WS},
                {nameof(PZ), PZ},
                {nameof(WP), WP},
                {nameof(Final_AP), Final_AP},
                {nameof(Final_L), Final_L},
                {nameof(Final_OC), Final_OC},
            };
            return res;
        }
    }
}