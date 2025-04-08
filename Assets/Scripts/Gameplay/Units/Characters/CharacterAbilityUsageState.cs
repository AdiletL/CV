using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterAbilityUsageState : State
    {
        public override StateCategory Category { get; } = StateCategory.Action;
        
        protected SO_CharacterAbilityUsage so_CharacterAbilityUsage;
        protected CharacterAnimation characterAnimation;
        
        protected Ability.Ability currentAbility;
        
        protected float durationAnimation, countDurationAnimation;
        protected bool isCanUsage = true;

        protected Queue<float> momentEvents;
        
        public void SetConfig(SO_CharacterAbilityUsage so_CharacterAbilityUsage) => this.so_CharacterAbilityUsage = so_CharacterAbilityUsage;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        
         protected virtual AnimationEventConfig getAnimationEventConfig(AbilityBehaviour abilityBehaviour)
        { 
            return so_CharacterAbilityUsage.Animations[abilityBehaviour];
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (var VARIABLE in so_CharacterAbilityUsage.Animations.Values)
            {
                if(VARIABLE?.Clip) characterAnimation.AddClip(VARIABLE.Clip);   
            }
        }

        public override void Enter()
        {
            base.Enter();

            if (currentAbility == null || !isCanUsage || currentAbility.IsCooldown)
            {
                stateMachine.ExitCategory(Category, null);
                return;
            }

            if (momentEvents == null || momentEvents.Count == 0 || durationAnimation == 0)
                currentAbility.StartEffect();
        }

        public override void Update()
        {
            countDurationAnimation += Time.deltaTime;
            if (durationAnimation <= countDurationAnimation)
            {
                FinishDurationAnimation();
            }
            else
            {
                if (momentEvents?.Count > 0)
                {
                    if (momentEvents.Peek() <= countDurationAnimation)
                    {
                        currentAbility.StartEffect();
                        momentEvents.Dequeue();
                    }
                }
            }
        }

        public override void Exit()
        {
            if(!IsActive) return;
            ExitCurrentAbility();
            base.Exit();
        }

        protected virtual void ClearValues()
        {
            countDurationAnimation = 0;
            momentEvents?.Clear();
        }

        private void ExitCurrentAbility()
        {
            characterAnimation.ExitAnimation();
            currentAbility?.Exit();
            currentAbility = null;
        }
        
        public void SetAbility(Ability.Ability ability)
        {
            if(!isCanUsage) return;
            if(currentAbility != ability)
                ExitCurrentAbility();
            ClearValues();
            currentAbility = ability;
            UpdateAnimation();
        }

        protected virtual void UpdateAnimation()
        {
            var animationEventConfig = getAnimationEventConfig(currentAbility.AbilityBehaviourID);
            if (currentAbility.TimerCast <= 0)
            {
                if (animationEventConfig.Clip)
                    durationAnimation = animationEventConfig.Clip.length;
                else
                    durationAnimation = 0;
                momentEvents ??= new Queue<float>();
                foreach (var VARIABLE in animationEventConfig.MomentEvents)
                    momentEvents.Enqueue(durationAnimation * VARIABLE);
            }
            else
            {
                durationAnimation = currentAbility.TimerCast;
                if(IsActive) currentAbility.StartEffect();
            }
            
            if(animationEventConfig.Clip) 
                characterAnimation.ChangeAnimationWithDuration(animationEventConfig.Clip, durationAnimation);
        }

        protected virtual void FinishDurationAnimation()
        {
            stateMachine.ExitCategory(Category, null);
        }
        
        public void ActivateUsage()
        {
            isCanUsage = true;
        }

        public void DeactivateUsage()
        {
            isCanUsage = false;
            stateMachine.ExitCategory(Category, null);
        }
    }
    
    public class CharacterAbilityUsageStateBuilder : StateBuilder<CharacterAbilityUsageState>
    {
        public CharacterAbilityUsageStateBuilder(CharacterAbilityUsageState instance) : base(instance)
        {
        }

        public CharacterAbilityUsageStateBuilder SetConfig(SO_CharacterAbilityUsage so_CharacterAbilityUsage)
        {
            state.SetConfig(so_CharacterAbilityUsage);
            return this;
        }
        
        public CharacterAbilityUsageStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}