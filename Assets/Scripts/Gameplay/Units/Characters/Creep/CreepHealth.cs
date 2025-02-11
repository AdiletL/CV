using System;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public abstract class CreepHealth : CharacterHealth
    {
        public event Action OnTakeDamage; 
        
        protected CreepController CreepController;
        protected CharacterAnimation characterAnimation;
        protected SO_CreepHealth so_CreepHealth;
        protected AnimationClip takeDamageClip;

        protected bool isCanTakeDamageEffect;
        
        public override void Initialize()
        {
            base.Initialize();
            
            CreepController = (CreepController)unitController;
            characterAnimation = CreepController.GetComponentInUnit<CharacterAnimation>();
            so_CreepHealth = (SO_CreepHealth)so_UnitHealth;
            takeDamageClip = so_CreepHealth.takeDamageClip;
            isCanTakeDamageEffect = so_CreepHealth.IsCanTakeDamageEffect;
            
            var takeDamageState = (CreepTakeDamageState)new CreepTakeDamageStateBuilder(new CreepTakeDamageState())
                .SetCharacterAnimation(characterAnimation)
                .SetClip(takeDamageClip)
                .SetStateMachine(CreepController.StateMachine)
                .Build();
            
            CreepController.StateMachine.AddStates(takeDamageState);
        }

        public override void TakeDamage(IDamageable damageable)
        {
            base.TakeDamage(damageable);
            
            OnTakeDamage?.Invoke();
            if(!isCanTakeDamageEffect) return;
            CreepController.StateMachine.ExitOtherStates(typeof(CreepTakeDamageState), true);
        }
    }
}