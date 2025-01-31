using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerSwitchMoveSync : CharacterSwitchMoveSync
    {
        private PhotonView photonView;
        private StateMachine stateMachine;
        private PlayerSwitchMoveState _playerSwitchMoveState;
        
        public void Initialize(StateMachine stateMachine, PlayerSwitchMoveState playerSwitchMoveState)
        {
            this.stateMachine = stateMachine;
            this._playerSwitchMoveState = playerSwitchMoveState;
            photonView = GetComponent<PhotonView>();
        }

        public void InitializeRunState()
        {
            if(!photonView.IsMine) return;
            
            photonView.RPC(nameof(InitializeRunStateRPC), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void InitializeRunStateRPC()
        {
            if(photonView.IsMine) return;
            
            _playerSwitchMoveState.InitializeRunState();
        }
    }
}