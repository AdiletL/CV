using System;
using Unit.Character.Player;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(NextRoomContainer))]
    public class TriggerOnNextRoom : MonoBehaviour
    {
        private NextRoomContainer nextRoomContainer;
        private bool isTriggerPlayer;

        private void Start()
        {
            nextRoomContainer = GetComponent<NextRoomContainer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(isTriggerPlayer) return;
            if (other.TryGetComponent(out PlayerController player))
            {
                isTriggerPlayer = true;
                nextRoomContainer.TriggerNextRoom();
            }
        }
    } 
}