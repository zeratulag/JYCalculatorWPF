using JX3CalculatorShared.Globals;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace JX3CalculatorShared.Views
{
    public static class ViewGlobals
    {
        public static class PREFIX
        {
            public const string Grid = "Grid_";
            public const string GroupBox = "GroupBox_";
            public const string ItemsControl = "ItemsControl_";
            public const string TabControl = "TabControl_";
            public const string TabItem = "TabItem_";
            public const string Expander = "Expander_";
            public const string ListView = "ListView_";

            public const string FightOption = "FightOption_";
            public const string ItemDT = "ItemDT_";
            public const string QiXue = "QiXue";
            public const string MiJi = "MiJi_";
            public const string BigFM = "BigFM_";

            public const string ExpanderMiJi = Expander + MiJi;
            public const string ListViewMiJi = ListView + MiJi;
        }

        public static class SUFFIX
        {
            public const string CheckBox = "_chb";
            public const string ComboBox = "_cbb";
            public const string Label = "_lab";
            public const string SpinBox = "_spb";
        }

    }

    public static class QualityColor
    {
        public static ImmutableDictionary<QualityEnum, string> ColorMap = new Dictionary<QualityEnum, string>()
        {
            {QualityEnum.None, "#000000"},
            {QualityEnum.WHITE, "#ffffff"},
            {QualityEnum.GREEN, "#00d24b"},
            {QualityEnum.BLUE, "#007eff"},
            {QualityEnum.PURPLE, "#ff2dff"},
            {QualityEnum.ORANGE, "#ffa500"},
        }.ToImmutableDictionary();

        static QualityColor()
        {
        }

        // 品级到字体颜色转换
        public static string GetColor(this QualityEnum quality)
        {
            return ColorMap[quality];
        }

        // 品级到物品外边框颜色转换
        public static string GetExternalBorderColor(this QualityEnum quality)
        {
            string res = "#000000";
            if (quality == QualityEnum.None)
            {
                res = "#00000000";
            }

            return res;
        }

        // 品级到物品内边框颜色转换
        public static string GetInternalBorderColor(this QualityEnum quality)
        {
            string res = quality.GetColor();
            if (quality == QualityEnum.None)
            {
                res = "#00000000";
            }

            return res;
        }

        public static string GetExternalBorderColor(int quality)
        {
            var qualityenum = quality < 0 ? QualityEnum.None : (QualityEnum)quality;
            return GetExternalBorderColor(qualityenum);
        }

        public static string GetInternalBorderColor(int quality)
        {
            var qualityenum = quality < 0 ? QualityEnum.None : (QualityEnum)quality;
            return GetInternalBorderColor(qualityenum);
        }

    }
}