namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        public override void ApplyDamage()
        {
            base.ApplyDamage();
            if (CurrentTarget.TryGetComponent(out IHealth health)
                &&  health.IsLive)
            {
                health.TakeDamage(Damageable);    
            }
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}