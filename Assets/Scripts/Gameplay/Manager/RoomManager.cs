using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private LevelController levelController;
        
        private List<RoomController> currentRooms = new();
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
            RoomController.OnSpawnNextRooms += SpawnNextRoom;
        }

        private void OnDisable()
        {
            RoomController.OnSpawnNextRooms -= SpawnNextRoom;
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

        public void SpawnStartRoom(int levelIndex)
        {
            var roomConfig = GetStartRoom(levelIndex);
            var randomRoom = roomConfig.RoomObjects[Random.Range(0, roomConfig.RoomObjects.Length)];
            
            var roomGameObject = PhotonNetwork.Instantiate(randomRoom.AssetGUID, Vector3.zero, Quaternion.identity);
            roomGameObject.transform.SetParent(levelController.transform);
            roomGameObject.transform.localPosition = Vector3.zero;
            
            var roomController = roomGameObject.GetComponent<RoomController>();
            diContainer.Inject(roomController);
            roomController.SetID(roomConfig.ID);
            roomController.Initialize();
            PlayerSpawnPosition = roomController.PlayerSpawnPoint.position;
            currentRooms.Add(roomController);

            BuildNavMesh(roomGameObject);

            var endPositions = new List<Vector3>(roomController.EndPoints.Length);
            for (int i = 0; i < endPositions.Count; i++)
                endPositions.Add(roomController.EndPoints[i].position);
            
            SpawnNextRoom(roomConfig.ID, endPositions);
        }
        
        private void SpawnNextRoom(int id, List<Vector3> endPositions)
        {
            StartCoroutine(SpawnNextRoomCoroutine(id, endPositions));
        }

        private IEnumerator SpawnNextRoomCoroutine(int id, List<Vector3> endPositions)
        {
            yield return new WaitForEndOfFrame();
            
            var roomConfig = GetRoom(LevelManager.CurrentLevelIndex, id);
            var nextRoomsConfig = roomConfig.NextRooms;
            SO_Room nextRoom = null;
            GameObject roomGameObject = null;

            for (int i = 0; i < endPositions.Count; i++)
            {
                if (nextRoomsConfig.Length - 1 < i) yield break;
                
                nextRoom = nextRoomsConfig[i];
                var randomRoom = roomConfig.RoomObjects[Random.Range(0, nextRoom.RoomObjects.Length)];
                
                roomGameObject = PhotonNetwork.Instantiate(randomRoom.AssetGUID, Vector3.zero, Quaternion.identity);
                var roomController = roomGameObject.GetComponent<RoomController>();
                roomGameObject.transform.localPosition = endPositions[i] - roomController.StartPoint.localPosition;
                roomGameObject.transform.SetParent(levelController.transform);

                diContainer.Inject(roomController);
                roomController.SetID(nextRoom.ID);
                roomController.Initialize();
                currentRooms.Add(roomController);
                
                BuildNavMesh(roomGameObject);
                yield return null;
            }
        }

        private void BuildNavMesh(GameObject room)
        {
            navMeshManager.BuildNavMesh(room);
        }
    }
}