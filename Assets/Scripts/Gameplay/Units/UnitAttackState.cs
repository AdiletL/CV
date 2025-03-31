using Machine;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit
{
    public abstract class UnitAttackState : State, IAttack, IRangeAttack, IAttackSpeed
    {
        public override StateCategory Category { get; } = StateCategory.Attack;
        
        protected GameObject gameObject;
        protected Transform center;
        
        public DamageData DamageData { get; protected set; }
        
        public Stat DamageStat { get; } = new Stat();
        public Stat AttackSpeedStat { get;} = new Stat();
        public Stat RangeAttackStat { get; } = new Stat();
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;


        public override void Initialize()
        {
            base.Initialize();
            DamageData = new DamageData(gameObject, DamageType.Physical, DamageStat.CurrentValue);
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