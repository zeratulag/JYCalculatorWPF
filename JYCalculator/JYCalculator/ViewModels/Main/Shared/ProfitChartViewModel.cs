using JX3CalculatorShared.Class;
using JX3CalculatorShared.ViewModels;
using JYCalculator.Class;
using JYCalculator.Data;
using System.Collections.Generic;
using System.Linq;

namespace JYCalculator.ViewModels
{
    public class ProfitChartViewModel : ComboBoxViewModel<AttrWeight>
    {
        public List<AttrProfitItem> Data { get; set; }
        public ProfitDF FinalProfitDF;

        public string Header { get; private set; }

        public ProfitChartViewModel() : base(StaticXFData.DB.AttrWeight.Arr)
        {
            SelectedIndex = 1;
        }

        public void UpdateDamageDeriv(DamageDeriv dv)
        {
            var res = dv.ProfitList.Data.ToList();
            res.Reverse();
            Data = res;
        }

        public void UpdateDamageDeriv()
        {
            UpdateDamageDeriv(FinalProfitDF.Items[SelectedIndex]);
        }

        public void UpdateSourceDF(ProfitDF df)
        {
            FinalProfitDF = df;
            _Update();
        }

        protected override void _Update()
        {
            if (FinalProfitDF == null) return;
            UpdateDamageDeriv();
            UpdateHeader();
        }

        protected void UpdateHeader()
        {
            Header = SelectedItem.Name + "属性收益图";
        }
    }
}