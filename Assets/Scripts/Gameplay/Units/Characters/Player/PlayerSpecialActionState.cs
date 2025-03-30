using Gameplay.Ability;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerSpecialActionState : CharacterSpecialActionState
    {
        [Inject] private DiContainer diContainer;
        
        private SO_PlayerSpecialAction so_PlayerSpecialAction;
        private PlayerMoveState playerMoveState;
        private CharacterAnimation characterAnimation;
        private AnimationClip blockClip;
        
        private float changedRotateSpeedValue;
        private float changedMovementSpeedValue;

        private const float MULTIPLY_MOVEMENT_SPEED = .5f;
        private const int ANIMATION_LAYER = 3;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerSpecialAction = (SO_PlayerSpecialAction)so_CharacterSpecialAction;
            
            blockClip = so_PlayerSpecialAction.AbilityConfigData.DamageResistanceConfig.Clip;
            characterAnimation.AddClip(blockClip);
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            
            var durationAnimation = blockClip.length;
            characterAnimation.ChangeAnimationWithDuration(blockClip, durationAnimation, isForce: true, layer: ANIMATION_LAYER);
        }

        public override void Exit()
        {
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            IsCanExit = true;
            base.Exit();
        }
    }

    public class PlayerSpecialActionStateBuilder : CharacterSpecialActionStateBuilder
    {
        public PlayerSpecialActionStateBuilder() : base(new PlayerSpecialActionState())
        {
        }
        
        public PlayerSpecialActionStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is PlayerSpecialActionState playerSpecialActionState)
                playerSpecialActionState.SetCharacterAnimation(characterAnimation);
            return this;
        }
    }
}