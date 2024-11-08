using Machine;
using ScriptableObjects.Character.Enemy;
using UnityEngine;

namespace Character.Enemy
{
    public abstract class EnemyHealth : CharacterHealth
    {
        protected EnemyController enemyController;
        protected CharacterAnimation characterAnimation;
        protected SO_EnemyHealth so_EnemyHealth;
        protected AnimationClip takeDamageClip;
        
        public override void Initialize()
        {
            base.Initialize();
            enemyController = (EnemyController)characterMainController;
            characterAnimation = enemyController.components.GetComponentInGameObjects<CharacterAnimation>();
            so_EnemyHealth = (SO_EnemyHealth)so_CharacterHealth;
            takeDamageClip = so_EnemyHealth.takeDamageClip;

            var takeDamageState = (EnemyTakeDamageState)new EnemyTakeDamageStateBuilder(new EnemyTakeDamageState())
                .SetCharacterAnimation(characterAnimation)
                .SetClip(takeDamageClip)
                .SetStateMachine(enemyController.StateMachine)
                .Build();
            
            enemyController.StateMachine.AddStates(takeDamageState);
        }

        public override void TakeDamage(IDamageble damageble)
        {
            base.TakeDamage(damageble);
            enemyController.StateMachine.ExitCategory(StateCategory.action);
            enemyController.StateMachine.ExitOtherCategories(StateCategory.action);
            enemyController.StateMachine.SetStates(typeof(EnemyTakeDamageState));
        }
    }
}