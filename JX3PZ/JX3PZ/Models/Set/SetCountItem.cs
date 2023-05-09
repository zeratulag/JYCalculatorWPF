using JX3PZ.Class;

namespace JX3PZ.Models
{
    public readonly struct SetCountItem
    {
        public readonly int Position;
        public readonly string Name;
        public readonly string EID;
        public readonly int SetID;

        public SetCountItem(int position, string name, string eid, int setID)
        {
            Position = position;
            Name = name;
            EID = eid;
            SetID = setID;
        }

        public SetCountItem(int position, Equip e)
        {
            Position = position;
            if (e != null)
            {
                Name = e.Name;
                EID = e.EID;
                SetID = e.SetID;
            }
            else
            {
                Name = "";
                EID = "";
                SetID = -1;
            }
        }
    }
}