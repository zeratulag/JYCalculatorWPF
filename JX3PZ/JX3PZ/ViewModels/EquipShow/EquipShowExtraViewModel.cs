using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace JX3PZ.ViewModels
{
    public class EquipShowExtraViewModel : ObservableObject
    {
        /// <summary>
        /// 额外补充信息，包括装备来源和物品百科URL
        /// </summary>
        public bool HasEquip { get; set; } = false;

        public string GetInfo { get; set; } = ""; // 获取来源（简单）
        public string GetInfoToolTip { get; set; } = ""; // 长版获取，详细
        public string URL { get; set; } = ""; // 物品百科链接
        public bool ShowURL { get; set; } = true;

        public RelayCommand OpenWikiCmd { get; }

        public EquipShowExtraViewModel()
        {
            OpenWikiCmd = new RelayCommand(OpenWiki);
        }

        public EquipShowExtraViewModel(EquipShowExtraViewModel old)
        {
            HasEquip = old.HasEquip;
            GetInfo = old.GetInfo;
            GetInfoToolTip = old.GetInfoToolTip;
            URL = old.URL;
            ShowURL = old.ShowURL;
        }

        public void OpenWiki()
        {
            CommandTool.OpenUrl(URL);
        }

        public void UpdateFrom(Equip e)
        {
            if (e == null)
            {
                HasEquip = false;
                GetInfo = "";
                GetInfoToolTip = "";
                URL = "";
            }
            else
            {
                HasEquip = true;
                if (!e.IsGetInfoParsed)
                {
                    e.ParseGetInfo();
                } // 按需解析

                GetInfo = e.GetInfo.Desc;
                GetInfoToolTip = e.GetInfo.ToolTip;
                URL = GetURL(e.EID);
            }
        }

        public void UpdateFrom(EquipSnapShotModel model)
        {
            UpdateFrom(model.CEquip);
        }

        public static string GetURL(string EID)
        {
            var res = $"https://www.jx3box.com/item/view/{EID}";
            return res;
        }


        #region 流文档元素

        public Section GetSection()
        {
            var section = FlowDocumentTool.NewSection("Extra");
            var p1 = GetGetInfoParagraph();
            section.AddParagraph(p1);
            var p2 = GetWikiParagraph();
            section.AddParagraph(p2);
            return section;
        }

        public Paragraph GetGetInfoParagraph()
        {
            var para = FlowDocumentTool.NewParagraph(tag: "GetInfo");
            TextBox txb = new TextBox()
            {
                Text = GetInfo,
                ToolTip = GetInfoToolTip,
                Foreground = Brushes.White,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };
            // 创建一个TextBlock来显示文本，并添加ToolTip
            // 将TextBlock包装在InlineUIContainer中
            InlineUIContainer container = new InlineUIContainer(txb);
            // 将InlineUIContainer添加到Paragraph
            para.Inlines.Add(container);
            return para;
        }

        public Paragraph GetWikiParagraph()
        {
            if (!ShowURL) return null;
            var para = FlowDocumentTool.NewParagraph(tag: "Wiki");
            if (URL.IsNotEmptyOrWhiteSpace())
            {
                var span = FlowDocumentTool.GetSpan("\ud83d\udd17查看装备百科词条", "#4fefff");
                var urlSpan = FlowDocumentTool.GetUrlSpan(span, URL);
                para.Inlines.Add(urlSpan);
            }

            return para;
        }

        #endregion
    }
}