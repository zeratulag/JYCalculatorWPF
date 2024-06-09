using JX3CalculatorShared.Class;
using JX3CalculatorShared.Data;
using JX3PZ.Class;
using JX3PZ.Globals;

namespace JX3PZ.Data
{
    public class AttributeTabItem
    {
        /// <summary>
        /// Attribute.Tab的每个行
        /// </summary>

        #region 自动生成的

        public int ID { get; set; } = -1;

        public string ModifyType { get; set; }
        public int Param1 { get; set; } = 0;
        public int Param2 { get; set; } = 0;
        public string SpecialDesc { get; set; } = "";
        public int IsMobile { get; set; } = 0;

        #endregion

        public AttributeID Attribute { get; protected set; }

        public void Parse()
        {
            Attribute = AttributeIDLoader.GetAttributeID(ModifyType);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public bool Equals(AttributeTabItem other)
        {
            return other != null && ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AttributeTabItem);
        }


        /// <summary>
        /// 根据是否精炼，生成词条
        /// </summary>
        /// <param name="attributeEntryType"></param>
        /// <param name="strengthLevel"></param>
        /// <returns></returns>
        public AttributeEntry GetAttributeEntry(AttributeEntryTypeEnum attributeEntryType, int strengthLevel = 0)
        {
            if (strengthLevel == 0)
            {
                var res = new AttributeEntry(this, attributeEntryType);
                return res;
            }
            else
            {
                var res = new AttributeStrengthEntry(this, attributeEntryType, strengthLevel);
                return res;
            }
        }

        public AttributeStrengthEntry GetAttributeStrengthEntry(AttributeEntryTypeEnum attributeEntryType, int strengthLevel = 0)
        {
            var res = new AttributeStrengthEntry(this, attributeEntryType, strengthLevel);
            return res;
        }
    }
}