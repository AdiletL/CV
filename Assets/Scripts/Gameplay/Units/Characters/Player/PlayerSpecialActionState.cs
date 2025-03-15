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
        private DamageResistanceConfig damageResistanceConfig;
        private DamageResistanceAbility damageResistanceAbility;
        private CharacterAnimation characterAnimation;
        private AnimationClip blockClip;
        
        private float changedRotateSpeedValue;
        private float changedMovementSpeedValue;

        private const float MULTIPLY_MOVEMENT_SPEED = .5f;
        private const int ANIMATION_LAYER = 2;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetDamageResistanceAbilityConfig(DamageResistanceConfig config) => this.damageResistanceConfig = config;

        private DamageResistanceAbility CreateBlockPhysicalDamage()
        {
            return (DamageResistanceAbility)new DamageResistanceAbilityBuilder()
                .SetStatConfigs(damageResistanceConfig.StatConfigs)
                .SetBlockedInputType(damageResistanceConfig.SO_BaseAbilityConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(damageResistanceConfig.SO_BaseAbilityConfig.AbilityBehaviour)
                .SetCooldown(damageResistanceConfig.Cooldown)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerSpecialAction = (SO_PlayerSpecialAction)so_CharacterSpecialAction;
            
            damageResistanceAbility = CreateBlockPhysicalDamage();
            diContainer.Inject(damageResistanceAbility);
            damageResistanceAbility.Initialize();
            blockClip = so_PlayerSpecialAction.AbilityConfigData.DamageResistanceConfig.Clip;
            characterAnimation.AddClip(blockClip);
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            damageResistanceAbility.Enter();
            
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
            damageResistanceAbility.Update();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
            damageResistanceAbility.LateUpdate();
        }

        public override void Exit()
        {
            damageResistanceAbility.Exit();
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            IsCanExit = true;
            
            playerMoveState?.MovementSpeedStat.AddValue(changedMovementSpeedValue);
            playerMoveState?.RotationSpeedStat.AddValue(changedRotateSpeedValue);
            base.Exit();
        }
    }

    public class PlayerSpecialActionStateBuilder : CharacterSpecialActionStateBuilder
    {
        public PlayerSpecialActionStateBuilder() : base(new PlayerSpecialActionState())
        {
        }

        public PlayerSpecialActionStateBuilder SetBlockPhysicalDamageConfig(DamageResistanceConfig config)
        {
            if(state is PlayerSpecialActionState playerSpecialActionState)
                playerSpecialActionState.SetDamageResistanceAbilityConfig(config);
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