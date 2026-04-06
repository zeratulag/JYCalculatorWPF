using CommunityToolkit.Mvvm.ComponentModel;
using HtmlAgilityPack;
using JX3PZ.Data;
using JX3PZ.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MingYiTools
{
    public class MingYiEquipItemEntityViewModel: ObservableObject
    {
        public bool IsValidEquip { get; set; }
        public int Strength { get; set; }
        public string ToolTip { get;  set; }
        // 如果需要更详细的属性（如颜色、字体），可以定义一个结构来存储
        public List<RichTextSegment> DescRichTextSegments { get;  set; }
        public string PositionLabel { get; set; } // 部位描述
        public string Name { get; set; } // 物品名称
        public string NameColor { set; get; }
        public EquipSlotEnum EquipSlot { get; set; }
        public string DescText { get; set; }

        public string StrengthLabel { get; set; }
        public string NameLabel { get; set; }

        public void ParseDesc(string Desc)
        {
            if (string.IsNullOrWhiteSpace(Desc))
            {
                ToolTip = string.Empty;
                return;
            }

            // 1. 加载原始 HTML 片段
            var doc = new HtmlDocument();
            doc.LoadHtml(Desc);

            
            // 找到最后一个元素节点（用于特殊判断）
            var lastElementNode = doc.DocumentNode.ChildNodes
                .LastOrDefault(n => n.NodeType == HtmlNodeType.Element);

            DescRichTextSegments = new List<RichTextSegment>();

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                // 仅处理元素节点，忽略空白文本节点
                if (node.NodeType != HtmlNodeType.Element) continue;

                // 获取节点内部文本 (例如: text="内容" font=60)
                // 注意：因为数据格式是非标 HTML（属性在尖括号外），所以内容在 InnerText 中
                string rawContent = node.InnerText;
                if (string.IsNullOrWhiteSpace(rawContent)) continue;

                // 2.【关键技巧】构造虚拟节点进行二次解析
                // 将 "text='...'" 这样的文本包裹在一个虚拟标签中，让 HAP 帮我们解析属性
                var dummyDoc = new HtmlDocument();
                dummyDoc.LoadHtml($"<span {rawContent} />");

                var attrNode = dummyDoc.DocumentNode.FirstChild;
                if (attrNode == null) continue;

                // 3. 提取属性并处理转义
                if (node.Name.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    string textVal = attrNode.GetAttributeValue("text", "");

                    // 新增判定：如果当前 node 是最后一个元素且 textVal 精确为字面值 @"\n\n"，则忽略它
                    if (lastElementNode != null && node == lastElementNode && textVal == @"\n\n")
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(textVal))
                    {
                        // 替换：字面量反斜杠 + 实际换行 / 实际制表
                        textVal = textVal.Replace(@"\" + "\n", "\n")
                            .Replace(@"\" + "\t", "\t");

                        // 替换：字面量 "\n"（即反斜杠 + 字母 n）
                        textVal = textVal.Replace(@"\n", "\n");
                        int fontVal = attrNode.GetAttributeValue("font", 0);

                        // --- 颜色解析逻辑 ---
                        string color = null; // 默认为 null，表示使用默认颜色

                        // 只有当属性中显式包含 'r' 时才解析颜色
                        // 避免未指定颜色的文本被错误的解析为黑色 (#000000)
                        if (attrNode.Attributes.Contains("r"))
                        {
                            int r = attrNode.GetAttributeValue("r", 0);
                            int g = attrNode.GetAttributeValue("g", 0);
                            int b = attrNode.GetAttributeValue("b", 0);

                            // 格式化为 Hex 字符串 (#RRGGBB)
                            color = $"#{r:X2}{g:X2}{b:X2}";
                        }
                        // ------------------
                        var seg = new RichTextSegment(textVal, fontVal, color);
                        DescRichTextSegments.Add(seg);
                    }
                }
                // Image 标签的属性通常在 Attributes 中或同样在 InnerText 中，视具体数据而定
                // 根据您的 Debug 信息，Image 的属性似乎也在 InnerText 中，可同样处理
            }

            // 移除尾部仅包含换行（实际换行或字面 "\n" 的段）
            if (DescRichTextSegments != null && DescRichTextSegments.Count > 0)
            {
                // 连续移除最后的空行段，直到末尾段不是仅由换行构成
                while (DescRichTextSegments.Count > 0)
                {
                    var last = DescRichTextSegments.Last();
                    var txt = last.Text ?? string.Empty;

                    // 判断 1：仅包含实际换行字符（\r 或 \n）
                    bool onlyActualNewlines = txt.Length > 0 && txt.Trim('\r', '\n').Length == 0;

                    // 判断 2：仅包含字面反斜杠和 n（例如 @"\n"、@"\n\n"）
                    bool onlyEscapedNewlines = Regex.IsMatch(txt, @"^(?:\\n)+$");

                    if (onlyActualNewlines || onlyEscapedNewlines)
                    {
                        DescRichTextSegments.RemoveAt(DescRichTextSegments.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }


                if (DescRichTextSegments.Count > 0)
                {
                    var last = DescRichTextSegments.Last();
                    var txt = last.Text ?? string.Empty;
                    if (txt.Length > 0 && (txt.EndsWith("\n") || txt.EndsWith("\r")))
                    {
                        var trimmed = txt.TrimEnd('\r', '\n');
                        last.Text = trimmed;
                    }
                }
            }

            // 重新生成 ToolTip（如果需要反映已移除的段）
            var sb = new StringBuilder();
            if (DescRichTextSegments != null && DescRichTextSegments.Count > 0)
            {
                foreach (var seg in DescRichTextSegments)
                {
                    sb.Append(seg.Text);
                }
            }

            ToolTip = sb.ToString();
        }

        public void Parse()
        {
            PositionLabel = EquipMapLib.Equip[EquipSlot].Label;

            if (IsValidEquip && DescRichTextSegments != null && DescRichTextSegments.Count > 0)
            {
                // 提取名称和颜色
                Name = DescRichTextSegments[0].Text;
                NameColor = DescRichTextSegments[0].Color;
                NameLabel = $"[{Name}]";
                // 准备强度后缀
                StrengthLabel = $"({Strength})";
                // 保持 DescText 以备兼容，但 UI 上主要使用拆分属性
                DescText = $"{PositionLabel}: {NameLabel} {StrengthLabel}";
            }
            else
            {
                Name = "无";
                NameLabel = Name;
                NameColor = null; // 无装备时没有颜色
                StrengthLabel = string.Empty;
                DescText = $"{PositionLabel}: {NameLabel}";
            }
        }
    }
}