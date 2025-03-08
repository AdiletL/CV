using System;
using System.Linq;
using Machine;
using Photon.Pun;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterStateSync : UnitStateSync
    {
        protected PhotonView photonView;

        public void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            photonView = GetComponent<PhotonView>();
        }

        public override void ActiveBlockChangeState()
        {
            if (!photonView.IsMine) return;
            
            photonView.RPC(nameof(ActiveBlockChangeStateRPC), RpcTarget.All);
        }

        [PunRPC]
        protected override void ActiveBlockChangeStateRPC()
        {
            if (photonView.IsMine) return;
            
            stateMachine.ActiveBlockChangeState();
        }

        public override void InActiveBlockChangeState()
        {
            if (!photonView.IsMine) return;
            
            photonView.RPC(nameof(InActiveBlockChangeStateRPC), RpcTarget.All);
        }
        [PunRPC]
        protected override void InActiveBlockChangeStateRPC()
        {
            if (photonView.IsMine) return;
            
            stateMachine.InActiveBlockChangeState();
        }

        public override void SetState(bool isForceSetState = false, params Type[] desiredStates)
        {
            if (!photonView.IsMine) return;

            string[] stateNames = desiredStates.Select(t => t.AssemblyQualifiedName).ToArray();
            photonView.RPC(nameof(SetStateRPC), RpcTarget.All, isForceSetState, stateNames);
        }

        [PunRPC]
        protected override void SetStateRPC(bool isForceSetState, string[] typeNames)
        {
            if (photonView.IsMine) return;
            
            Type[] types = typeNames
                .Select(TryGetType)
                .Where(t => t != null)
                .ToArray();

            stateMachine.SetStates(isForceSetState, types);
        }

        public override void ExitCategory(StateCategory excludedCategory, Type installationState, bool isForceSetState = false)
        {
            if (!photonView.IsMine) return;

            photonView.RPC(nameof(ExitCategoryRPC), RpcTarget.All, (int)excludedCategory, installationState?.AssemblyQualifiedName, isForceSetState);
        }

        [PunRPC]
        protected override void ExitCategoryRPC(int excludedCategory, string installationStateName, bool isForceSetState)
        {
            if (photonView.IsMine) return;
            
            Type stateType = TryGetType(installationStateName);

            stateMachine.ExitCategory((StateCategory)excludedCategory, stateType, isForceSetState);
        }

        public override void ExitOtherStates(Type installationState, bool isForceSetState = false)
        {
            if (!photonView.IsMine) return;

            photonView.RPC(nameof(ExitOtherStatesRPC), RpcTarget.All, installationState?.AssemblyQualifiedName, isForceSetState);
        }

        [PunRPC]
        protected override void ExitOtherStatesRPC(string installationStateName, bool isForceSetState)
        {
            if (photonView.IsMine) return;
            
            Type stateType = TryGetType(installationStateName);
            
Debug.Log(stateMachine);
Debug.Log(stateType);
            stateMachine.ExitOtherStates(stateType, isForceSetState);
        }

        protected static Type TryGetType(string typeName)
        {
            return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName, false);
        }
    }
}