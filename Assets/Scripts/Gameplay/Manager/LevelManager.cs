﻿using Cysharp.Threading.Tasks;
using Gameplay.Unit.Character.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class LevelManager : MonoBehaviourPun, IManager
    {
        [Inject] private DiContainer diContainer;
        [Inject] private GameUnits gameUnits;
        [Inject] private RoomManager roomManager;

        [SerializeField] private AssetReferenceGameObject playerPrefab;
        [SerializeField] private GameObject cameraPrefab;

        private PlayerController playerController;
        
        public static int CurrentLevelIndex { get; private set; }
        

        public void Initialize()
        {
            
        }
        
        public void StartLevel()
        {
            CreateCamera();
            roomManager.SpawnStartRoom(CurrentLevelIndex);

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

        private void CreateCamera()
        {
            var cameraObject = Instantiate(cameraPrefab);
            cameraObject.GetComponent<CameraController>().Initialize();
        }

        private void CreatePlayer()
        {
            var playerGameObject = PhotonNetwork.Instantiate(playerPrefab.AssetGUID,
                roomManager.PlayerSpawnPosition, Quaternion.identity);

            InitializePlayer(playerGameObject.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializePlayer(int viewID)
        {
            var playerGameObject = PhotonView.Find(viewID).gameObject;
            playerController = playerGameObject.GetComponent<PlayerController>();
            diContainer.Inject(playerController);
            diContainer.Bind<PlayerController>().FromInstance(playerController);
            gameUnits.AddUnits(playerController.gameObject);
            
            playerController.Initialize();

            playerController.Activate();
            playerController.Appear();
        }
    }
}
