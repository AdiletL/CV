using System.Threading.Tasks;

namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        public override async Task FireAsync()
        {
            ApplyDamage();
            await Task.CompletedTask;
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}