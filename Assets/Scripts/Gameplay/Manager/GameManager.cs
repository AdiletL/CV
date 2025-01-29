using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Gameplay.Skill;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class GameManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;

        [SerializeField] private AssetReferenceGameObject poolManagerPrefab;
        [SerializeField] private AssetReferenceGameObject levelManagerPrefab;
        [SerializeField] private AssetReferenceGameObject uiManagerPrefab;
        [SerializeField] private SO_SkillContainer so_SkillContainerPrefab;
        [SerializeField] private SO_GameConfig so_GameConfigPrefab;
        
        private LevelManager levelManager;
        private PoolManager poolManager;
        private UIManager uiManager;
        private GameUnits gameUnits;
        private ExperienceSystem experienceSystem;
        
        private PhotonView photonView;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // Подписываемся на событие загрузки сцены
        }
        
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if (!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(LoadAndBindAsset), RpcTarget.AllBuffered);
        }
        
        [PunRPC]
        private void LoadAndBindAsset()
        {
            Debug.Log(diContainer);
            Debug.Log("1");
            
            diContainer.Bind<SO_SkillContainer>().FromInstance(so_SkillContainerPrefab).AsSingle();
            diContainer.Bind<SO_GameConfig>().FromInstance(so_GameConfigPrefab).AsSingle();
            diContainer.Bind<SO_GameHotkeys>().FromInstance(so_GameConfigPrefab.SO_GameHotkeys).AsSingle();
            diContainer.Bind<SO_GameUIColor>().FromInstance(so_GameConfigPrefab.SO_GameUIColor).AsSingle();
            
            if(!PhotonNetwork.IsMasterClient) return;
            photonView.RPC(nameof(InitializeGameUnits), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void InitializeGameUnits()
        {
            Debug.Log("3");
            gameUnits = new GameUnits();
            diContainer.Inject(gameUnits);
            diContainer.Bind<GameUnits>().FromInstance(gameUnits).AsSingle();
            
            experienceSystem = new ExperienceSystem();
            diContainer.Inject(experienceSystem);
            diContainer.Bind<ExperienceSystem>().FromInstance(experienceSystem).AsSingle();

            InstantiatePoolManager();
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
            Debug.Log("4");
            var result = PhotonView.Find(viewID).gameObject;
            poolManager = result.GetComponent<PoolManager>();
            diContainer.Inject(poolManager);
            diContainer.Bind(poolManager.GetType()).FromInstance(poolManager).AsSingle();
            poolManager.transform.SetParent(transform);
            poolManager.Initialize();
            InstantiateUIManager();
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
            Debug.Log("5");
            var result = PhotonView.Find(viewID).gameObject;
            uiManager = result.GetComponent<UIManager>();
            diContainer.Inject(uiManager);
            diContainer.Bind(uiManager.GetType()).FromInstance(uiManager).AsSingle();
            uiManager.transform.SetParent(transform);
            uiManager.Initialize();
            InstantiateLevelManager();
        }

        private void InstantiateLevelManager()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            var result = PhotonNetwork.Instantiate(levelManagerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            photonView.RPC(nameof(InitializeLevelManager), RpcTarget.AllBuffered, result.GetComponent<PhotonView>().ViewID);
        }
        
        [PunRPC]
        private void InitializeLevelManager(int viewID)
        {
            Debug.Log("6");
            var result = PhotonView.Find(viewID).gameObject;
            levelManager = result.GetComponent<LevelManager>();
            diContainer.Inject(levelManager);
            diContainer.Bind(levelManager.GetType()).FromInstance(levelManager).AsSingle();
            levelManager.transform.SetParent(transform);
            levelManager.Initialize();

            StartLevel();
        }

        private void StartLevel()
        {
            levelManager.StartLevel();
        }
    }
}
