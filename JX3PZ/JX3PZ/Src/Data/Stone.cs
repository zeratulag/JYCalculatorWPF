using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using HandyControl.Expression.Shapes;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using JX3PZ.ViewModels;

namespace JX3PZ.Data
{
    public class Stone
    {
        #region 自动生成

        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public int UIID { get; set; } = -1;
        public string Attribute1ID { get; set; }
        public int DiamondType1 { get; set; } = -1;
        public int Compare1 { get; set; } = -1;
        public int DiamondCount1 { get; set; } = -1;
        public int DiamondIntensity1 { get; set; } = -1;
        public string Attribute2ID { get; set; }
        public int DiamondType2 { get; set; } = -1;
        public int Compare2 { get; set; } = -1;
        public int DiamondCount2 { get; set; } = -1;
        public int DiamondIntensity2 { get; set; } = -1;
        public string Attribute3ID { get; set; }
        public int DiamondType3 { get; set; } = -1;
        public int Compare3 { get; set; } = -1;
        public int DiamondCount3 { get; set; } = -1;
        public int DiamondIntensity3 { get; set; } = -1;
        public int TabType { get; set; } = -1;
        public int TabIndex { get; set; } = -1;
        public int Level { get; set; } = -1;
        public int IconID { get; set; } = -1;
        public int Attribute1Value { get; set; } = -1;
        public int Attribute2Value { get; set; } = -1;
        public int Attribute3Value { get; set; } = -1;
        public int Quality { get; set; } = -1;
        public string ToolTip { get; set; } = "";
        public string Tag { get; set; } = "";
        public string ShortDesc { get; set; } = "";
        public int Useful { get; set; } = -1;

        #endregion

        public ImmutableArray<int> AttributeValues { get; private set; }
        public ImmutableArray<AttributeEntry> AttributeEntries => Attributes.AttributeEntries;
        public bool IsValid { get; private set; } // 是否不为空
        public int ValidAttributesCount => Attributes.ValidAttributesCount; // 有效属性词条数, 2或3

        public StoneAttribute Attributes { get; private set; }


        public void Parse()
        {
            IsValid = Quality > 0;
            Attributes = new StoneAttribute(this);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }


        public bool Equals(Stone other)
        {
            return other != null && other.ID == ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Stone);
        }

        /// <summary>
        /// 该五彩石是否满足筛选器要求
        /// </summary>
        /// <param name="arg">筛选器</param>
        /// <returns></returns>
        public bool CanFilter(StoneFilterArg arg)
        {
            if (arg == null)
            {
                return true;
            }

            if (arg.AttrFilter.IsSubsetOf(Attributes.AttributeIDs))
            {
                return FitName(arg.Name);
            }

            return false;
        }

        /// <summary>
        /// 名称是否符合
        /// </summary>
        /// <param name="name">搜索的名称</param>
        /// <returns></returns>
        public bool FitName(string name)
        {
            if (name.IsEmptyOrWhiteSpace())
            {
                return true;
            }

            return Name.Contains(name) || Tag.Contains(name);
        }
        public bool IsHaste => Attributes.HasHaste; // 是否有加速
    }
}