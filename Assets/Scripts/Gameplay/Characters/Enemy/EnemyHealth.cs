using ScriptableObjects.Character.Enemy;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyHealth : CharacterHealth
    {
        private EnemyController enemyController;
        private CharacterAnimation characterAnimation;
        private SO_EnemyHealth so_EnemyHealth;
        private AnimationClip takeDamageClip;
        
        public override void Initialize()
        {
            base.Initialize();
            enemyController = (EnemyController)characterMainController;
            characterAnimation = enemyController.components.GetComponentInGameObjects<CharacterAnimation>();
            so_EnemyHealth = (SO_EnemyHealth)so_CharacterHealth;
            takeDamageClip = so_EnemyHealth.takeDamageClip;
        }

        public override void TakeDamage(IDamageble damageble)
        {
            base.TakeDamage(damageble);
            characterAnimation.ChangeAnimation(takeDamageClip, transitionDuration: 0, play: true);
        }
    }
}