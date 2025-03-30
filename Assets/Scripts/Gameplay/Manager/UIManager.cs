using Cysharp.Threading.Tasks;
using Gameplay.Spawner;
using Gameplay.UI.ScreenSpace;
using Gameplay.UI.ScreenSpace.ContextMenu;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class UIManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private AssetReferenceGameObject mainCanvasPrefab;
        [SerializeField] private AssetReferenceGameObject uiCastTimerPrefab;
        [SerializeField] private AssetReferenceGameObject uiContextMenuPrefab;
        
        [Space]
        [SerializeField] private AssetReferenceGameObject[] popUpSpawnerPrefabs;
        
        private GameObject mainCanvas;
        private PhotonView photonView;
        
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if(!PhotonNetwork.IsMasterClient) return;

            foreach (var popUpSpawnerPrefab in popUpSpawnerPrefabs)
            {
                var popUp =
                    PhotonNetwork.Instantiate(popUpSpawnerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                photonView.RPC(nameof(InstantiatePopUpSpawner), RpcTarget.AllBuffered, popUp.GetComponent<PhotonView>().ViewID);
            }
            
            var newMainCanvas = PhotonNetwork.Instantiate(mainCanvasPrefab.AssetGUID, Vector3.zero,
                Quaternion.identity);
            var newUICastTimer = PhotonNetwork.Instantiate(uiCastTimerPrefab.AssetGUID, Vector3.zero,
                Quaternion.identity);
            var newUIContextMenu = PhotonNetwork.Instantiate(uiContextMenuPrefab.AssetGUID, Vector3.zero,
                Quaternion.identity);

            photonView.RPC(nameof(InitializeMainCanvas), RpcTarget.AllBuffered, newMainCanvas.GetComponent<PhotonView>().ViewID);
            photonView.RPC(nameof(InitializeUICastTimer), RpcTarget.AllBuffered, newUICastTimer.GetComponent<PhotonView>().ViewID);
            photonView.RPC(nameof(InitializeUIContextMenu), RpcTarget.AllBuffered, newUIContextMenu.GetComponent<PhotonView>().ViewID);
            
        }

        [PunRPC]
        private void InitializeMainCanvas(int viewID)
        {
            mainCanvas = PhotonView.Find(viewID).gameObject;
        }
        
        [PunRPC]
        private void InstantiatePopUpSpawner(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            var popUpPopUpSpawner = result.GetComponent<PopUpSpawner>();
            diContainer.Inject(popUpPopUpSpawner);
            diContainer.Bind(popUpPopUpSpawner.GetType()).FromInstance(popUpPopUpSpawner).AsSingle();
            popUpPopUpSpawner.transform.SetParent(transform);
            popUpPopUpSpawner.Initialize();
        }

        [PunRPC]
        private void InitializeUICastTimer(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            var uiCastTimer = result.GetComponent<UICastTimer>();
            diContainer.Inject(uiCastTimer);
            diContainer.Bind(uiCastTimer.GetType()).FromInstance(uiCastTimer).AsSingle();
            uiCastTimer.Initialize();
        }
        
        [PunRPC]
        private void InitializeUIContextMenu(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            var uiContextMenu = result.GetComponent<UIContextMenu>();
            diContainer.Inject(uiContextMenu);
            diContainer.Bind(uiContextMenu.GetType()).FromInstance(uiContextMenu).AsSingle();
            result.transform.SetParent(mainCanvas.transform);
        }
    }
}