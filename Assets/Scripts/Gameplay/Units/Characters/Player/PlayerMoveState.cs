using Movement;
using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        private PhotonView photonView;
        private PlayerKinematicControl playerKinematicControl;
        
        private Vector3 directionMovement;
        private float reductionEndurance;
        
        public Stat RotationSpeedStat { get; private set; } = new Stat();
        
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
        public void SetRunReductionEndurance(float runReductionEndurance) => this.reductionEndurance = runReductionEndurance;
        

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

        private void OnExitCategory(IState state)
        {
            if (typeof(CharacterJumpState).IsAssignableFrom(state.GetType()))
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
            unitEndurance.EnduranceStat.RemoveValue(reductionEndurance);
        }

        private void Rotate()
        {
            playerKinematicControl.SetDirectionRotate(directionMovement);
        }
    }

    public class PlayerMoveStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveStateBuilder() : base(new PlayerMoveState())
        {
        }
        
        public PlayerMoveStateBuilder SetPhotonView(PhotonView photonView)
        {
            if(state is PlayerMoveState playerRunStateOrig)
                playerRunStateOrig.SetPhotonView(photonView);
            return this;
        }
        
        public PlayerMoveStateBuilder SetReductionEndurance(float reductionEndurance)
        {
            if(state is PlayerMoveState playerRunStateOrig)
                playerRunStateOrig.SetRunReductionEndurance(reductionEndurance);
            return this;
        }
        
        public PlayerMoveStateBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if(state is PlayerMoveState playerRunStateOrig)
                playerRunStateOrig.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
    }
}