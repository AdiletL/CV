using System;
using System.Collections.Generic;
using Machine;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Character
{
    [Flags]
    public enum DisableCategory
    {
        Nothing = 0,
        Everything = ~0,
        Movement = 1 << 0,
        Rotation = 1 << 1,
        Control = 1 << 2,
        ItemUsage = 1 << 3,
        AbilityUsage = 1 << 4,
    }
    public abstract class CharacterDisableState : State, IDisablelable
    {
        [Inject] protected SO_GameDisable so_GameDisable;
        
        public override StateCategory Category { get; } = StateCategory.Disable;
        
        protected GameObject gameObject;
        protected SO_CharacterDisable so_CharacterDisable;
        protected CharacterAnimation characterAnimation;
        protected const int ANIMATION_LAYER = 4;
        
        private Dictionary<DisableCategory, int> blockedActions;
        public DisableType CurrentDisableType { get; protected set; }
        
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetConfig(SO_CharacterDisable so_CharacterDisable) => this.so_CharacterDisable = so_CharacterDisable;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        
        
        protected AnimationClip getClip(DisableType disableType)
        {
            return so_CharacterDisable.Animations[disableType];
        }
        
        
        public bool IsActionBlocked(DisableCategory input)
        {
            if (blockedActions == null) return false;
            foreach (DisableCategory flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == DisableCategory.Nothing || (input & flag) == 0 || 
                    flag == DisableCategory.Everything) continue;

                if (blockedActions.ContainsKey(flag) && blockedActions[flag] > 0)
                    return true;
            }
            return false;
        }
        
        public void BlockAction(DisableCategory input)
        {
            blockedActions ??= new();
            foreach (DisableCategory flag in Enum.GetValues(typeof(DisableCategory)))
            {
                if (flag == DisableCategory.Nothing || (input & flag) == 0 || 
                    flag == DisableCategory.Everything) continue;

                blockedActions.TryAdd(flag, 0);
                blockedActions[flag]++;
            }
        }
        
        public void UnblockAction(DisableCategory input)
        {
            if(blockedActions == null) return;
            foreach (DisableCategory flag in Enum.GetValues(typeof(DisableCategory)))
            {
                if (flag == DisableCategory.Nothing || (input & flag) == 0 || 
                    flag == DisableCategory.Everything) continue;

                if (blockedActions.ContainsKey(flag))
                {
                    blockedActions[flag]--;

                    if (blockedActions[flag] <= 0) 
                        blockedActions.Remove(flag);
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            foreach (var VARIABLE in so_CharacterDisable.Animations.Values)
            {
                if(VARIABLE) characterAnimation.AddClip(VARIABLE);
            }
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            ActivateDisable(CurrentDisableType);
            characterAnimation.ChangeAnimationWithDuration(getClip(CurrentDisableType), isForce: true, layer: ANIMATION_LAYER);
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            base.Exit();
        }

        public void SetDisableType(DisableType disableType)
        {
            CurrentDisableType = disableType;
            if (!IsActive) return;
            ActivateDisable(CurrentDisableType);
            characterAnimation.ChangeAnimationWithDuration(getClip(CurrentDisableType), isForce: true, layer: ANIMATION_LAYER);
        }

        public virtual void ActivateDisable(DisableType disableType)
        {
            if((so_GameDisable.BlockActions[disableType] & DisableCategory.ItemUsage) != 0)
                stateMachine.GetState<CharacterItemUsageState>()?.DeactivateUsage();
            if((so_GameDisable.BlockActions[disableType] & DisableCategory.AbilityUsage) != 0)
                stateMachine.GetState<CharacterAbilityUsageState>()?.DeactivateUsage();
            if((so_GameDisable.BlockActions[disableType] & DisableCategory.Movement) != 0)
                stateMachine.GetInterfaceImplementingClass<IMovement>()?.DeactivateMovement();
            if((so_GameDisable.BlockActions[disableType] & DisableCategory.Rotation) != 0)
                stateMachine.GetInterfaceImplementingClass<IRotate>()?.DeactivateRotate();
            
            BlockAction(so_GameDisable.BlockActions[disableType]);
        }

        public virtual void DeactivateDisable(DisableType disableType)
        {
            UnblockAction(so_GameDisable.BlockActions[disableType]);
            
            if(!IsActionBlocked(DisableCategory.ItemUsage))
                stateMachine.GetState<CharacterItemUsageState>()?.ActivateUsage();
            if(!IsActionBlocked(DisableCategory.AbilityUsage))
                stateMachine.GetState<CharacterAbilityUsageState>()?.ActivateUsage();
            if(!IsActionBlocked(DisableCategory.Movement))
                stateMachine.GetInterfaceImplementingClass<IMovement>()?.ActivateMovement();
            if(!IsActionBlocked(DisableCategory.Rotation))
                stateMachine.GetInterfaceImplementingClass<IRotate>()?.ActivateRotate();
            
            foreach (int value in blockedActions.Values)
            {
                if (value > 0) return;
            }
            
            IsCanExit = true;
            stateMachine.ExitCategory(Category, null);
        }

        public void CleanseDisable()
        {
            IsCanExit = true;
            foreach (var VARIABLE in blockedActions.Keys)
                blockedActions[VARIABLE] = 0;
            stateMachine.ExitCategory(Category, null);
        }
    }
    
    public abstract class CharacterDisableStateBuilder : StateBuilder<CharacterDisableState>
    {
        public CharacterDisableStateBuilder(CharacterDisableState instance) : base(instance)
        {
        }
        
        public CharacterDisableStateBuilder SetGameObject(GameObject gameObject)
        {
            state.SetGameObject(gameObject);
            return this;
        }

        public CharacterDisableStateBuilder SetConfig(SO_CharacterDisable so_CharacterDisable)
        {
            state.SetConfig(so_CharacterDisable);
            return this;
        }

        public CharacterDisableStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            state.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}