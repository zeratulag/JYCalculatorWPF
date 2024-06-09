namespace JX3PZ.Class
{
    public class Weapon : Equip
    {
        public WeaponBaseAttrs WeaponBase { get; private set; } // 最小武器伤害

        public override void Parse()
        {
            ParseAttrs();
            WeaponBase = new WeaponBaseAttrs(this);
            //GetDefaultShow();
        }
    }
}