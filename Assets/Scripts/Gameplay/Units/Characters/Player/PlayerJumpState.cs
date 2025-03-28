using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerJumpState : CharacterJumpState
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        private SO_PlayerMove so_PlayerMove;
        private PlayerKinematicControl playerKinematicControl;
        private CharacterStatsController characterStatsController;

        private AnimationClip jumpClip;
        private KeyCode jumpKey;
        private int currentJumpCount;
        private int maxJumpCount;

        private readonly float cooldownCheckGround = .1f;
        private float countCooldownCheckGround;
        private float consumptionEndurance;
        private float jumpPower;

        public void SetCharacterStatsController(CharacterStatsController characterStatsController) => this.characterStatsController = characterStatsController;

        
        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)so_CharacterMove;
            playerKinematicControl = gameObject.GetComponent<PlayerKinematicControl>();
            jumpClip = so_PlayerMove.JumpConfig.Clip;
            jumpPower = so_PlayerMove.JumpConfig.Power;
            maxJumpCount = so_PlayerMove.JumpConfig.MaxCount;
            consumptionEndurance = so_PlayerMove.JumpConfig.ConsumptionEndurance;
            jumpKey = so_GameHotkeys.JumpKey;
            characterAnimation.AddClip(jumpClip);
        }

        public override void Enter()
        {
            base.Enter();
            if(!IsCanJump) return;
            IsCanExit = false;
            characterAnimation.SetBlock(true);
            ClearValues();
            StartJump();
            AddRegenerationEnduranceStat();
        }

        public override void Update()
        {
            base.Update();
            if (countCooldownCheckGround > cooldownCheckGround)
            {
                if (Input.GetKeyDown(jumpKey) && currentJumpCount < maxJumpCount)
                { 
                    StartJump();
                }
            }
            countCooldownCheckGround += Time.deltaTime;
            if (countCooldownCheckGround > cooldownCheckGround)
            {
                if (playerKinematicControl.IsGrounded)
                {
                    IsCanExit = true;
                    characterAnimation.SetBlock(false);
                    this.stateMachine.ExitCategory(Category, null);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            IsCanExit = true;
            characterAnimation.SetBlock(false);
            ClearRegenerationEnduranceStat();
        }
        private void ClearValues()
        {
            currentJumpCount = 0;
            countCooldownCheckGround = 0;
        }

        private void AddRegenerationEnduranceStat()
        {
            characterStatsController.GetStat(StatType.RegenerationEndurance).RemoveCurrentValue(consumptionEndurance);
        }

        private void ClearRegenerationEnduranceStat()
        {
            characterStatsController.GetStat(StatType.RegenerationEndurance).AddCurrentValue(consumptionEndurance);
        }
        
        private void StartJump()
        {
            if(!IsCanJump) return;
            
            currentJumpCount++;
            var currentPower = jumpPower;
            if (currentJumpCount >= maxJumpCount)
                currentPower /= 1.3f;
            
            var velocity = new Vector3(0, currentPower, 0);
            characterAnimation.ChangeAnimationWithDuration(jumpClip, isForce: true);
            playerKinematicControl.AddVelocity(velocity);
            playerKinematicControl.ForceUnground();
        }
    }
    
    public class PlayerJumpStateBuilder : CharacterJumpStateBuilder
    {
        public PlayerJumpStateBuilder() : base(new PlayerJumpState())
        {
        }

        public PlayerJumpStateBuilder SetCharacterStatsController(CharacterStatsController characterStatsController)
        {
            if(state is PlayerJumpState playerJumpState)
                playerJumpState.SetCharacterStatsController(characterStatsController);
            
            return this;
        }
    }
}