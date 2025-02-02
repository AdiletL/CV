using Cysharp.Threading.Tasks;

namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        
        public override async UniTask FireAsync()
        {
            if(Calculate.Move.IsFacingTargetUsingAngle(this.gameObject.transform.position, this.gameObject.transform.forward, CurrentTarget.transform.position, angleToTarget))
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