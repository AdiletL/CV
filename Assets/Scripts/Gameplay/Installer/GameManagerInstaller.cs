using Gameplay.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Installer
{
    public class GameManagerInstaller : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject poolManagerPrefab;
        [SerializeField] private AssetReferenceGameObject levelManagerPrefab;
        [SerializeField] private AssetReferenceGameObject uiManagerPrefab;
        [SerializeField] private AssetReferenceGameObject navMeshManagerPrefab;
        [SerializeField] private AssetReferenceGameObject roomManagerPrefab;
        [SerializeField] private AssetReferenceGameObject networkManagerPrefab;
        [SerializeField] private AssetReferenceGameObject gameManagerPrefab;

        public void Install(DiContainer diContainer)
        {
            var loadNetworkManager = Addressables.LoadAssetAsync<GameObject>(networkManagerPrefab).WaitForCompletion();
            var networkManagerObject = diContainer.InstantiatePrefab(loadNetworkManager);
            var networkManager = networkManagerObject.GetComponent<NetworkManager>();
            diContainer.Bind(networkManager.GetType()).FromInstance(networkManager).AsSingle();
            networkManager.Initialize();
            
            var loadPoolManager = Addressables.LoadAssetAsync<GameObject>(poolManagerPrefab).WaitForCompletion();
            var poolManagerObject = diContainer.InstantiatePrefab(loadPoolManager);
            var poolManager = poolManagerObject.GetComponent<IPoolableObject>();
            diContainer.Bind(poolManager.GetType()).FromInstance(poolManager).AsSingle();
            poolManager.Initialize();
            
            var loadUIManager = Addressables.LoadAssetAsync<GameObject>(uiManagerPrefab).WaitForCompletion();
            var uiManagerObject = diContainer.InstantiatePrefab(loadUIManager);
            var uiManager = uiManagerObject.GetComponent<UIManager>();
            diContainer.Bind(uiManager.GetType()).FromInstance(uiManager).AsSingle();
            uiManager.Initialize();
            
            var loadNavmeshManager = Addressables.LoadAssetAsync<GameObject>(navMeshManagerPrefab).WaitForCompletion();
            var navmeshManagerObject = diContainer.InstantiatePrefab(loadNavmeshManager);
            var navmeshManager = navmeshManagerObject.GetComponent<NavMeshManager>();
            diContainer.Bind(navmeshManager.GetType()).FromInstance(navmeshManager).AsSingle();
            navmeshManager.Initialize();
            
            var loadRoomManager = Addressables.LoadAssetAsync<GameObject>(roomManagerPrefab).WaitForCompletion();
            var roomManagerObject = diContainer.InstantiatePrefab(loadRoomManager);
            var roomManager = roomManagerObject.GetComponent<RoomManager>();
            diContainer.Bind(roomManager.GetType()).FromInstance(roomManager).AsSingle();
            roomManager.Initialize();
            
            var loadLevelManager = Addressables.LoadAssetAsync<GameObject>(levelManagerPrefab).WaitForCompletion();
            var levelManagerObject = diContainer.InstantiatePrefab(loadLevelManager);
            var levelManager = levelManagerObject.GetComponent<LevelManager>();
            diContainer.Bind(levelManager.GetType()).FromInstance(levelManager).AsSingle();
            levelManager.Initialize();
            
            var loadGameManager = Addressables.LoadAssetAsync<GameObject>(gameManagerPrefab).WaitForCompletion();
            var loadGameManagerObject = diContainer.InstantiatePrefab(loadGameManager);
            var gameManager = loadGameManagerObject.GetComponent<Manager.GameManager>();
            gameManager.Initialize();
        }
    }
}