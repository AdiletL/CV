using Machine;
using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public abstract class CreepHealth : CharacterHealth
    {
        public override UnitType UnitType { get; } = UnitType.creep;
        
        protected CreepController CreepController;
        protected CharacterAnimation characterAnimation;
        protected SO_CreepHealth so_CreepHealth;
        protected AnimationClip takeDamageClip;
        
        public override void Initialize()
        {
            base.Initialize();
            
            CreepController = (CreepController)unitController;
            characterAnimation = CreepController.GetComponentInUnit<CharacterAnimation>();
            so_CreepHealth = (SO_CreepHealth)so_UnitHealth;
            takeDamageClip = so_CreepHealth.takeDamageClip;

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
            
            CreepController.StateMachine.ExitOtherStates(typeof(CreepTakeDamageState));
        }
    }
}