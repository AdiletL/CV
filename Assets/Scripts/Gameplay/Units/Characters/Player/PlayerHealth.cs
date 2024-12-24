using Machine;
using ScriptableObjects.Unit.Character.Creep;
using ScriptableObjects.Unit.Character.Player;
using Unit.Character;
using Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerHealth : CharacterHealth
    {
        public override UnitType UnitType { get; } = UnitType.player;
        
        protected PlayerController playerController;
        protected CharacterAnimation characterAnimation;
        protected SO_PlayerHealth so_PlayerHealth;
        protected AnimationClip takeDamageClip;
        
        public override void Initialize()
        {
            base.Initialize();
            
            playerController = (PlayerController)unitController;
            characterAnimation = playerController.GetComponentInUnit<CharacterAnimation>();
            so_PlayerHealth = (SO_PlayerHealth)so_UnitHealth;
            //takeDamageClip = so_PlayerHealth.takeDamageClip;

            var takeDamageState = (PlayerTakeDamageState)new PlayerTakeDamageStateBuilder()
                .SetStateMachine(playerController.StateMachine)
                .Build();
            
            playerController.StateMachine.AddStates(takeDamageState);
        }

        public override void TakeDamage(IDamageable damageable)
        {
            base.TakeDamage(damageable);
            
            playerController.StateMachine.ExitCategory(StateCategory.action);
            playerController.StateMachine.SetStates(typeof(PlayerTakeDamageState));
            //CreepController.StateMachine.ExitOtherCategories(StateCategory.action);
        }
    }
}