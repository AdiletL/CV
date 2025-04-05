using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit
{
    public abstract class UnitAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.Attack;

        protected SO_UnitAttack so_UnitAttack;
        protected GameObject gameObject;
        protected Transform center;
        protected DamageType damageTypeID;
        
        public DamageData DamageData { get; protected set; }
        
        public Stat DamageStat { get; } = new Stat();
        public Stat AttackSpeedStat { get;} = new Stat();
        public Stat RangeAttackStat { get; } = new Stat();
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetCenter(Transform center) => this.center = center;
        public void SetConfig(SO_UnitAttack config) => so_UnitAttack = config;

        protected virtual DamageData CreateDamageData()
        {
            return new DamageData(gameObject, so_UnitAttack.DamageTypeID, DamageStat.CurrentValue, true);
        }

        public override void Initialize()
        {
            base.Initialize();
            damageTypeID = so_UnitAttack.DamageTypeID;
            DamageData = CreateDamageData();
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
        public UnitAttackStateBuilder SetConfig(SO_UnitAttack config)
        {
            state.SetConfig(config);
            return this;
        }
    }
}