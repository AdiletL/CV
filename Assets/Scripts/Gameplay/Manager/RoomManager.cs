using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay.Manager
{
    public class RoomManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        [Inject] private NavMeshManager navMeshManager;
        
        [SerializeField] private SO_LevelContainer so_LevelContainer;
        [SerializeField] private AssetReferenceGameObject levelControllerPrefab;
        
        private PhotonView photonView;
        private LevelController currentLevelController;
        private List<LevelController> levels = new ();
        
        public Vector3 PlayerSpawnPosition { get; private set; }
        
        private SO_Room GetStartRoom(int levelIndex)
        {
            return so_LevelContainer.Levels[levelIndex].StartRoom;
        }

        private SO_Room GetRoom(int levelIndex, int id)
        {
            foreach (var VARIABLE in so_LevelContainer.Levels[levelIndex].Rooms)
            {
                if (VARIABLE.ID == id)
                    return VARIABLE;
            }
            
            throw new NullReferenceException();
        }

        private void OnEnable()
        {
            RoomController.OnTriggerSpawnNextRoom += SpawnNextRoom;
        }

        private void OnDisable()
        {
            RoomController.OnTriggerSpawnNextRoom -= SpawnNextRoom;
        }

        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if(!PhotonNetwork.IsMasterClient) return;
            
        }
        
        private void InitializeLevelController(int levelID)
        {
            if (currentLevelController && levelID != currentLevelController.ID)
            {
                for (int i = levels.Count - 1; i >= 0; i--)
                {
                    if (levels[i].ID == levelID)
                    {
                        currentLevelController = levels[i];
                        return;
                    }
                }
            }

            var newGameObject = Addressables.InstantiateAsync(levelControllerPrefab).WaitForCompletion();
            var newLevel = newGameObject.GetComponent<LevelController>();
            diContainer.Inject(newLevel);
            newLevel.SetID(levelID);
            newLevel.Initialize();
            currentLevelController = newLevel;
            levels.Add(newLevel);
        }

        public void SpawnStartRoom(int levelID)
        {
            InitializeLevelController(levelID);
            
            var roomConfig = GetStartRoom(levelID);
            var randomRoom = roomConfig.RoomObjects[Random.Range(0, roomConfig.RoomObjects.Length)];
            
            var roomGameObject = PhotonNetwork.Instantiate(randomRoom.AssetGUID, Vector3.zero, Quaternion.identity);
            roomGameObject.transform.SetParent(currentLevelController.transform);
            roomGameObject.transform.localPosition = Vector3.zero;
            
            var roomController = roomGameObject.GetComponent<RoomController>();
            diContainer.Inject(roomController);
            roomController.SetID(roomConfig.ID);
            roomController.Initialize();
            PlayerSpawnPosition = roomController.PlayerSpawnPoint.position;
            currentLevelController.AddRoom(roomController);

            BuildNavMesh(roomGameObject);
        }
        
        private void SpawnNextRoom(int id)
        {
            if(!currentLevelController.IsNullRoom(id)) return;
            StartCoroutine(SpawnNextRoomCoroutine(currentLevelController, id));
        }

        private IEnumerator SpawnNextRoomCoroutine(LevelController currentLevelController, int id)
        {
            yield return new WaitForEndOfFrame();
            
            var roomConfig = GetRoom(LevelManager.CurrentLevelIndex, id);
            var randomRoom = roomConfig.RoomObjects[Random.Range(0, roomConfig.RoomObjects.Length)];
            
            var roomGameObject = PhotonNetwork.Instantiate(randomRoom.AssetGUID, Vector3.zero, Quaternion.identity);
            var roomController = roomGameObject.GetComponent<RoomController>();
            roomGameObject.transform.SetParent(currentLevelController.transform);

            diContainer.Inject(roomController);
            roomController.SetID(roomConfig.ID);
            roomController.Initialize();
            currentLevelController.AddRoom(roomController);
            
            BuildNavMesh(roomGameObject);
            yield return null;
        }

        private void BuildNavMesh(GameObject room)
        {
            navMeshManager.BuildNavMesh(room);
        }
    }
}