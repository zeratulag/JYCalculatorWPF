using JX3CalculatorShared.Class;

namespace JX3PZ.Models
{
    public class PzMainSav
    {
        // 配装存档
        public JBPZEquipSnapshotCollection EquipList;
        public string Title;
        public string Author;
        public bool IsSync;

        public PzMainSav()
        {

        }

        public JBBB GetJBBB()
        {
            var res = new JBBB();
            res.Title = Title;
            res.EquipList = EquipList;
            return res;
        }

    }
}