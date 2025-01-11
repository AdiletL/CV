using Cysharp.Threading.Tasks;

namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        public override async UniTask FireAsync()
        {
            ApplyDamage();
            await UniTask.CompletedTask;
        }
    }

    public class SwordBuilder : WeaponBuilder
    {
        public SwordBuilder() : base(new Sword())
        {
        }
    }
}