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
        protected SO_CreepHealth SoCreepHealth;
        protected AnimationClip takeDamageClip;
        
        public override void Initialize()
        {
            base.Initialize();
            
            CreepController = (CreepController)unitController;
            characterAnimation = CreepController.components.GetComponentFromArray<CharacterAnimation>();
            SoCreepHealth = (SO_CreepHealth)so_UnitHealth;
            takeDamageClip = SoCreepHealth.takeDamageClip;

            var takeDamageState = (CreepTakeDamageState)new CreepTakeDamageStateBuilder(new CreepTakeDamageState())
                .SetCharacterAnimation(characterAnimation)
                .SetClip(takeDamageClip)
                .SetStateMachine(CreepController.StateMachine)
                .Build();
            
            CreepController.StateMachine.AddStates(takeDamageState);
        }

        public override void TakeDamage(IDamageble damageble, GameObject gameObject)
        {
            base.TakeDamage(damageble, gameObject);
            
            CreepController.StateMachine.ExitCategory(StateCategory.action);
            CreepController.StateMachine.SetStates(typeof(CreepTakeDamageState));
            CreepController.StateMachine.ExitOtherCategories(StateCategory.action);
        }
    }
}