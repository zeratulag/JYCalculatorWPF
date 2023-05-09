using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3PZ.Models;

namespace JX3CalculatorShared.Messages
{
    public class StringMessage
    {
        public readonly string Value;
        public StringMessage(string value)
        {
            Value = value;
        }
    }

    public class ImportJBMessage
    {
        // 导入JB配装
        public readonly JBPZEquipSnapshotCollection Plan;
        public ImportJBMessage(JBPZEquipSnapshotCollection plan)
        {
            Plan = plan;
        }
    }

    public class PzPlanMessage
    {
        public readonly PzPlanModel Plan;
        public readonly string Title;
        public readonly string Author;
        public PzPlanMessage(string title, string author, PzPlanModel plan)
        {
            Title = title;
            Author = author;
            Plan = plan;
        }
    }

    public class CancelItemDTMessage
    {
        public readonly ItemDTTypeEnum Type;
        public readonly int UIID;

        public CancelItemDTMessage(ItemDTTypeEnum type, int uIID)
        {
            Type = type;
            UIID = uIID;
        }
    }
}