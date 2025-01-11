using System;
using System.Collections;
using ScriptableObjects.Gameplay;
using Unit;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviour, IManager
    {
        private DiContainer diContainer;
        private GameUnits gameUnits;
        
        [SerializeField] protected SO_LevelContainer so_LevelContainer;

        private LevelController levelController;
        private PlayerController playerController;

        private int currentLevelIndex;
        private int currentGameFieldIndex;

        [Inject]
        private void Construct(DiContainer diContainer, GameUnits gameUnits)
        {
            this.diContainer = diContainer;
            this.gameUnits = gameUnits;
        }
        
        protected virtual SO_Level GetLevel(int levelNumber)
        {
            if (so_LevelContainer.Levels.Length < levelNumber)
                throw new IndexOutOfRangeException();
            
            return so_LevelContainer.Levels[levelNumber];
        }

        private GameFieldController GetGameField(int levelIndex, int gameFieldIndex)
        {
            var level = GetLevel(levelIndex);
            if (level == null)
                throw new NullReferenceException();
            
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


        public void StartLevel(UnitController playerPrefab)
        {
            var gameField = GetGameField(currentLevelIndex, currentGameFieldIndex);
            var newGameField = diContainer.InstantiatePrefab(gameField);
            newGameField.transform.SetParent(levelController.transform);
            var gameFieldController = newGameField.GetComponent<GameFieldController>();
            gameFieldController.Initialize();
            gameFieldController.StartGame();
            
            levelController.SetGameField(gameFieldController);

            InstantiatePlayer(playerPrefab);
        }


        public void RestartLevel()
        {
            
        }

        public void StopLevel()
        {
            
        }

        private void InstantiatePlayer(UnitController playerPrefab)
        {
            var newPlayer = diContainer.InstantiatePrefab(playerPrefab);
            var playerController = newPlayer.GetComponent<PlayerController>();
            playerController.GetComponent<CharacterController>().enabled = false;
            diContainer.Inject(playerController);
            playerController.transform.position = levelController.CurrentGameField.PlayerSpawnPoint.position;
            playerController.transform.rotation = levelController.CurrentGameField.PlayerSpawnPoint.rotation;
            playerController.Initialize();
            playerController.GetComponent<CharacterController>().enabled = true;
            gameUnits.AddUnits(playerController);
        }
    }
}