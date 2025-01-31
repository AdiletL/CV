using System;
using Machine;
using UnityEngine;

namespace Unit
{
    public abstract class UnitStateSync : MonoBehaviour
    {
        protected StateMachine stateMachine;
        
        public abstract void ActiveBlockChangeState();
        protected abstract void ActiveBlockChangeStateRPC();
        public abstract void InActiveBlockChangeState();
        protected abstract void InActiveBlockChangeStateRPC();
        
        public abstract void SetState(bool isForceSetState = false, params Type[] desiredStates);
        public abstract void ExitCategory(StateCategory excludedCategory, Type installationState, bool isForceSetState = false);
        public abstract void ExitOtherStates(Type installationState, bool isForceSetState = false);

        protected abstract void SetStateRPC(bool isForceSetState, string[] typeNames);

        protected abstract void ExitCategoryRPC(int excludedCategory, string installationStateName,
            bool isForceSetState);

        protected abstract void ExitOtherStatesRPC(string installationStateName, bool isForceSetState);
    }
}