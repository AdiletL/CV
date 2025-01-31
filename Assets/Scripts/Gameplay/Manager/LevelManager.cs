using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using Unit.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviourPun, IManager
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;

        [SerializeField] protected SO_LevelContainer so_LevelContainerPrefab;
        [SerializeField] private AssetReferenceGameObject playerPrefab;
        [SerializeField] private GameObject cameraPrefab;
        [SerializeField] private AssetReferenceGameObject levelControllerPrefab;

        private LevelController levelController;
        private PlayerController playerController;
        
        private PhotonView photonView;

        private int currentLevelIndex;
        private int currentGameFieldIndex;

        private SO_Level GetLevel(int levelNumber)
        {
            if (so_LevelContainerPrefab.Levels.Length < levelNumber)
                throw new IndexOutOfRangeException();

            return so_LevelContainerPrefab.Levels[levelNumber];
        }
        private AssetReference GetGameFieldReference(int levelIndex, int gameFieldIndex)
        {
            var level = GetLevel(levelIndex);
            if (level == null)
                throw new NullReferenceException();

            if (level.GameFields.Length < gameFieldIndex)
                throw new IndexOutOfRangeException();
            
            var gameFields = level.GameFields[gameFieldIndex];
            var gameField = gameFields.GameFieldControllers[Random.Range(0, gameFields.GameFieldControllers.Length)];
            return gameField;
        }
        

        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if(!PhotonNetwork.IsMasterClient) return;
            
            var newGameObject = PhotonNetwork.Instantiate(levelControllerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeLevelController), RpcTarget.AllBuffered, newGameObject.GetComponent<PhotonView>().ViewID);
        }

        [PunRPC]
        private void InitializeLevelController(int viewID)
        {
            var newGameObject = PhotonView.Find(viewID).gameObject;
            levelController = newGameObject.GetComponent<LevelController>();
            levelController.Initialize();
        }
        
        public void StartLevel()
        {
            _ = InitializeLevel();
        }

        private async UniTask InitializeLevel()
        {
            await UniTask.WaitUntil(() => levelController != null);
            
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(CreateCamera), RpcTarget.AllBuffered);
                CreateLevel();
            }

            CreatePlayer();
        }

        public void RestartLevel()
        {
            // Реализация перезапуска уровня
        }

        public void StopLevel()
        {
            // Реализация остановки уровня
        }

        [PunRPC]
        private void CreateCamera()
        {
            Instantiate(cameraPrefab);
        }

        private void CreateLevel()
        {
            var gameField = GetGameFieldReference(currentLevelIndex, currentGameFieldIndex);
            var newGameField = PhotonNetwork.Instantiate(gameField.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeGameField), RpcTarget.AllBuffered,
                newGameField.GetComponent<PhotonView>().ViewID);
        }
        [PunRPC]
        private void InitializeGameField(int viewID)
        {
            var targetGameObject = PhotonView.Find(viewID).gameObject;
            targetGameObject.transform.SetParent(levelController.transform);
            var gameFieldController = targetGameObject.GetComponent<GameFieldController>();
            Debug.Log(diContainer);
            diContainer.Inject(gameFieldController);
            gameFieldController.Initialize();
            gameFieldController.StartGame();
            levelController.SetGameField(gameFieldController);
        }

        private void CreatePlayer()
        {
            var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.AssetGUID,
                levelController.CurrentGameField.PlayerSpawnPoint.position,
                levelController.CurrentGameField.PlayerSpawnPoint.rotation);
            
            photonView.RPC(nameof(InitializePlayer), RpcTarget.AllBuffered, playerGameObject.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializePlayer(int viewID)
        {
            var playerGameObject = PhotonView.Find(viewID).gameObject;
            playerController = playerGameObject.GetComponent<PlayerController>();
            diContainer.Inject(playerController);
            diContainer.Bind<PlayerController>().FromInstance(playerController);
            playerController.GetComponent<CharacterController>().enabled = false;
            playerController.Initialize();
            playerController.GetComponent<CharacterController>().enabled = true;

            gameUnits.AddUnits(playerController.gameObject);
            playerController.Appear();
        }
    }
}
