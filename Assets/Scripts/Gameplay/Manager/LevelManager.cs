using System;
using Cysharp.Threading.Tasks;
using ScriptableObjects.Gameplay;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviour, IManager
    {
        private DiContainer diContainer;
        private GameUnits gameUnits;
        
        [SerializeField] protected AssetReference so_LevelContainer;
        [SerializeField] private AssetReferenceGameObject playerPrefab;

        private SO_LevelContainer levelContainer;
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
            if (levelContainer.Levels.Length < levelNumber)
                throw new IndexOutOfRangeException();
            
            return levelContainer.Levels[levelNumber];
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

        private async UniTask<SO_LevelContainer> LoadLevelContainer()
        {
            return await Addressables.LoadAssetAsync<SO_LevelContainer>(so_LevelContainer);
        }
        
        public async UniTask Initialize()
        {
            if (!levelController)
            {
                levelController = CreateLevelController();
                levelController.Initialize();
            }
            
            if (!levelContainer) levelContainer = await LoadLevelContainer();
        }


        public async UniTask StartLevel()
        {
            var gameField = GetGameField(currentLevelIndex, currentGameFieldIndex);
            var newGameField = diContainer.InstantiatePrefab(gameField);
            newGameField.transform.SetParent(levelController.transform);
            var gameFieldController = newGameField.GetComponent<GameFieldController>();
            gameFieldController.Initialize();
            gameFieldController.StartGame();
            
            levelController.SetGameField(gameFieldController);

            InstantiatePlayer();
        }


        public void RestartLevel()
        {
            
        }

        public void StopLevel()
        {
            
        }

        private async void InstantiatePlayer()
        {
            var player = await Addressables.LoadAssetAsync<GameObject>(playerPrefab);
            playerController = diContainer.InstantiatePrefabForComponent<PlayerController>(player);
            diContainer.Inject(playerController);
            diContainer.Bind<PlayerController>().FromInstance(playerController);
            
            playerController.GetComponent<CharacterController>().enabled = false;
            playerController.transform.position = levelController.CurrentGameField.PlayerSpawnPoint.position;
            playerController.transform.rotation = levelController.CurrentGameField.PlayerSpawnPoint.rotation;
            playerController.Initialize();
            playerController.GetComponent<CharacterController>().enabled = true;
            gameUnits.AddUnits(playerController);
        }
    }
}