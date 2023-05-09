using System;
using System.Collections.Generic;
using JX3CalculatorShared.Data;

namespace JX3CalculatorShared.Class
{
    public class CharAttrCollection : AttrCollection
    {
        public static readonly Func<string, AttributeID> GetTemplate = AttributeIDLoader.GetAttributeID;

        public static CharAttrCollection Empty = new CharAttrCollection(false);
        public static CharAttrCollection SimplifiedEmpty = new CharAttrCollection(true);

        #region 构造

        public CharAttrCollection(bool simplified = false) : base(simplified: simplified)
        {
        }

        public CharAttrCollection(AttrCollection attr) : base(attr.Values, attr.Others)
        {
            Simplified = attr.Simplified;
        }

        public CharAttrCollection(IDictionary<string, double> data) : base(data, AttributeIDLoader.AttributeIsValue)
        {
        }

        public CharAttrCollection(Dictionary<string, double> value,
            Dictionary<string, List<object>> other,
            bool simplified = false) : base(value, other, simplified)
        {
        }

        #endregion

        #region 修改

        public new CharAttrCollection MultiplyValues(double k)
        {
            var attr = base.Multiply(k);
            return new CharAttrCollection(attr);
        }

        #endregion


        public CharAttrCollection Simplify()
        {
            var res = base.Simplify(GetTemplate);
            return new CharAttrCollection(res);
        }

        public static CharAttrCollection Sum(IEnumerable<CharAttrCollection> ats)
        {
            var res = AttrCollection.Sum(ats);
            return new CharAttrCollection(res);
        }
    }
}