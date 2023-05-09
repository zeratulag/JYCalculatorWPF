using CommunityToolkit.Mvvm.ComponentModel;
using JX3PZ.Class;
using JX3PZ.Models;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using JX3CalculatorShared.Utils;

namespace JX3PZ.ViewModels
{
    public class EquipShowExtraViewModel: ObservableObject
    {
        /// <summary>
        /// 额外补充信息，包括装备来源和物品百科URL
        /// </summary>
        public bool HasEquip { get; set; } = false;
        public string GetInfo { get; set; } = "";// 获取来源（简单）
        public string GetInfoToolTip { get; set; } = ""; // 长版获取，详细
        public string URL { get; set; } = "";// 物品百科链接
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
                if (!e.IsGetInfoParsed) {e.ParseGetInfo();} // 按需解析
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


    }
}