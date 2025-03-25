using Gameplay.Factory;
using Gameplay.Factory.Weapon;
using Photon.Pun;
using ScriptableObjects.Ability;
using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private AssetReferenceGameObject poolManagerPrefab;
        [SerializeField] private AssetReferenceGameObject levelManagerPrefab;
        [SerializeField] private AssetReferenceGameObject uiManagerPrefab;
        [SerializeField] private AssetReferenceGameObject navMeshManagerPrefab;
        [SerializeField] private AssetReferenceGameObject roomManagerPrefab;
        [SerializeField] private AssetReferenceGameObject networkManagerPrefab;
        [SerializeField] private SO_GameConfig so_GameConfig;
        [SerializeField] private SO_BaseAbilityConfigContainer so_BaseAbilityConfigContainer;
        
        private LevelManager levelManager;
        private PoolManager poolManager;
        private UIManager uiManager;
        private NavMeshManager navMeshManager;
        private RoomManager roomManager;
        private NetworkManager networkManager;
        
        private GameUnits gameUnits;
        private ExperienceSystem experienceSystem;
        
        private PhotonView photonView;
        
        
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(LoadAndBindAsset), RpcTarget.AllBuffered);
        }
        
        [PunRPC]
        private void LoadAndBindAsset()
        {
            diContainer.Bind<SO_GameConfig>().FromInstance(so_GameConfig).AsSingle();
            diContainer.Bind<SO_GameHotkeys>().FromInstance(so_GameConfig.SO_GameHotkeys).AsSingle();
            diContainer.Bind<SO_GameUIColor>().FromInstance(so_GameConfig.SO_GameUIColor).AsSingle();
            diContainer.Bind<SO_GameStatIcon>().FromInstance(so_GameConfig.SO_GameStatIcon).AsSingle();
            diContainer.Bind<SO_BaseAbilityConfigContainer>().FromInstance(so_BaseAbilityConfigContainer).AsSingle();
            
            if(!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(InitializeGameUnitsAndExperienceSystem), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void InitializeGameUnitsAndExperienceSystem()
        {
            gameUnits = new GameUnits();
            diContainer.Inject(gameUnits);
            diContainer.Bind<GameUnits>().FromInstance(gameUnits).AsSingle();
            
            experienceSystem = new ExperienceSystem();
            diContainer.Inject(experienceSystem);
            diContainer.Bind<ExperienceSystem>().FromInstance(experienceSystem).AsSingle();

            InitializeEquipmentFactory();
            InitializeInventoryItemFactory();
            InitializeAbilityFactory();
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            InstantiateNetworkManager();
            InstantiatePoolManager();
            InstantiateUIManager();
            InstantiateNavMeshManager();
            InstantiateRoomManager();
            InstantiateLevelManager();
        }
        
        private void InstantiateNetworkManager()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(networkManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeNetworkManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializeNetworkManager(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            networkManager = result.GetComponent<NetworkManager>();
            diContainer.Inject(networkManager);
            diContainer.Bind(networkManager.GetType()).FromInstance(networkManager).AsSingle();
            networkManager.transform.SetParent(transform);
            networkManager.Initialize();
        }
        
        private void InstantiatePoolManager()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(poolManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializePoolManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializePoolManager(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            poolManager = result.GetComponent<PoolManager>();
            diContainer.Inject(poolManager);
            diContainer.Bind(poolManager.GetType()).FromInstance(poolManager).AsSingle();
            poolManager.transform.SetParent(transform);
            poolManager.Initialize();
        }

        private void InstantiateUIManager()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(uiManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeUIManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializeUIManager(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            uiManager = result.GetComponent<UIManager>();
            diContainer.Inject(uiManager);
            diContainer.Bind(uiManager.GetType()).FromInstance(uiManager).AsSingle();
            uiManager.transform.SetParent(transform);
            uiManager.Initialize();
        }
        
        private void InstantiateNavMeshManager()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(navMeshManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeNavMeshManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializeNavMeshManager(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            navMeshManager = result.GetComponent<NavMeshManager>();
            diContainer.Inject(navMeshManager);
            diContainer.Bind(navMeshManager.GetType()).FromInstance(navMeshManager).AsSingle();
            navMeshManager.transform.SetParent(transform);
            navMeshManager.Initialize();
        }

        private void InstantiateRoomManager()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(roomManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeRoomManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }

        [PunRPC]
        private void InitializeRoomManager(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            roomManager = result.GetComponent<RoomManager>();
            diContainer.Inject(roomManager);
            diContainer.Bind(roomManager.GetType()).FromInstance(roomManager).AsSingle();
            roomManager.transform.SetParent(transform);
            roomManager.Initialize();
        }
        
        private void InstantiateLevelManager()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(levelManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeLevelManagerAndStartLevel), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializeLevelManagerAndStartLevel(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            levelManager = result.GetComponent<LevelManager>();
            diContainer.Inject(levelManager);
            diContainer.Bind(levelManager.GetType()).FromInstance(levelManager).AsSingle();
            levelManager.transform.SetParent(transform);
            levelManager.Initialize();
            StartLevel();
        }

        private void InitializeEquipmentFactory()
        {
            EquipmentFactory equipmentFactory = new EquipmentFactory();
            diContainer.Inject(equipmentFactory);
            diContainer.Bind(equipmentFactory.GetType()).FromInstance(equipmentFactory).AsSingle();
        }

        private void InitializeInventoryItemFactory()
        {
            InventoryItemFactory inventoryItemFactory = new InventoryItemFactory();
            diContainer.Inject(inventoryItemFactory);
            diContainer.Bind(inventoryItemFactory.GetType()).FromInstance(inventoryItemFactory).AsSingle();
        }

        private void InitializeAbilityFactory()
        {
            AbilityFactory abilityFactory = new AbilityFactory();
            diContainer.Inject(abilityFactory);
            diContainer.Bind(abilityFactory.GetType()).FromInstance(abilityFactory).AsSingle();
        }
        
        private void StartLevel()
        {
            levelManager.StartLevel();
        }
    }
}
