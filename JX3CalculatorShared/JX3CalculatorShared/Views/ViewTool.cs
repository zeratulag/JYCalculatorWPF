using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace JX3CalculatorShared.Views
{
    public static class ViewTool
    {
        /// <summary>
        /// 在一个对象中中按照指定的条件筛选子组件对象，返回组件名称:组件的字典
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="parent">父对象</param>
        /// <param name="prefix">组件名称前缀</param>
        /// <param name="suffix">组件名称后缀</param>
        /// <returns></returns>
        public static Dictionary<string, T> FindChildrenElements<T>(this Panel parent,
            string prefix = "", string suffix = "")
            where T : FrameworkElement
        {
            var res = new Dictionary<string, T>();
            foreach (var child in parent.Children)
            {
                if (child is T childT)
                {
                    var name = childT.Name;
                    if (name.StartsWith(prefix) && name.EndsWith(suffix))
                    {
                        res.Add(name, childT);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取一个TabControl的子页面
        /// </summary>
        /// <param name="tabControl"></param>
        /// <returns></returns>
        public static TabItem[] GeTabItems(this TabControl tabControl)
        {
            var res = tabControl.Items.OfType<TabItem>().ToArray();
            return res;
        }
    }


    public class Attached
    {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(Attached),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, FormattedTextPropertyChanged));

        public static void SetFormattedText(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(FormattedTextProperty, value);
        }

        public static string GetFormattedText(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(FormattedTextProperty);
        }

        private static void FormattedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            var formattedText = (string)e.NewValue ?? string.Empty;
            formattedText = string.Format("<Span xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{0}</Span>", formattedText);

            textBlock.Inlines.Clear();
            using (var xmlReader = XmlReader.Create(new StringReader(formattedText)))
            {
                var result = (Span)XamlReader.Load(xmlReader);
                textBlock.Inlines.Add(result);
            }
        }
    }

    /// <summary>
    /// 用于将控件名称转换解析的工具类
    /// </summary>
    public static class ViewNameTool
    {

        /// <summary>
        /// 从奇穴下拉框的名称中解析奇穴重数
        /// </summary>
        /// <param name="qiXueCbbName">奇穴下拉框名称</param>
        /// <returns>奇穴层数（1~12）</returns>
        public static int GetQiXuePosition(string qiXueCbbName)
        {
            var s = qiXueCbbName.RemovePrefix(ViewGlobals.PREFIX.QiXue).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            int i = int.Parse(s);
            return i;
        }

        /// <summary>
        /// 从奇穴重数还原奇穴下列列表对象名称
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetQiXueCbbElementName(int position)
        {
            var res = $"{ViewGlobals.PREFIX.QiXue}{position:D}{ViewGlobals.SUFFIX.ComboBox}";
            return res;
        }

        /// <summary>
        /// 从单体下拉框解析单体类型
        /// </summary>
        /// <param name="itemDTCbbName"></param>
        /// <returns></returns>
        public static string GetItemDTType(string itemDTCbbName)
        {
            var s = itemDTCbbName.RemovePrefix(ViewGlobals.PREFIX.ItemDT).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            return s;
        }


        public static string GetBigFMType(string bigFMCbbName)
        {
            var s = bigFMCbbName.RemovePrefix(ViewGlobals.PREFIX.BigFM).RemoveSuffix(ViewGlobals.SUFFIX.ComboBox);
            return s;
        }

        public static EquipSubTypeEnum GetBigFMTypeEnum(string bigFMCbbName)
        {
            var s = GetBigFMType(bigFMCbbName);
            Enum.TryParse(s, true, out EquipSubTypeEnum subType);
            return subType;
        }

        /// <summary>
        /// 从装备类型枚举量获取大附魔GUI元素的名称
        /// </summary>
        /// <param name="subType">装备类型枚举</param>
        /// <returns>大附魔元素的名称(CheckBoxName, ComboBoxName)</returns>
        public static (string CheckBoxName, string ComboBoxName) GetBigFMElementsName(EquipSubTypeEnum subType)
        {
            var s = subType.ToString();
            var prefix = ViewGlobals.PREFIX.BigFM + s;
            var chbname = prefix + ViewGlobals.SUFFIX.CheckBox;
            var cbbname = prefix + ViewGlobals.SUFFIX.ComboBox;
            return (chbname, cbbname);
        }


        /// <summary>
        /// 从SkillKey获取秘籍GUI元素的名称
        /// </summary>
        /// <param name="skillkey">技能key，例如JD</param>
        /// <returns>秘籍元素的名称(ExpanderName, ListViewName)</returns>
        public static (string ExpanderName, string ListViewName) GetMiJiElementsName(string skillkey)
        {
            var expanderName = ViewGlobals.PREFIX.ExpanderMiJi + skillkey;
            var listViewName = ViewGlobals.PREFIX.ListViewMiJi + skillkey;
            return (expanderName, listViewName);
        }
    }


    public static class RenderTool
    {
        public static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        public static BitmapFrame GetBitmapFrame(FrameworkElement visual)
        {
            var realSize = GetElementPixelSize(visual);
            var wpfsize = (Vector)visual.DesiredSize;
            var xcoef = realSize.Width / wpfsize.X;
            var ycoef = realSize.Height / wpfsize.Y;

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)realSize.Width, (int)realSize.Height, 96 * xcoef, 96 * ycoef, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            return frame;
        }

        public static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            BitmapFrame frame = GetBitmapFrame(visual);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        public static Size GetElementPixelSize(UIElement element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
                transformToDevice = source.CompositionTarget.TransformToDevice;
            else
                using (var source1 = new HwndSource(new HwndSourceParameters()))
                    transformToDevice = source1.CompositionTarget.TransformToDevice;

            if (element.DesiredSize == new Size())
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return (Size)transformToDevice.Transform((Vector)element.DesiredSize);
        }
        public enum LengthDirection
        {
            Vertical, // |
            Horizontal // ——
        }


    }

    public static class HandlerTool
    {
        public static void CommonOnDragOver(object sender, DragEventArgs e)
        {
            // 文件拖放特效
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
            e.Handled = true;
        }
    }
}