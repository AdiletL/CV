using UnityEngine;

namespace Character.Enemy
{
    public class EnemyTakeDamageState : CharacterTakeDamageState
    {
        public CharacterAnimation CharacterAnimation { get; set; }
        public AnimationClip TakeDamageClip { get; set; }

        private float durationAnimation, countTimeAnimation;

        public override void Enter()
        {
            base.Enter();
            durationAnimation = TakeDamageClip.length;
            countTimeAnimation = 0;
            CharacterAnimation?.ChangeAnimation(TakeDamageClip, transitionDuration: 0, force: true);
        }

        public override void Update()
        {
            base.Update();
            
            countTimeAnimation += Time.deltaTime;
            if (countTimeAnimation > durationAnimation)
            {
                this.StateMachine.ExitCategory(Category);
                countTimeAnimation = 0;
            }
        }
    }

    public class EnemyTakeDamageStateBuilder : CharacterTakeDamageStateBuilder
    {
        public EnemyTakeDamageStateBuilder(EnemyTakeDamageState instance) : base(instance)
        {
        }

        public EnemyTakeDamageStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is EnemyTakeDamageState enemyTakeDamageState)
            {
                enemyTakeDamageState.CharacterAnimation = characterAnimation;
            }

            return this;
        }

        public EnemyTakeDamageStateBuilder SetClip(AnimationClip clip)
        {
            if (state is EnemyTakeDamageState enemyTakeDamageState)
            {
                enemyTakeDamageState.TakeDamageClip = clip;
            }

            return this;
        }
    }
}