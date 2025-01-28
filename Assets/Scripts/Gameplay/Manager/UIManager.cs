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
        [SerializeField] private AssetReferenceGameObject damagePopUpSpawnerPrefab;
        
        private UICreatureInformation uiCreatureInformation;
        private DamagePopUpSpawner damagePopUpSpawner;

        private PhotonView photonView;
        
        public void Initialize()
        {
            photonView = GetComponent<PhotonView>();
            
            if(!PhotonNetwork.IsMasterClient) return;

            var newUICreature = PhotonNetwork.Instantiate(uiCreatureInformationPrefab.AssetGUID, Vector3.zero,
                Quaternion.identity);

            var newDamage =
                PhotonNetwork.Instantiate(damagePopUpSpawnerPrefab.AssetGUID, Vector3.zero, Quaternion.identity);
            
            photonView.RPC(nameof(InitializeUICreatureInformation), RpcTarget.AllBuffered, newUICreature.GetComponent<PhotonView>().ViewID);
            photonView.RPC(nameof(InstantiateDamagePopUpSpawner), RpcTarget.AllBuffered, newDamage.GetComponent<PhotonView>().ViewID);
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
        private void InstantiateDamagePopUpSpawner(int viewID)
        {
            var result = PhotonView.Find(viewID).gameObject;
            damagePopUpSpawner = result.GetComponent<DamagePopUpSpawner>();
            diContainer.Inject(damagePopUpSpawner);
            diContainer.Bind(damagePopUpSpawner.GetType()).FromInstance(damagePopUpSpawner).AsSingle();
            damagePopUpSpawner.transform.SetParent(transform);
            damagePopUpSpawner.Initialize();
        }
    }
}