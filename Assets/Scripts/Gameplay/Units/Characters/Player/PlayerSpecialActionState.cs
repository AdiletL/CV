using Gameplay.Ability;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSpecialActionState : CharacterSpecialActionState
    {
        private BlockPhysicalDamageConfig blockPhysicalDamageConfig;
        private BlockPhysicalDamage blockPhysicalDamage;
        private CharacterAnimation characterAnimation;
        private AnimationClip blockClip;
        private const int ANIMATION_LAYER = 2;
        
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetBlockClip(AnimationClip blockClip) => this.blockClip = blockClip;
        public void SetBlockPhysicalDamageConfig(BlockPhysicalDamageConfig config) => this.blockPhysicalDamageConfig = config;

        private BlockPhysicalDamage CreateBlockPhysicalDamage()
        {
            return (BlockPhysicalDamage)new BlockPhysicalDamageBuilder()
                .SetNormalDamageResistanceConfig(blockPhysicalDamageConfig.NormalDamageResistanceConfig)
                .SetBlockedInputType(blockPhysicalDamageConfig.BlockedInputType)
                .SetGameObject(gameObject)
                .SetAbilityBehaviour(blockPhysicalDamageConfig.AbilityBehaviour)
                .SetCooldown(blockPhysicalDamageConfig.Cooldown)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            blockPhysicalDamage = CreateBlockPhysicalDamage();
            blockPhysicalDamage.Initialize();
        }

        public override void Enter()
        {
            base.Enter();
            IsCanExit = false;
            blockPhysicalDamage.Activate();
            var durationAnimation = blockClip.length;
            characterAnimation.ChangeAnimationWithDuration(blockClip, durationAnimation, isForce: true, layer: ANIMATION_LAYER);
        }

        public override void Exit()
        {
            base.Exit();
            blockPhysicalDamage.Finish();
            characterAnimation.ExitAnimation(ANIMATION_LAYER);
            IsCanExit = true;
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