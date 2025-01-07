using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private SO_LevelContainer so_LevelContainer;

        private LevelController levelController;

        private int currentLevelIndex;
        private int currentGameFieldIndex;
        
        private SO_Level GetLevel(int levelNumber)
        {
            if (so_LevelContainer.levels.Length < levelNumber)
                throw new IndexOutOfRangeException();
            
            return so_LevelContainer.levels[levelNumber - 1];
        }

        private GameFieldController GetGameField(int levelNumber, int index)
        {
            if(GetLevel(levelNumber).GameFieldControllers.Length < index)
                throw new IndexOutOfRangeException();
            
            return GetLevel(levelNumber).GameFieldControllers[index];
        }
        
        private LevelController CreateLevelController()
        {
            var newGameObject = new GameObject("LevelController");
            return newGameObject.AddComponent<LevelController>();
        }
        
        public void Initialize()
        {
            if (!levelController)
            {
                levelController = CreateLevelController();
                levelController.Initialize();
            }
        }


        public void StartLevel()
        {
            var gameFieldController = diContainer.InstantiatePrefabForComponent<GameFieldController>(GetGameField(currentLevelIndex, currentGameFieldIndex));
            gameFieldController.transform.SetParent(levelController.transform);
            gameFieldController.GetComponent<GameFieldController>().Initialize();
            gameFieldController.StartGame();
        }

        public void RestartLevel()
        {
            
        }

        public void StopLevel()
        {
            
        }
    }
}