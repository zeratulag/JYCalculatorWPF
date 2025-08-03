using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;

namespace JX3PZ.Data
{
    public class DiamondLevelItem
    {
        public int Level { get; }
        public int Quality { get; }
        public string LevelDesc { get; } = ""; // 一级五行石
        public string AttributeDesc { get; } = ""; // 力道提高xxx
        public string ShortAttributeDesc { get; } = ""; // 力道+XX
        public string ToolTip { get; } = "";
        public string AttributeEntryDesc { get; }
        public string Color { get; }

        public int Value => Item.LevelValues[Level];
        public string ModifyType => Item.Attribute.FullID;

        public readonly AttributeEntry Entry = null; // 当前的属性对象

        public readonly DiamondTabItem Item; // 父对象
        public string EquipTag { get; }
        public string ImagePath { get; }

        public SolidColorBrush CForeground { get;}

        public DiamondLevelItem(DiamondTabItem item, int level)
        {
            Item = item;
            Level = level;
            Quality = DiamondTabItem.GetQuality(level);
            ImagePath = BindingTool.Diamond2Path(Level);

            if (Level > 0)
            {
                Color = ColorConst.Green;
                LevelDesc = $"{StringConsts.ChinaNumber[Level]}级五行石";
                if (item.IsEmpty)
                {
                    AttributeDesc = "";
                    ShortAttributeDesc = "";
                    ToolTip = LevelDesc;
                }
                else
                {
                    AttributeDesc = Item.GetDesc(Level, prefix: false);
                    ShortAttributeDesc = Item.GetSimpleDesc(Level, prefix: false);
                    ToolTip = $"{LevelDesc}：{AttributeDesc}";
                    AttributeEntryDesc = $"熔嵌孔：{AttributeDesc}";
                }

                Entry = new AttributeEntry(this);
                EquipTag = item.Tag;
            }
            else
            {
                Color = ColorConst.Inactive;
                AttributeDesc = "未熔嵌";
                ShortAttributeDesc = AttributeDesc;
                ToolTip = "未熔嵌";
                AttributeEntryDesc = $"熔嵌孔：{Item.GetDesc(Level, prefix: false)}";
                EquipTag = "空";
            }

            CForeground = SolidColorBrushLib.FromColor(Color, true);
        }

        public static IEnumerable<DiamondLevelItem> GetLevelItems(DiamondTabItem item)
        {
            var res = from _ in Enumerable.Range(0, item.LevelValues.Length) select new DiamondLevelItem(item, _);
            return res;
        }


        #region 流文档元素

        public Span MakeSpan()
        {
            // 创建Run控件
            Run run = new Run(AttributeEntryDesc)
            {
                Foreground = CForeground,
            };
            var span = FlowDocumentTool.GetImageSpan(ImagePath, run);
            return span;
        }

        #endregion
    }
}