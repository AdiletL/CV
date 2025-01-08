using System;
using System.Collections;
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
            
            return so_LevelContainer.levels[levelNumber];
        }

        private GameFieldController GetGameField(int levelIndex, int gameFieldIndex)
        {
            var level = GetLevel(levelIndex);
            if(level.GameFieldControllers.Length < gameFieldIndex)
                throw new IndexOutOfRangeException();
            
            return level.GameFieldControllers[gameFieldIndex];
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
            var gameField = GetGameField(currentLevelIndex, currentGameFieldIndex);
            var newGameObject = diContainer.InstantiatePrefab(gameField);
            newGameObject.transform.SetParent(levelController.transform);
            var gameFieldController = newGameObject.GetComponent<GameFieldController>();
            gameFieldController.Initialize();
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