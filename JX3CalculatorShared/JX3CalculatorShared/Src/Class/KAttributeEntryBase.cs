using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Data;

namespace JX3CalculatorShared.Class
{
    public class KAttributeEntryBase : ObservableObject
    {
        /// <summary>
        /// 通用属性词条，包括数值类属性和非数值类属性，包括装备属性和非装备属性
        /// </summary>
        public string ModifyType { get; }
        public KAttributeID KAttribute { get; }

        public KAttributeEntryBase(string modifyType)
        {
            ModifyType = modifyType;
            KAttribute = AttributeIDLoader.GetAttributeID(modifyType);
        }
    }
}