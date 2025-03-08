using Machine;
using UnityEngine;
using Zenject;

namespace Unit
{
    public abstract class UnitAttackState : State, IAttack
    {
        [Inject] protected DiContainer diContainer;
        public override StateCategory Category { get; } = StateCategory.Attack;
        
        protected GameObject gameObject;
        protected Transform center;
        
        public IDamageable Damageable { get; protected set; }
        
        public Stat DamageStat { get; } = new Stat();
        public Stat AttackSpeedStat { get;} = new Stat();
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;

        protected abstract IDamageable CreateDamageable();


        public override void Initialize()
        {
            base.Initialize();
            Damageable = CreateDamageable();
            diContainer.Inject(Damageable);
        }

        public abstract void Attack();
        public abstract void ApplyDamage();
    }
    
    public abstract class UnitAttackStateBuilder : StateBuilder<UnitAttackState>
    {
        public UnitAttackStateBuilder(UnitAttackState instance) : base(instance)
        {
        }

        public UnitAttackStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }

        public UnitAttackStateBuilder SetCenter(Transform center)
        {
            state.SetCenter(center);
            return this;
        }
    }
}