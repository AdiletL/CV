using Gameplay.Ability;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSpecialActionState : CharacterSpecialActionState
    {
        private PlayerRunState playerRunState;
        private BlockPhysicalDamageConfig blockPhysicalDamageConfig;
        private BlockPhysicalDamageAbility blockPhysicalDamageAbility;
        private CharacterAnimation characterAnimation;
        private AnimationClip blockClip;
        
        private float changedRotateSpeedValue;
        private float changedMovementSpeedValue;

        private const float MULTIPLY_MOVEMENT_SPEED = .5f;
        private const int ANIMATION_LAYER = 2;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetBlockClip(AnimationClip blockClip) => this.blockClip = blockClip;
        public void SetBlockPhysicalDamageConfig(BlockPhysicalDamageConfig config) => this.blockPhysicalDamageConfig = config;

        private BlockPhysicalDamageAbility CreateBlockPhysicalDamage()
        {
            return (BlockPhysicalDamageAbility)new BlockPhysicalDamageBuilder()
                .SetNormalDamageResistanceConfig(blockPhysicalDamageConfig.NormalDamageResistanceConfig)
                .SetBlockedInputType(blockPhysicalDamageConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(blockPhysicalDamageConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetCooldown(blockPhysicalDamageConfig.Cooldown)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            blockPhysicalDamageAbility = CreateBlockPhysicalDamage();
            blockPhysicalDamageAbility.Initialize();
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            blockPhysicalDamageAbility.Enter();
            
            var durationAnimation = blockClip.length;
            characterAnimation.ChangeAnimationWithDuration(blockClip, durationAnimation, isForce: true, layer: ANIMATION_LAYER);
            
            playerRunState ??= stateMachine.GetState<PlayerRunState>();
            if(playerRunState == null) return;
            
            changedMovementSpeedValue = playerRunState.MovementSpeedStat.CurrentValue * MULTIPLY_MOVEMENT_SPEED;
            playerRunState.MovementSpeedStat.RemoveValue(changedMovementSpeedValue);
            
            changedRotateSpeedValue = playerRunState.RotationSpeedStat.CurrentValue;
            playerRunState.RotationSpeedStat.RemoveValue(changedRotateSpeedValue);
        }

        public override void Exit()
        {
            base.Exit();
            blockPhysicalDamageAbility.FinishEffect();
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            IsCanExit = true;
            
            playerRunState?.MovementSpeedStat.AddValue(changedMovementSpeedValue);
            playerRunState?.RotationSpeedStat.AddValue(changedRotateSpeedValue);
        }
    }

    public class PlayerSpecialActionStateBuilder : CharacterSpecialActionStateBuilder
    {
        public PlayerSpecialActionStateBuilder() : base(new PlayerSpecialActionState())
        {
        }

        public PlayerSpecialActionStateBuilder SetBlockPhysicalDamageConfig(BlockPhysicalDamageConfig config)
        {
            if(state is PlayerSpecialActionState playerSpecialActionState)
                playerSpecialActionState.SetBlockPhysicalDamageConfig(config);
            return this;
        }
        public PlayerSpecialActionStateBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(state is PlayerSpecialActionState playerSpecialActionState)
                playerSpecialActionState.SetCharacterAnimation(characterAnimation);
            return this;
        }
        public PlayerSpecialActionStateBuilder SetBlockClip(AnimationClip blockClip)
        {
            if(state is PlayerSpecialActionState playerSpecialActionState)
                playerSpecialActionState.SetBlockClip(blockClip);
            return this;
        }
    }
}