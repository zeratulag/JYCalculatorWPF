using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
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

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class QualityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            QualityEnum quality = (QualityEnum)value;
            string color = quality.GetColor();
            var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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
            var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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
            var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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
}