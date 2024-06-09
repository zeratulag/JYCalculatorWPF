using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static JX3CalculatorShared.Utils.BindingTool;

namespace JX3CalculatorShared.Views
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IconIDConverter : IValueConverter
    {
        // 品级到字体颜色转换
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string res = IconID2Path((int)value);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            string s = path.RemovePrefix(".png");
            string id = s.Split(";").Last();
            int i = Int32.Parse(id);
            return i;
        }
    }

    [ValueConversion(typeof(int), typeof(BitmapImage))]
    public class DiamondLevelToImgConverter : IValueConverter
    {
        // 五行石等级到图标
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = ImageLib.GetDiamond((int)value);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(BitmapImage))]
    public class IconIDToImgConverter : IValueConverter
    {
        // 图标ID到图标
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = ImageLib.GetIcon((int)value);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class PathToImgConverter : IValueConverter
    {
        // 图标ID到图标
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filename = (string)value;
            var res = ImageLib.GetImageFromPath(filename);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class FileNameToImgConverter : IValueConverter
    {
        // 图标ID到图标
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filename = "null";
            if (value is int)
            {
                filename = ((int)value).ToString();
            }
            else
            {
                filename = (string)value;
            }

            var res = ImageLib.GetImageFromFileName(filename);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class StringToColorConverter : IValueConverter
    {
        // 颜色字符串到颜色转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color = (string)value;
            var res = Utils.SolidColorBrushLib.FromColor(color);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class QualityToColorConverter : IValueConverter
    {
        // 品质到颜色转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            QualityEnum quality = (QualityEnum)value;
            var res = SolidColorBrushLib.FromQuality(quality);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class StoneLevelToColorConverter : IValueConverter
    {
        // 五行石等级到颜色转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int q = DiamondTabItem.GetQuality((int)value);
            QualityEnum quality = (QualityEnum)q;
            string color = quality.GetColor();
            //var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            var res = Utils.SolidColorBrushLib.FromColor(color);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class QualityToExternalBorderColorConverter : IValueConverter
    {
        // 品质到外边框颜色转换
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            QualityEnum quality = (QualityEnum)value;
            string color = quality.GetExternalBorderColor();
            //var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            var res = Utils.SolidColorBrushLib.FromColor(color);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class QualityToInternalBorderColorConverter : IValueConverter
    {
        // 品质到内边框颜色转换
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            QualityEnum quality = (QualityEnum)value;
            string color = quality.GetInternalBorderColor();
            //var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            var res = Utils.SolidColorBrushLib.FromColor(color);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 小数到百号文本的转换
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dvalue = (double)value;
            string res = $"{dvalue:P2}";
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            double res = double.Parse(text.Replace("%", "")) / 100;
            return res;
        }
    }

    /// <summary>
    /// 浮点数到整数的转换（非四舍五入）
    /// </summary>
    [ValueConversion(typeof(double), typeof(int))]
    public class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dvalue = (double)value;
            int res = (int)dvalue;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ivalue = (int)value;
            double res = ivalue;
            return res;
        }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bvalue = (bool)value;
            SolidColorBrush color;
            if (bvalue)
            {
                color = new SolidColorBrush(Colors.Black);
            }
            else
            {
                color = new SolidColorBrush(Colors.Red);
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityHiddenConverter : IValueConverter
    {
        // 保留位置的转换器
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean && (bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}