using JX3CalculatorShared.Class;
using JX3CalculatorShared.Globals;
using JX3CalculatorShared.Utils;
using JX3PZ.Class;
using JX3PZ.Globals;
using JX3PZ.ViewModels;

namespace JX3PZ.Data
{
    public class Enhance
    {
        // 小附魔

        #region 自动生成

        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public int UIID { get; set; } = -1;
        public int Attribute1Value { get; set; } = -1;
        public string Attribute1ID { get; set; }
        public int Score { get; set; } = -1;
        public int DestItemSubType { get; set; } = -1;
        public int Quality { get; set; } = -1;
        public int IconID { get; set; } = -1;
        public string ItemName { get; set; }
        public int ExpansionPackLevel { get; set; }
        public int Useful { get; set; } = -1;
        public string Desc { get; set; } = "";
        public string ShortDesc { get; set; } = "";
        public string ItemDesc { get; set; } = "";

        #endregion

        public string ToolTip { get; private set; }

        public AttributeEntry Entry { get; private set; }
        public SimpleAttributeEntry SEntry { get; private set; }
        public EnhanceAttributeEntryViewModel VM { get; private set; }

        public void Parse()
        {
            ToolTip = ItemDesc + GetToolTipTail();
            VM = new EnhanceAttributeEntryViewModel(this);
            Entry = new AttributeEntry(this);
            SEntry = Entry.GetSimpleEntry();
        }

        public string GetToolTipTail()
        {
            var res = $"\n\nUIID: {UIID}\t附魔ID: {ID}\t[{ExpansionPackLevel}级]";
            return res;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }


        public bool Equals(Enhance other)
        {
            return other != null && other.ID == ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Enhance);
        }

        public bool IsHaste => Attribute1ID == AppStatic.HasteModifyType; // 是否为加速附魔

        public static (string Color, string Path) GetColorImage(Enhance e)
        {
            string color;
            string path;
            if (e == null)
            {
                color = ColorConst.Inactive;
                path = BindingTool.ImageName2Path("enhance-null");
            }
            else
            {
                color = ColorConst.Enhance;
                path = BindingTool.ImageName2Path("enhance");
            }

            return (color, path);
        }
    }
}