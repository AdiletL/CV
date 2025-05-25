using ScriptableObjects.Ability;
using UnityEngine;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private LevelManager levelManager;
        [Inject] private SO_AbilityContainer so_AbilityContainer;
        
        public void Initialize()
        {
            //Debug.Log(so_AbilityContainer);
            levelManager.StartLevel();
        }
    }
}
