using System;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public abstract class CreepHealth : CharacterHealth
    {
        public event Action OnTakeDamage; 
        
        protected CreepController CreepController;
        protected SO_CreepHealth so_CreepHealth;

        protected bool isCanTakeDamageEffect;
        
        public override void Initialize()
        {
            base.Initialize();
            
            CreepController = (CreepController)unitController;
            so_CreepHealth = (SO_CreepHealth)so_UnitHealth;
            isCanTakeDamageEffect = so_CreepHealth.IsCanTakeDamageEffect;
        }

        public override void TakeDamage(DamageData damageData)
        {
            base.TakeDamage(damageData);
            OnTakeDamage?.Invoke();
        }
    }
}