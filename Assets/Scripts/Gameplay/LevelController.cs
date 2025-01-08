using System.Collections.Generic;
using Unit.Character;
using Unit.Trap;
using UnityEngine;

namespace Gameplay
{
    public class LevelController : MonoBehaviour
    {
        public GameFieldController CurrentGameField { get; private set; }
        
        public void Initialize()
        {
            
        }

        public void SetGameField(GameFieldController gameFieldController)
        {
            CurrentGameField = gameFieldController;
        }
    }
}