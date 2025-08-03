using JX3CalculatorShared.Class;
using JYCalculator.Data;

namespace JYCalculator.Src.Class
{
    public struct BuffRecord
    {
        public int ID { get; }
        public int Level { get; }
        public double Cover { get; }
        public double Stack { get; }

        public string BuffID { get; }

        public BuffRecord(int id, int level, double cover, double stack)
        {
            ID = id;
            Level = level;
            Stack = stack;
            Cover = cover;
            BuffID = $"{id}_{level}";
        }

        public BaseBuff ToBaseBuff()
        {
            return StaticXFData.DB.Buff.GetBaseBuffByRecord(this);
        }
    }
}