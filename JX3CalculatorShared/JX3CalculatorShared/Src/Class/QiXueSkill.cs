using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Src.Data;
using JX3CalculatorShared.Utils;

namespace JX3CalculatorShared.Class
{
    public class QiXueSkill: AbsIconToolTipItem
    {
        #region 成员

        public readonly string Key;
        public readonly int Position;
        public readonly int Order;
        public readonly int SkillID;
        public readonly int Level;
        public string ItemName { get; }
        public string ItemNameP { get; }
        public string ShortName { get; }
        public new string IconPath { get; }

        #endregion

        #region 构造

        public QiXueSkill(QiXueItem item)
        {
            Key = item.key;
            Position = item.position;
            Order = item.order;
            SkillID = item.SkillID;
            Level = item.Level;
            ItemName = item.ItemName;
            ItemNameP = StringTool.PadQiXueItemName(ItemName);
            ShortName = ItemName.Substring(0, 2);

            IconID = item.IconID;
            Name = item.Name;
            ToolTip = item.ToolTip + GetToolTipTail();

            IconPath = BindingTool.IconID2Path(IconID);
        }

        public string GetToolTipTail()
        {
            var id = Funcs.MergeIDLevel(SkillID, Level);
            var res = $"\n\nID: {id}";
            return res;
        }

        #endregion

    }
}