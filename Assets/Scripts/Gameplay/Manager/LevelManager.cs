using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviourPun, IManager
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;

        [SerializeField] protected AssetReference so_LevelContainer;
        [SerializeField] private AssetReferenceGameObject playerPrefab;
        [SerializeField] private AssetReferenceGameObject cameraPrefab;

        private SO_LevelContainer levelContainer;
        private LevelController levelController;
        private PlayerController playerController;

        private int currentLevelIndex;
        private int currentGameFieldIndex;

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

            if (level.GameFieldControllers.Length < gameFieldIndex)
                throw new IndexOutOfRangeException();

            var result =  Addressables.LoadAssetAsync<GameObject>(level.GameFieldControllers[gameFieldIndex]).Result;
            return result.GetComponent<GameFieldController>();
        }

        private AssetReference GetGameFieldReference(int levelIndex, int gameFieldIndex)
        {
            var level = GetLevel(levelIndex);
            if (level == null)
                throw new NullReferenceException();

            if (level.GameFieldControllers.Length < gameFieldIndex)
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
        
        public void StartLevelTest()
        {
            PhotonNetwork.Instantiate(cameraPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            if (!PhotonNetwork.IsMasterClient)
            {
                InstantiatePlayer();
                return;
            }
            
            if (photonView != null)
            {
                photonView.RPC("StartLevel", RpcTarget.All);
            }
            else
            {
                Debug.LogError("PhotonView is missing or is not assigned correctly.");
            }
        }

        [PunRPC]
        public void StartLevel()
        {
            var gameField = GetGameFieldReference(currentLevelIndex, currentGameFieldIndex);
            var newGameField = PhotonNetwork.Instantiate(gameField.AssetGUID, Vector3.zero, Quaternion.identity);
            newGameField.transform.SetParent(levelController.transform);
            var gameFieldController = newGameField.GetComponent<GameFieldController>();
            diContainer.Inject(gameFieldController);
            gameFieldController.Initialize();
            gameFieldController.StartGame();

            levelController.SetGameField(gameFieldController);
            
            InstantiatePlayer();
        }

        public void RestartLevel()
        {
            // Реализация перезапуска уровня
        }

        public void StopLevel()
        {
            // Реализация остановки уровня
        }

        private void InstantiatePlayer()
        {
            var player = PhotonNetwork.Instantiate(playerPrefab.AssetGUID, levelController.CurrentGameField.PlayerSpawnPoint.position, levelController.CurrentGameField.PlayerSpawnPoint.rotation);
            
            playerController = player.GetComponent<PlayerController>();
            diContainer.Inject(playerController);
            diContainer.Bind<PlayerController>().FromInstance(playerController);

            playerController.GetComponent<CharacterController>().enabled = false;
            playerController.Initialize();
            playerController.GetComponent<CharacterController>().enabled = true;

            gameUnits.AddUnits(player);
            playerController.Appear();
        }
    }
}
