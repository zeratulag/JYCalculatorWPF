using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Views;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static JX3CalculatorShared.Globals.AppStatic;

namespace JX3CalculatorShared.Utils

{
    public static class ImageLib
    {
        // 图标库，防止内存泄露
        public static readonly Dictionary<int, BitmapImage> Icon; // 图标库
        public static readonly Dictionary<int, BitmapImage> Diamond; // 五行石图标库
        public static readonly Dictionary<string, BitmapImage> Path; // 图标路径图标库

        static ImageLib()
        {
            Icon = new Dictionary<int, BitmapImage>(400);
            Diamond = new Dictionary<int, BitmapImage>(10);
            Path = new Dictionary<string, BitmapImage>(10);
        }

        public static BitmapImage GetImageFromPath(string path) => Path.GetOrCreate(path, BindingTool.Path2Image);

        public static BitmapImage GetImageFromFileName(string name) =>
            GetImageFromPath(BindingTool.ImageName2Path(name));

        public static BitmapImage GetDiamond(int level) => Diamond.GetOrCreate(level, BindingTool.Diamond2Image);
        public static BitmapImage GetIcon(int iconID) => Icon.GetOrCreate(iconID, BindingTool.IconID2Image);
    }

    public static class BindingTool
    {
        public static string IconID2Path(int iconID, string extension = "png")
        {
            // 图标ID到路径
            string res = $"{RESOURCE_ICON_URI}{iconID}.{extension}";
            return res;
        }

        public static int IconPath2ID(string iconPath, string extension = "png")
        {
            string s = iconPath.RemoveSuffix($".{extension}");
            string id = s.Split("/").Last();
            int i = int.Parse(id);
            return i;
        }

        public static BitmapImage Path2Image(string path)
        {
            // 路径到资源
            var u = new Uri(path);
            var res = new BitmapImage(u);
            return res;
        }

        public static BitmapImage IconID2Image(int iconID)
        {
            // 图标ID到资源
            return Path2Image(IconID2Path(iconID));
        }

        public static string Diamond2Path(int level, string extension = "png")
        {
            string res = $"{RESOURCE_DIAMOND_URI}{level}.{extension}";
            return res;
        }

        public static string ImageName2Path(string name, string extension = "png")
        {
            // Images文件夹下的图片路径到完整路径

            string fileName = name;
            if (name.IsNullOrWhiteSpace() || name.IsEmptyOrWhiteSpace())
            {
                fileName = "null";
            }

            string res = $"{RESOURCE_IMAGE_URI}{fileName}.{extension}";
            return res;
        }

        public static BitmapImage Diamond2Image(int level)
        {
            // 五行石等级到资源
            return Path2Image(Diamond2Path(level));
        }

        public static SolidColorBrush Color2SolidColorBrush(string color)
        {
            var res = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            return res;
        }


        /// <summary>
        /// 品级到颜色的字符串标识
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static string Quality2Color(QualityEnum quality)
        {
            string color = quality.GetColor();
            return color;
        }

        public static string Quality2Color(int quality) => Quality2Color((QualityEnum)quality);
    }

    public static class SolidColorBrushLib
    {
        public static readonly Dictionary<string, SolidColorBrush> Data;

        static SolidColorBrushLib()
        {
            Data = new Dictionary<string, SolidColorBrush>(20);
        }

        public static SolidColorBrush FromColor(string color, bool clone = false)
        {
            SolidColorBrush res = null;
            res = clone ? BindingTool.Color2SolidColorBrush(color) : // 复制一份防止线程权限问题
                Data.GetOrCreate(color, BindingTool.Color2SolidColorBrush);
            return res;
        }

        /// <summary>
        /// 品级到颜色转换
        /// </summary>
        /// <param name="quality">品级</param>
        /// <param name="clone">是否返回拷贝（防止线程权限问题）</param>
        /// <returns></returns>
        public static SolidColorBrush FromQuality(QualityEnum quality, bool clone = false)
        {
            SolidColorBrush res = null;
            string color = BindingTool.Quality2Color(quality);
            res = FromColor(color, clone);
            return res;
        }

        public static SolidColorBrush FromQuality(int quality, bool clone = false) =>
            FromQuality((QualityEnum)quality, clone);
    }
}