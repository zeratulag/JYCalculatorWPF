using JX3PZ.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Span = System.Windows.Documents.Span;

namespace JX3CalculatorShared.Utils
{
    public static class FlowDocumentTool
    {
        public static void Clear(this FlowDocument doc)
        {
            doc.Blocks.Clear();
        }

        /// <summary>
        /// 将XAML字符串转换为FlowDocument中的对象
        /// </summary>
        /// <param name="text">XAML字符串</param>
        /// <returns></returns>
        public static object ConvertXamlText(string text)
        {
            var res = XamlReader.Parse(text);
            return res;
        }

        public static Run GetRun(string text, string color = ColorConst.Default)
        {
            var run = new Run(text) { Foreground = SolidColorBrushLib.FromColor(color, true) };
            return run;
        }

        public static Span GetSpan(string text, string color = ColorConst.Default, object tag = null)
        {
            var span = new Span();
            if (tag != null)
            {
                span.Tag = tag;
            }

            var run = GetRun(text, color);
            span.Inlines.Add(run);
            return span;
        }

        /// <summary>
        /// 转换为富文本的XAML字符串表示
        /// </summary>
        /// <param name="text">文字</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static string GetRunXamlText(string text, string color = ColorConst.Default)
        {
            const string RunTMP = "<Run Text=\"{0}\" Foreground=\"{1}\"/>";
            var res = String.Format(RunTMP, text, color);
            return res;
        }

        public static string GetRunXamlText(ColorText ctext) => GetRunXamlText(ctext.Text, ctext.Color);


        // 生成一个左边是图像，右边是文字的Span对象，用于五行石和附魔用
        public static Span GetImageSpan(string path, Run run, int width = 18, int height = 18)
        {
            var imgUri = new Uri(path, UriKind.Absolute);

            // 创建Image控件
            Image image = new Image
            {
                Source = new BitmapImage(imgUri),
                Width = width,
                Height = height,
                Margin = new Thickness(0, 0, 3, 0) // 右边距设为3
            };
            // 创建InlineUIContainer并添加Image
            InlineUIContainer inlineContainer = new InlineUIContainer(image);

            // 创建Span控件并设置BaselineAlignment属性
            Span span = new Span
            {
                BaselineAlignment = BaselineAlignment.Center
            };

            // 将InlineUIContainer和Run添加到Span中
            span.Inlines.Add(inlineContainer);
            span.Inlines.Add(run);
            return span;
        }

        // 给一个Span添加超链接
        public static Span GetUrlSpan(Inline inline, string url)
        {
            // 创建一个新的Span对象
            // 创建Hyperlink对象
            Hyperlink hyperlink = new Hyperlink(inline)
            {
                NavigateUri = new Uri(url)
            };

            // 注册点击事件
            hyperlink.RequestNavigate += (sender, e) =>
            {
                // 打开链接
                CommandTool.OpenUrl(e.Uri.AbsoluteUri);
                //Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
                //{
                //    UseShellExecute = true
                //});

                // 设置事件为已处理
                e.Handled = true;
            };

            var span = new Span();
            span.Inlines.Add(hyperlink);
            return hyperlink;
        }



        // 返回一个文档对象中的所有字符串
        public static string GetAllText(this TextElement element)
        {
            TextRange textRange = new TextRange(element.ContentStart, element.ContentEnd);
            return textRange.Text;
        }

        public static string GetAllText(this FlowDocument element)
        {
            TextRange textRange = new TextRange(element.ContentStart, element.ContentEnd);
            return textRange.Text;
        }


        // 向FlowDocumentc中添加一个Section，如果这个Section为空，则不添加
        public static void AddSection(this FlowDocument flowDocument, Section section)
        {
            if (section != null)
            {
                flowDocument.Blocks.Add(section);
            }
        }



        // 向Section中添加一个Paragraph，如果这个Paragraph为空，则不添加
        public static void AddParagraph(this Section section, Paragraph paragraph)
        {
            if (paragraph != null)
            {
                section.Blocks.Add(paragraph);
            }
        }

        // 新建一个0距离的Section
        public static Section NewSection(object tag = null)
        {
            var res = new Section() { Margin = new Thickness(0) };
            if (tag != null)
            {
                res.Tag = tag;
            }
            return res;
        }

        // 新建一个0距离的段落
        public static Paragraph NewParagraph(object tag = null)
        {
            var res = new Paragraph() { Margin = new Thickness(0) };
            if (tag != null)
            {
                res.Tag = tag;
            }
            return res;
        }

        // 如果字符串不为空，则加入
        public static void AddText(this Paragraph para, string text)
        {
            if (text.IsNotEmptyOrWhiteSpace())
            {
                para.Inlines.Add(text);
            }
        }

        // 向一个段落加入一些span，并且自动在两个元素之间换行
        public static void AddLines(this Paragraph para, IEnumerable<Span> spans)
        {
            var realSpans = spans.Where(_ => _ != null).ToArray();
            int n = realSpans.Length;
            int i = 0;
            foreach (var _ in realSpans)
            {
                para.Inlines.Add(_);
                if (i != n - 1)
                {
                    para.Inlines.Add(new LineBreak());
                }
                i++;
            }
        }

        // 向一个段落加入一些字符串（仅非空），并且自动在两个元素之间换行
        public static void AddLines(this Paragraph para, params string[] texts)
        {
            var spans = new List<Span>(5);
            foreach (var text in texts)
            {
                if (text.IsNotEmptyOrWhiteSpace())
                {
                    spans.Add(new Span(new Run(text)));
                }
            }
            AddLines(para, spans);
        }
    }

    public struct ColorText
    {
        public readonly string Text;
        public readonly string Color;

        public ColorText(string text, string color)
        {
            Text = text;
            Color = color;
        }
    }


    public static class TableTool
    {
        /// <summary>
        /// 将每一列的宽度都设为Auto
        /// </summary>
        /// <param name="table"></param>
        public static void SetColumnWidthAuto(this Table table)
        {
            foreach (var col in table.Columns)
            {
                col.Width = GridLength.Auto;
            }
        }

        /// <summary>
        /// 删除最后一行
        /// </summary>
        /// <param name="table"></param>
        public static void RemoveLastRow(this Table table)
        {
            var n = table.RowGroups.Count;
            table.RowGroups.RemoveAt(n - 1);
        }
    }
}