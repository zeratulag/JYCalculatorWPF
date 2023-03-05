namespace JX3CalculatorShared.Class
{
    public class DPSTableItem
    {
        #region 成员
        public string Name { get; }
        public string DescName { get; } // 描述
        public double DPS { get; set; } // 时段DPS
        public double DPSContribute { get; protected set; } // 贡献
        public double Cover { get; set; } // 覆盖率
        public double Proportion { get; set; } // 占比

        #endregion

        public DPSTableItem(string name, string descName, double dps)
        {
            Name = name;
            DescName = descName;
            DPS = dps;
        }

        public void Proceed()
        {
            DPSContribute = DPS * Cover;
        }

    }
}