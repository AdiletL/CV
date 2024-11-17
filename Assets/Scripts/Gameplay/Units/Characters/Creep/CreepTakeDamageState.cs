using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepTakeDamageState : CharacterTakeDamageState
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

    public class CreepTakeDamageStateBuilder : CharacterTakeDamageStateBuilder
    {
        public CreepTakeDamageStateBuilder(CreepTakeDamageState instance) : base(instance)
        {
        }

        public CreepTakeDamageStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if (state is CreepTakeDamageState enemyTakeDamageState)
            {
                enemyTakeDamageState.CharacterAnimation = characterAnimation;
            }

            return this;
        }

        public CreepTakeDamageStateBuilder SetClip(AnimationClip clip)
        {
            if (state is CreepTakeDamageState enemyTakeDamageState)
            {
                enemyTakeDamageState.TakeDamageClip = clip;
            }

            return this;
        }
    }
}