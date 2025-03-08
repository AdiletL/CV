using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepTakeDamageState : CharacterTakeDamageState
    {
        protected CharacterAnimation characterAnimation;
        protected AnimationClip takeDamageClip;

        protected float durationAnimation, countTimeAnimation;


        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetAnimationClip(AnimationClip animationClip) => this.takeDamageClip = animationClip;

        public override void Enter()
        {
            base.Enter();
            durationAnimation = takeDamageClip.length;
            countTimeAnimation = 0;
            characterAnimation?.ChangeAnimationWithDuration(takeDamageClip, duration: this.durationAnimation, transitionDuration: 0, isForce: true);
        }

        public override void Update()
        {
            base.Update();
            CountTimeAnimation();
        }

        protected virtual void CountTimeAnimation()
        {
            countTimeAnimation += Time.deltaTime;
            if (countTimeAnimation > durationAnimation)
            {
                this.stateMachine.ExitCategory(Category, null);
                countTimeAnimation = 0;
            }
        }
    }

    public class CreepTakeDamageStateBuilder : CharacterTakeDamageStateBuilder
    {
        public CreepTakeDamageStateBuilder(CreepTakeDamageState instance) : base(instance)
        {
        }
        
        public CreepTakeDamageStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CreepTakeDamageState enemyTakeDamageState)
                enemyTakeDamageState.SetCharacterAnimation(characterAnimation);

            return this;
        }

        public CreepTakeDamageStateBuilder SetClip(AnimationClip clip)
        {
            if (state is CreepTakeDamageState enemyTakeDamageState)
                enemyTakeDamageState.SetAnimationClip(clip);

            return this;
        }
    }
}