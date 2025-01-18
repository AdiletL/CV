using Cysharp.Threading.Tasks;

namespace Gameplay.Weapon
{
    public class Sword : Weapon
    {
        
        public override async UniTask FireAsync()
        {
            if(Calculate.Move.IsFacingTargetUsingAngle(this.GameObject.transform.position, this.GameObject.transform.forward, CurrentTarget.transform.position, AngleToTarget))
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