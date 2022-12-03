using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace JYCalculator.ViewModels
{
    public class AboutDialogViewModelBase : ObservableObject
    {
        
        public string MainName { get; set; } // 主标题
        public string BuildDateTime { get; set; } // 构建时间
        public string Description { get; set; } // 描述
        public string ThanksTo { get; set; } // 鸣谢
        public string GameDLC { get; set; } // 游戏资料片版本

        public ImmutableDictionary<string, string> URLDict;
        public string GitUrl { get; set; }
        public string JBUrl { get; set; }
        public string TMUrl { get; set; }


    }
}