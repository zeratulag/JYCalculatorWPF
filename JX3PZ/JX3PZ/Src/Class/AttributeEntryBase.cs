using CommunityToolkit.Mvvm.ComponentModel;
using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Globals;
using JX3PZ.Globals;

namespace JX3PZ.Class
{
    public class AttributeEntryBase : ObservableObject
    {
        /// <summary>
        /// 通用属性词条，包括数值类属性和非数值类属性
        /// </summary>
        public string ModifyType { get; }
        public AttributeID Attribute { get; }
        public AttributeEntryTypeEnum EntryType { get; protected set; }
        public string Desc { get; protected set; } = null;

        public AttributeEntryBase(string modifyType, AttributeEntryTypeEnum entryType = AttributeEntryTypeEnum.Default)
        {
            ModifyType = modifyType;
            Attribute = AttributeIDLoader.GetAttributeID(modifyType);
            EntryType = entryType;
        }

        public bool IsHaste => ModifyType == AppStatic.HasteModifyType; // 是否为加速
    }

}