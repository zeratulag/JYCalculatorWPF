using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;

namespace JX3CalculatorShared.Class
{
    public struct KAttributeEntry
    {
        /// <summary>
        /// 通用属性词条，包括数值类属性和非数值类属性，包括装备属性和非装备属性
        /// </summary>
        public string ModifyType { get; }

        public double Value { get; set; }

        public KAttributeEntry(string modifyType, double value)
        {
            ModifyType = modifyType;
            Value = value;
        }
    }

    public static class KAttributeEntryHelper
    {
        public static KAttributeID GetKAttribute(this KAttributeEntry entry) => AttributeIDLoader.GetAttributeID(entry.ModifyType);
    }
}