using Machine;
using Photon.Pun;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerStateSync : CharacterStateSync
    {
        private CharacterSwitchMoveState _characterSwitchMoveState;
        private CharacterSwitchAttackState _characterSwitchAttackState;
        
        public void Initialize(StateMachine stateMachine, CharacterSwitchMoveState characterSwitchMoveState, 
            CharacterSwitchAttackState characterSwitchAttackState)
        {
            this.stateMachine = stateMachine;
            this._characterSwitchMoveState = characterSwitchMoveState;
            this._characterSwitchAttackState = characterSwitchAttackState;
            photonView = GetComponent<PhotonView>();
        }

        public void SetTargetForMove(GameObject target)
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(SetTargetForMoveRPC), RpcTarget.All, target.GetComponent<PhotonView>().ViewID);
        }

        [PunRPC]
        private void SetTargetForMoveRPC(int viewID)
        {
            var target = PhotonView.Find(viewID).gameObject;
            _characterSwitchMoveState.SetTarget(target);
        }
        public void SetSwitchMove()
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(SetSwitchMoveRPC), RpcTarget.All);
        }

        [PunRPC]
        private void SetSwitchMoveRPC()
        {
            _characterSwitchMoveState.SetState();
        }

        public void ExitCategorySwitchMove(StateCategory category)
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(ExitCategorySwitchMoveRPC), RpcTarget.All, (int)category);
        }

        [PunRPC]
        private void ExitCategorySwitchMoveRPC(int category)
        {
            _characterSwitchMoveState.ExitCategory((StateCategory)category);
        }

        public void ExitOtherStatesSwitchMove()
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(ExitOtherStatesSwitchMoveRPC), RpcTarget.All);
        }

        [PunRPC]
        private void ExitOtherStatesSwitchMoveRPC()
        {
            _characterSwitchMoveState.ExitOtherStates();
        }
        
        public void SetSwitchAttack()
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(SetSwitchAttackRPC), RpcTarget.All);
        }

        [PunRPC]
        private void SetSwitchAttackRPC()
        {
            _characterSwitchAttackState.SetState();
        }

        public void ExitCategorySwitchAttack()
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(ExitCategorySwitchAttackRPC), RpcTarget.All);
        }

        [PunRPC]
        private void ExitCategorySwitchAttackRPC()
        {
            _characterSwitchAttackState.SetState();
        }
       
        public void ExitOtherStatesSwitchAttack()
        {
            if(!photonView.IsMine) return;
            photonView.RPC(nameof(ExitOtherStatesSwitchAttackRPC), RpcTarget.All);
        }

        [PunRPC]
        private void ExitOtherStatesSwitchAttackRPC()
        {
            _characterSwitchAttackState.SetState();
        }
    }
}