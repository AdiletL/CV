using System;
using ScriptableObjects.Gameplay;
using UnityEngine;

namespace Gameplay
{
    public class NextRoomConfigHandler : MonoBehaviour
    {
        public event Action<int> OnTrigger;

        [SerializeField] private SO_Room nextRoomConfig;

        public void TriggerNextRoom()
        {
            if(nextRoomConfig != null)
                OnTrigger?.Invoke(nextRoomConfig.ID);
        }
    }
}