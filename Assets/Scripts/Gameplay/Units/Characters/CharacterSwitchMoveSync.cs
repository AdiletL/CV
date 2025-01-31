using Photon.Pun;
using UnityEngine;

namespace Unit.Character
{
    public abstract class CharacterSwitchMoveSync : MonoBehaviour
    {
        protected PhotonView photonView;
        protected StateMachine stateMachine;

        public void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            photonView = GetComponent<PhotonView>();
        }
    }
}