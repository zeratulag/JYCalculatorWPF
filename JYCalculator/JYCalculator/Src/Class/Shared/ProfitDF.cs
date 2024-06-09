using JYCalculator.Data;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.Class
{
    public class ProfitDF
    {
        public readonly DamageDeriv PointDeriv;
        public readonly DamageDeriv ScoreDeriv;
        public DamageDeriv[] Items;

        public ProfitDF(DamageDeriv pointDeriv)
        {
            PointDeriv = pointDeriv;
            var list = new List<DamageDeriv>(10) { };

            var db = StaticXFData.DB.AttrWeight;
            list.AddRange(db.Arr.Select(aw => PointDeriv.GetAttrWeightedDeriv(aw)));

            Items = list.ToArray();

            foreach (var _ in Items)
            {
                _.GetPointAttrDerivList();
                if (_.Name != "单点属性")
                {
                    _.GetScoreAttrDerivList();

                    if (_.Name == "同分属性")
                    {
                        ScoreDeriv = _;
                    }
                    else
                    {
                        if (_.Weight.IsDiamond)
                        {
                            _.ProfitList.AsDiamondAttrProfitList();
                        }
                    }
                }
            }
        }
    }
}