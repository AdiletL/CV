using Photon.Pun;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerMoveState : CharacterMoveState, IRotate
    {
        private SO_PlayerMove so_PlayerMove;
        private PhotonView photonView;
        private PlayerKinematicControl playerKinematicControl;
        private CharacterStatsController characterStatsController;
        
        private Vector3 directionMovement;
        private float runConsumptionEndurance;
        
        public Stat RotationSpeedStat { get; } = new Stat();
        public bool IsCanRotate { get; private set; } = true;
        
        ~PlayerMoveState()
        {
            UnsubscribeStat();
        }
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetCharacterStatsController(CharacterStatsController characterStatsController) => this.characterStatsController = characterStatsController;
        public void SetRunConsumptionEnduranceRate(float consumptionEnduranceRate) => this.runConsumptionEndurance = consumptionEnduranceRate;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeStat();
            
            so_PlayerMove = (SO_PlayerMove)so_CharacterMove;
            RotationSpeedStat.AddCurrentValue(so_PlayerMove.RotateSpeed);
        }

        public override void Enter()
        {
            base.Enter();
            currentClip = getRandomClip(runClips);
            AddRegenerationEnduranceStat();
        }

        public override void Subscribe()
        {
            base.Subscribe();
            stateMachine.OnExitCategory += OnExitCategory;
        }

        private void SubscribeStat()
        {
            
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            stateMachine.OnExitCategory -= OnExitCategory;
        }

        private void UnsubscribeStat()
        {
            
        }
        
        public override void Update()
        {
            base.Update();
            
            CheckDirectionMovement();
            ExecuteRotate();
            ExecuteMovement();
            
            if (directionMovement.magnitude == 0)
                stateMachine.ExitCategory(Category, typeof(CharacterIdleState));
        }

        public override void Exit()
        {
            playerKinematicControl.ClearVelocity();
            playerKinematicControl.SetRotationSpeed(0);
            ClearRegenerationEnduranceStat();
            base.Exit();
        }

        private void OnExitCategory(IState state)
        {
            if (typeof(IJump).IsAssignableFrom(state.GetType()))
                PlayAnimation();
            if (typeof(ISpecialAction).IsAssignableFrom(state.GetType()))
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
            if (IsCanMove)
            {
                playerKinematicControl.SetVelocity(directionMovement * MovementSpeedStat.CurrentValue);
                PlayAnimation();
            }
            else
            {
                playerKinematicControl.ClearVelocity();
            }
        }

        public void ExecuteRotate()
        {
            playerKinematicControl.SetDirectionRotate(directionMovement);
            
            if(IsCanRotate) playerKinematicControl.SetRotationSpeed(RotationSpeedStat.CurrentValue);
            else playerKinematicControl.SetRotationSpeed(0);
        }

        public void ActivateRotate() => IsCanRotate = true;
        public void DeactivateRotate()
        {
            IsCanRotate = false;
            playerKinematicControl.SetRotationSpeed(0);
        }

        public override void DeactivateMovement()
        {
            base.DeactivateMovement();
            playerKinematicControl.ClearVelocity();
        }

        private void AddRegenerationEnduranceStat()
        {
            characterStatsController.GetStat(StatType.RegenerationEndurance).RemoveCurrentValue(runConsumptionEndurance);
        }

        private void ClearRegenerationEnduranceStat()
        {
            characterStatsController.GetStat(StatType.RegenerationEndurance).AddCurrentValue(runConsumptionEndurance);
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