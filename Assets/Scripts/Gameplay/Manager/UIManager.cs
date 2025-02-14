using Cysharp.Threading.Tasks;
using Gameplay.Spawner;
using Gameplay.UI.ScreenSpace.CreatureInformation;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Gameplay.Manager
{
    public class UIManager : MonoBehaviour, IManager
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private AssetReference uiCreatureInformationPrefab;
        [SerializeField] private AssetReferenceGameObject[] popUpSpawnerPrefabs;
        
        private UICreatureInformation uiCreatureInformation;

        private PhotonView photonView;
        
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if(!PhotonNetwork.IsMasterClient) return;

            var newUICreature = PhotonNetwork.Instantiate(uiCreatureInformationPrefab.AssetGUID, Vector3.zero,
                Quaternion.identity);

            foreach (var popUpSpawnerPrefab in popUpSpawnerPrefabs)
            {
                var popUp =
                    PhotonNetwork.Instantiate(popUpSpawnerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
                photonView.RPC(nameof(InstantiatePopUpSpawner), RpcTarget.AllBuffered, popUp.GetComponent<PhotonView>().ViewID);
            }
            
            photonView.RPC(nameof(InitializeUICreatureInformation), RpcTarget.AllBuffered, newUICreature.GetComponent<PhotonView>().ViewID);
        }

        
        [PunRPC]
        private void InitializeUICreatureInformation(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            uiCreatureInformation = result.GetComponent<UICreatureInformation>();
            diContainer.Inject(uiCreatureInformation);
            diContainer.Bind(uiCreatureInformation.GetType()).FromInstance(uiCreatureInformation).AsSingle();
            uiCreatureInformation.transform.SetParent(transform);
            uiCreatureInformation.Initialize();
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
    }
}