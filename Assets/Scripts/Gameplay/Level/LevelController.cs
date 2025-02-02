using System.Collections.Generic;
using Photon.Pun;
using Unit.Character;
using Unit.Trap;
using UnityEngine;

namespace Gameplay
{
    public class LevelController : MonoBehaviour
    {
        public RoomController CurrentRoom { get; private set; }
        
        public void Initialize()
        {
            
        }

        public void SetGameField(RoomController roomController)
        {
            CurrentRoom = roomController;
        }
    }
}