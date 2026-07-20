using System;
using System.Windows.Media;

namespace JX3CalculatorShared.Utils
{
    /// <summary>
    /// 差异数值显示工具
    /// </summary>
    public static class DeltaDisplayTool
    {
        public const string PositiveColor = "#229954";
        public const string NegativeColor = "#C0392B";
        public const string NeutralColor = "Gray";

        public const string PointStringFormat = "{0:+0;-0;0}";
        public const string PercentStringFormat = "{0:+0.00%;-0.00%;0.00%}";
        public const string ParenthesizedPercentStringFormat = "{0:(+0.00%);(-0.00%);(0.00%)}";

        public const double PointThreshold = 0.5;
        public const double PercentThreshold = 0.005 / 100;

        public static readonly Brush HasteChangedColor = Brushes.Coral;
        public const string HasteChanged = "加速改变";

        public static string GetPointColor(double delta)
        {
            return GetColor(delta, PointThreshold);
        }

        public static string GetPercentColor(double delta)
        {
            return GetColor(delta, PercentThreshold);
        }

        private static string GetColor(double delta, double threshold)
        {
            if (double.IsNaN(delta) || double.IsInfinity(delta) || Math.Abs(delta) <= threshold)
            {
                return NeutralColor;
            }

            return delta > 0 ? PositiveColor : NegativeColor;
        }
    }
}
