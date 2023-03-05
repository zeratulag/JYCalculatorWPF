using System;
using System.Collections.Generic;
using JX3CalculatorShared.Data;
using JX3CalculatorShared.Utils;

namespace JX3CalculatorShared.Class
{
    /// <summary>
    /// 描述技能属性的类
    /// </summary>
    public class SkillAttrCollection : AttrCollection
    {
        public static readonly Func<string, SkillAttrTemplate> GetTemplate = AtLoader.GetSkillAttrTemplate;

        public static SkillAttrCollection Empty = new SkillAttrCollection();
        public static SkillAttrCollection SimplifiedEmpty = new SkillAttrCollection();

        #region 构造

        public SkillAttrCollection(bool simplified = false) : base(simplified)
        {
        }

        public SkillAttrCollection(AttrCollection attr) : base(attr.Values, attr.Others)
        {
            Simplified = attr.Simplified;
        }

        public SkillAttrCollection(IDictionary<string, double> data) : base(data, AtLoader.SkillAt_is_Value)
        {
        }

        public SkillAttrCollection(SkillAttrCollection old) : base(old.Values.Copy(), old.Others.Copy())
        {
            Simplified = old.Simplified;
        }

        #endregion

        public SkillAttrCollection Simplify()
        {
            var res = base.Simplify(GetTemplate);
            return new SkillAttrCollection(res);
        }

        public static SkillAttrCollection Sum(IEnumerable<SkillAttrCollection> ats)
        {
            var res = AttrCollection.Sum(ats);
            return new SkillAttrCollection(res);
        }


        public new SkillAttrCollection Copy()
        {
            return new SkillAttrCollection(this);
        }
    }
}