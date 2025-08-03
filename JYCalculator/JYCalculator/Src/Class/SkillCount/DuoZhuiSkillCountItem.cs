using JYCalculator.Data;
using JYCalculator.Models;
using JYCalculator.Src;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class DuoZhuiSkillCountItem : JYSkillCountItem
    {

        public bool IsBigXW;
        public Dictionary<string, double> ZMDict { get; private set; }

        public double _XinWuCast;

        public DuoZhuiSkillCountItem(AbilitySkillNumItem item, QiXueConfigModel qiXue) : base(item, qiXue)
        {
            _XinWuCast = (double) item.XW * qiXue.XWDuration / item.Time ;
        }

        public override void ResetTime(double newTime)
        {
            base.ResetTime(newTime);
            var k = newTime / _Time;
            _XinWuCast *= k;
        }

        public void DoPreWork()
        {
        }

        public override Dictionary<string, double> ToDict()
        {
            var res = base.ToDict();
            res.Add(nameof(_XinWuCast), _XinWuCast);
            return res;
        }
    }
}