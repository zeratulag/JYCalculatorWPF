using JX3CalculatorShared.Utils;
using JX3PZ.ViewModels;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace JX3PZ.Models
{
    public class EquipShowBoxModel
    {
        public Section Head = null;
        public Section Magic = null;
        public Section Stone = null;
        public Section Require = null;
        public Section Enhance = null;
        public Section Set = null;
        public Section Tail = null;

        public FlowDocument FDocument { get; private set; } = null;
        public string XamlText { get; private set; }

        public EquipShowBoxModel()
        {
            ClearDocument();
        }

        // 当没有选择装备时清空Model
        public void ClearDocument()
        {
            FDocument = new FlowDocument();
            SerializeFlowDocument();
        }

        public void UpdateFrom(EquipShowHeadViewModel head, EquipShowMagicViewModel magic, EquipShowStoneViewModel stone, EquipShowEnhanceViewModel enhance,
            EquipShowSetViewModel set, EquipShowTailViewModel tail)
        {
            Head = head?.GetSection();
            Magic = magic?.GetMagicSection();

            if (stone == null || !stone.HasStone)
            {
                Stone = null;
            }
            else
            {
                Stone = stone?.GetSection();
            }

            Require = magic?.GetRequireSection();
            Enhance = enhance?.GetSection();

            Set = set.HasSet ? set?.GetSection() : null;
            Tail = tail?.GetSection();
            GetFlowDocument();
            SerializeFlowDocument();
        }

        public FlowDocument GetFlowDocument()
        {
            FlowDocument doc = new FlowDocument() { PagePadding = new Thickness(0) };
            doc.AddSection(Head);
            doc.AddSection(Magic);
            doc.AddSection(Stone);
            doc.AddSection(Require);
            doc.AddSection(Enhance);
            doc.AddSection(Set);
            doc.AddSection(Tail);
            FDocument = doc;
            return doc;
        }

        public string SerializeFlowDocument()
        {
            XamlText = XamlWriter.Save(FDocument);
            return XamlText;
        }
    }
}