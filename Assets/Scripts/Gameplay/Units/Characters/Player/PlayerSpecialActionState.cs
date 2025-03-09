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
        private BlockPhysicalDamageConfig blockPhysicalDamageConfig;
        private BlockPhysicalDamageAbility blockPhysicalDamageAbility;
        private CharacterAnimation characterAnimation;
        private AnimationClip blockClip;
        
        private float changedRotateSpeedValue;
        private float changedMovementSpeedValue;

        private const float MULTIPLY_MOVEMENT_SPEED = .5f;
        private const int ANIMATION_LAYER = 2;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
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
            so_PlayerSpecialAction = (SO_PlayerSpecialAction)so_CharacterSpecialAction;
            
            blockPhysicalDamageAbility = CreateBlockPhysicalDamage();
            diContainer.Inject(blockPhysicalDamageAbility);
            blockPhysicalDamageAbility.Initialize();
            blockClip = so_PlayerSpecialAction.AbilityConfigData.BlockPhysicalDamageConfig.BlockClip;
            characterAnimation.AddClip(blockClip);
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            blockPhysicalDamageAbility.Enter();
            
            var durationAnimation = blockClip.length;
            characterAnimation.ChangeAnimationWithDuration(blockClip, durationAnimation, isForce: true, layer: ANIMATION_LAYER);
            
            playerMoveState ??= stateMachine.GetState<PlayerMoveState>();
            if(playerMoveState == null) return;
            
            changedMovementSpeedValue = playerMoveState.MovementSpeedStat.CurrentValue * MULTIPLY_MOVEMENT_SPEED;
            playerMoveState.MovementSpeedStat.RemoveValue(changedMovementSpeedValue);
            
            changedRotateSpeedValue = playerMoveState.RotationSpeedStat.CurrentValue;
            playerMoveState.RotationSpeedStat.RemoveValue(changedRotateSpeedValue);
        }

        public override void Update()
        {
            base.Update();
            blockPhysicalDamageAbility.Update();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
            blockPhysicalDamageAbility.LateUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            blockPhysicalDamageAbility.Exit();
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            IsCanExit = true;
            
            playerMoveState?.MovementSpeedStat.AddValue(changedMovementSpeedValue);
            playerMoveState?.RotationSpeedStat.AddValue(changedRotateSpeedValue);
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
    }
}