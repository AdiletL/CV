using Photon.Pun;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private SO_PlayerMove so_PlayerMove;
        private PhotonView photonView;
        private PlayerKinematicControl playerKinematicControl;
        private CharacterStatsController characterStatsController;
        
        private Vector3 directionMovement;
        private float consumptionEnduranceRate;
        private float currentConsumptionEnduranceRate;
        private bool isAddedEnduranceStat;
        
        public Stat RotationSpeedStat { get; } = new Stat();
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetCharacterStatsController(CharacterStatsController characterStatsController) => this.characterStatsController = characterStatsController;
        public void SetRunConsumptionEnduranceRate(float consumptionEnduranceRate) => this.consumptionEnduranceRate = consumptionEnduranceRate;

        public override void Initialize()
        {
            base.Initialize();
            so_PlayerMove = (SO_PlayerMove)so_CharacterMove;
            RotationSpeedStat.AddCurrentValue(so_PlayerMove.RotateSpeed);
        }

        public override void Enter()
        {
            base.Enter();
            currentClip = getRandomClip(runClips);
            PlayAnimation();
        }

        public override void Subscribe()
        {
            base.Subscribe();
            stateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            stateMachine.OnExitCategory -= OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            
            CheckDirectionMovement();
            Rotate();
            ExecuteMovement();
            
            if (directionMovement.magnitude == 0)
                stateMachine.ExitCategory(Category, typeof(CharacterIdleState));
        }

        public override void Exit()
        {
            base.Exit();
            ClearRegenerationEnduranceStat();
        }

        private void OnExitCategory(IState state)
        {
            if (typeof(CharacterJumpState).IsAssignableFrom(state.GetType()))
                PlayAnimation();
            if (typeof(CharacterSpecialActionState).IsAssignableFrom(state.GetType()))
                PlayAnimation();
        }
        
        private void CheckDirectionMovement()
        {
            directionMovement = Vector3.zero;
            
            if (Input.GetKey(KeyCode.A)) directionMovement.x = -1;
            if (Input.GetKey(KeyCode.D)) directionMovement.x = 1;
            if (Input.GetKey(KeyCode.W)) directionMovement.z = 1;
            if (Input.GetKey(KeyCode.S)) directionMovement.z = -1;
            
            if(directionMovement.magnitude > 0)
                directionMovement.Normalize();
        }
        
        public override void ExecuteMovement()
        {
            base.ExecuteMovement();
            playerKinematicControl.SetVelocity(directionMovement * (MovementSpeedStat.CurrentValue));
            playerKinematicControl.SetRotationSpeed(RotationSpeedStat.CurrentValue);
            AddRegenerationEnduranceStat();
        }

        private void Rotate()
        {
            playerKinematicControl.SetDirectionRotate(directionMovement);
        }

        private void AddRegenerationEnduranceStat()
        {
            if (!isAddedEnduranceStat)
            {
                characterStatsController.GetStat(StatType.RegenerationEndurance).RemoveCurrentValue(consumptionEnduranceRate);
                isAddedEnduranceStat = true;
            }
        }

        private void ClearRegenerationEnduranceStat()
        {
            if (isAddedEnduranceStat)
            {
                characterStatsController.GetStat(StatType.RegenerationEndurance).AddCurrentValue(consumptionEnduranceRate);
                isAddedEnduranceStat = false;
            }
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }
        
        public PlayerMoveStateBuilder SetPhotonView(PhotonView photonView)
        {
            if(state is PlayerMoveState playerMoveState)
                playerMoveState.SetPhotonView(photonView);
            return this;
        }
        
        public PlayerMoveStateBuilder SetConsumptionEnduranceRate(float consumptionEnduranceRate)
        {
            if(state is PlayerMoveState playerMoveState)
                playerMoveState.SetRunConsumptionEnduranceRate(consumptionEnduranceRate);
            return this;
        }
        
        public PlayerMoveStateBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if(state is PlayerMoveState playerMoveState)
                playerMoveState.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
        public PlayerMoveStateBuilder SetCharacterStatsController(CharacterStatsController characterStatsController)
        {
            if(state is PlayerMoveState playerMoveState)
                playerMoveState.SetCharacterStatsController(characterStatsController);
            return this;
        }
    }
}