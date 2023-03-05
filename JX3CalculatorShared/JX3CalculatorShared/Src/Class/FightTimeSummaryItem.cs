namespace JX3CalculatorShared.Class
{
    public class FightTimeSummaryItem
    {
        /// <summary>
        /// 用于存储战斗时间分析的类
        /// </summary>

        #region 成员

        public string Name { get; } // 名称

        public double TotalTime; // 总时间
        public double XWNumber { get; set; } // 心无次数
        public double YZNumber { get; set; } // 腰坠次数

        public double XWTime { get; set; } // 心无持续时间
        public double YZTime { get; set; } // 腰坠持续时间
        public double NormalTime { get; set; } // 常规持续时间

        public bool IsShort = false; // 是否为短时间战斗

        public double XWCover => XWTime / TotalTime; // 心无覆盖率
        public double YZCover => YZTime / TotalTime; // 腰坠覆盖率
        public double NormalCover => NormalTime / TotalTime; // 常规覆盖率
        public double YZOverXWCover => YZTime / XWTime; // 心无期间腰坠覆盖率

        #endregion

        public FightTimeSummaryItem(string name)
        {
            Name = name;
        }

        public FightTimeSummaryItem(string name, double xwnumber, double yznumber,
            double totaltime, double xwtime, double yztime, double normaltime) : this(name)
        {
            XWNumber = xwnumber;
            YZNumber = yznumber;
            TotalTime = totaltime;
            XWTime = xwtime;
            YZTime = yztime;
            NormalTime = normaltime;
        }
    }
}