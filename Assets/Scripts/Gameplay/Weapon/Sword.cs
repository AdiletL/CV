namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        public override void Fire()
        {
            ApplyDamage();
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}