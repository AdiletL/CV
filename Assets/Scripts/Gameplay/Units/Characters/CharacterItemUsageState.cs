using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Unit.Character;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public class CharacterItemUsageState : State
    {
        public override StateCategory Category { get; } = StateCategory.Action;
        
        protected SO_CharacterItemUsage so_CharacterItemUsage;
        protected CharacterAnimation characterAnimation;

        protected Item.Item currentItem;

        protected float durationAnimation, countDurationAnimation;

        protected Queue<float> momentEvents;
        
        public void SetConfig(SO_CharacterItemUsage so_CharacterItemUsage) => this.so_CharacterItemUsage = so_CharacterItemUsage;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        
        
        protected virtual AnimationEventConfig getAnimationEventConfig(ItemUsageType itemUsageType)
        { 
            return so_CharacterItemUsage.Animations[itemUsageType];
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (var VARIABLE in so_CharacterItemUsage.Animations.Values)
            {
                if(VARIABLE?.Clip) characterAnimation.AddClip(VARIABLE.Clip);   
            }
        }

        public override void Enter()
        {
            base.Enter();

            if (currentItem == null)
                stateMachine.ExitCategory(Category, null);
            
            if(momentEvents == null || momentEvents.Count == 0)
                currentItem.StartEffect();
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
                        currentItem.StartEffect();
                        momentEvents.Dequeue();
                    }
                }
            }
        }

        public override void Exit()
        {
            if(!IsActive) return;
            ExitCurrentItem();
            base.Exit();
        }

        protected virtual void ClearValues()
        {
            countDurationAnimation = 0;
            momentEvents?.Clear();
            
        }

        private void ExitCurrentItem()
        {
            characterAnimation.ExitAnimation();
            currentItem?.Exit();
            currentItem = null;
        }
        
        public void SetItem(Item.Item item)
        {
            ExitCurrentItem();
            ClearValues();
            currentItem = item;
            UpdateAnimation();
        }

        protected virtual void UpdateAnimation()
        {
            var animationEventConfig = getAnimationEventConfig(currentItem.ItemUsageTypeID);
            if (currentItem.TimerCast <= 0)
            {
                durationAnimation = animationEventConfig.Clip.length;
                momentEvents ??= new Queue<float>();
                foreach (var VARIABLE in animationEventConfig.MomentEvents)
                    momentEvents.Enqueue(durationAnimation * VARIABLE);
            }
            else
            {
                durationAnimation = currentItem.TimerCast;
                if(IsActive) currentItem.StartEffect();
            }
            
            characterAnimation.ChangeAnimationWithDuration(animationEventConfig.Clip, durationAnimation);
        }

        protected virtual void FinishDurationAnimation()
        {
            stateMachine.ExitCategory(Category, null);
        }
    }

    public class CharacterItemUsageStateBuilder : StateBuilder<CharacterItemUsageState>
    {
        public CharacterItemUsageStateBuilder(CharacterItemUsageState instance) : base(instance)
        {
        }

        public CharacterItemUsageStateBuilder SetConfig(SO_CharacterItemUsage so_CharacterItemUsage)
        {
            state.SetConfig(so_CharacterItemUsage);
            return this;
        }
        
        public CharacterItemUsageStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}