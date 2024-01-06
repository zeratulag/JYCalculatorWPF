using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Immutable;

namespace JX3CalculatorShared.ViewModels
{
    public class AboutDialogViewModelBase : ObservableObject
    {
        public string MainName { get; set; } // 主标题
        public string BuildDateTime { get; set; } // 构建时间
        public string Description { get; set; } // 描述
        public string ThanksTo { get; set; } // 鸣谢
        public string GameDLC { get; set; } // 游戏资料片版本
        public string LastPatchTime { get; set; } // 技改时间
        public string LastPatchURL { get; set; } // 技改链接

        public ImmutableDictionary<string, string> URLDict;
        public string GitUrl { get; set; }
        public string JBUrl { get; set; }
        public string TMUrl { get; set; }
    }

}