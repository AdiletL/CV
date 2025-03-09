using System;
using Gameplay.Unit.Character.Player;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(NextRoomConfigHandler))]
    public class TriggerOnNextRoom : MonoBehaviour
    {
        private NextRoomConfigHandler _nextRoomConfigHandler;
        private bool isTriggerPlayer;

        private void Start()
        {
            _nextRoomConfigHandler = GetComponent<NextRoomConfigHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(isTriggerPlayer) return;
            if (other.TryGetComponent(out PlayerController player))
            {
                isTriggerPlayer = true;
                _nextRoomConfigHandler.TriggerNextRoom();
            }
        }
    } 
}