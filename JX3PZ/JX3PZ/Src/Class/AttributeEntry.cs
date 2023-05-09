using JX3CalculatorShared.Class;
using JX3PZ.Data;
using JX3PZ.Globals;
using JX3PZ.ViewModels;
using Newtonsoft.Json.Linq;

namespace JX3PZ.Class
{
    public class AttributeEntry : AttributeEntryBase
    {
        // 表示属性词条
        public int Value { get; set; } = 0;
        public int Value2 { get; set; } // 仅当特殊属性才有，数值类属性为空

        public AttributeEntryViewModel VM; // 用于显示的VM

        public AttributeEntry(string modifyType, int value,
            AttributeEntryTypeEnum entryType = AttributeEntryTypeEnum.Default) :
            base(modifyType, entryType)
        {
            Value = value;
        }

        public AttributeEntry(AttributeTabItem item, AttributeEntryTypeEnum entryType = AttributeEntryTypeEnum.Default)
            : base(item.ModifyType, entryType)
        {
            if (Attribute.IsValue)
            {
                Value = item.Param1 == 0 ? item.Param2 : item.Param1;
            }
            else
            {
                Value = item.Param1;
                Value2 = item.Param2;
                Desc = item.SpecialDesc;
            }

            EntryType = entryType;
        }

        public AttributeEntry(DiamondLevelItem d) : this(d.ModifyType, d.Value, AttributeEntryTypeEnum.Diamond)
        {
            // 从五行石镶嵌构建
        }

        public AttributeEntry(Enhance e) : this(e.Attribute1ID, e.Attribute1Value, AttributeEntryTypeEnum.Enhance)
        {
            //  从附魔构建
        }

        public virtual string GetDesc()
        {
            // 当作为装备属性时其描述字符串
            if (Attribute.IsValue && Desc.IsEmptyOrWhiteSpace())
            {
                Desc = Attribute.GetDesc(Value, EntryType);
            }

            return Desc;
        }

        public AttributeEntryViewModel GetViewModel(string color = null)
        {
            if (Attribute.IsValue && Desc.IsEmptyOrWhiteSpace())
            {
                GetDesc();
            }

            string Color;
            if (color == null)
            {
                if (!Attribute.IsValue)
                {
                    Color = Attribute.GetSpecialColor();
                }
                else
                {
                    Color = EntryType.GetColor();
                }
            }
            else
            {
                Color = color;
            }

            VM = new AttributeEntryViewModel(Desc, Color);
            return VM;
        }

        public SimpleAttributeEntry GetSimpleEntry()
        {
            var res = new SimpleAttributeEntry(this);
            return res;
        }
    }
}