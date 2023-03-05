using System;

namespace JX3CalculatorShared.Class
{
    public class FightTimeSummaryBase
    {
        public static readonly double YZDuration = 15.0; // 腰坠持续时间
        public static readonly double RawYZCD = 180.0; // 腰坠初始CD
        public double TotalTime = 240.0;
        public double XWCD = 90;
        public double YZCD;
        public double XWDuration = 15.0;
        public FightTimeSummaryItem LongItem; // 长时间总结
        public FightTimeSummaryItem ShortItem; // 短时间总结
        public bool IsShort; // 是否为短时间战斗
        public FightTimeSummaryItem[] Data;


        public FightTimeSummaryBase()
        {
            Data = new FightTimeSummaryItem[] {
                new FightTimeSummaryItem("NULL0"),
                new FightTimeSummaryItem("NULL1")};
        }

        /// <summary>
        /// 用于描述战斗时间的类
        /// </summary>
        /// <param name="totalTime">总战斗时间（s）</param>
        /// <param name="xwCD">心无CD</param>
        /// <param name="xwDuration">心无持续时间</param>
        public FightTimeSummaryBase(double totalTime, double xwCD = 90.0, double xwDuration = 15.0) : this()
        {
            Update(totalTime, xwCD, xwDuration);
        }

        public void Update(double totalTime, double xwCD = 90.0, double xwDuration = 15.0, bool isShort = false)
        {
            TotalTime = totalTime;
            XWCD = xwCD;
            YZCD = Math.Ceiling(RawYZCD / xwCD) * xwCD;
            XWDuration = xwDuration;
            IsShort = isShort;
        }

        /// <summary>
        /// 仅仅改变心无CD
        /// </summary>
        /// <param name="xwCD"></param>
        public void UpdateXWCD(double xwCD)
        {
            XWCD = xwCD;
            YZCD = Math.Ceiling(RawYZCD / xwCD) * xwCD;

            var shorttime = GetShortTime();
            var longtime = GetLongTime();
            ShortItem = GetTimeSummary("短时间", shorttime);
            ShortItem.IsShort = true;
            LongItem = GetTimeSummary("长时间", longtime);

            Data[0] = ShortItem;
            Data[1] = LongItem;
        }


        /// <summary>
        /// 长时间战斗下，心无和腰坠的各自持续时间
        /// </summary>
        /// <returns>LongTotalDurationTime</returns>
        public (double XW, double YZ) GetLongTime()
        {
            double XW = TotalTime / XWCD * XWDuration;
            double YZ = TotalTime / YZCD * YZDuration;
            var totalDurationTime = (XW, YZ);
            return totalDurationTime;
        }

        /// <summary>
        /// 短时时间战斗下，心无和腰坠的各自持续时间
        /// </summary>
        /// <returns>ShortTotalDurationTime</returns>
        public (double XW, double YZ) GetShortTime()
        {
            double XW = CalcShortDurationTime(TotalTime, XWCD, XWDuration);
            double YZ = CalcShortDurationTime(TotalTime, YZCD, YZDuration);
            var totalDurationTime = (XW, YZ);
            return totalDurationTime;
        }

        /// <summary>
        /// 给定某个buff/skill 间隔时间interval，持续时间（秒），求一段时间内一共有多长时间覆盖。
        /// </summary>
        /// <param name="time">总时长（秒）</param>
        /// <param name="cd">触发间隔</param>
        /// <param name="duration">单次持续时间</param>
        /// <returns></returns>
        public static double CalcShortDurationTime(double time, double cd, double duration)
        {
            double res = Math.Min(time % cd, duration) + Math.Floor(time / cd) * duration;
            return res;
        }

        public FightTimeSummaryItem GetTimeSummary(string name, (double XW, double YZ) totalDurationTime)
        {
            double XW_Cover = totalDurationTime.XW / TotalTime;
            double YZ_Cover = totalDurationTime.YZ / TotalTime;
            double Normal_Cover = 1 - XW_Cover;
            double YZ_Over_XW_Cover = totalDurationTime.YZ / totalDurationTime.XW;


            var numXW = totalDurationTime.XW / XWDuration;
            var numYZ = totalDurationTime.YZ / YZDuration;
            double normalTime = TotalTime - totalDurationTime.XW;

            var summary = new FightTimeSummaryItem(name, numXW, numYZ, TotalTime,
                totalDurationTime.XW, totalDurationTime.YZ, normalTime);
            return summary;
        }
    }
}