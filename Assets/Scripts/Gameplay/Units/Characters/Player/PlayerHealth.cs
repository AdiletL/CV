using Machine;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerHealth : CharacterHealth, ICreepAttackable, ITrapAttackable
    {
        private PlayerController playerController;
        private CharacterAnimation characterAnimation;
        private SO_PlayerHealth so_PlayerHealth;
        private AnimationClip takeDamageClip;
        
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
            
            playerController.StateMachine.ExitCategory(StateCategory.Action, typeof(PlayerTakeDamageState));
        }
    }
}