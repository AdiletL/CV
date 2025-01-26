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
        
        public async UniTask Initialize()
        {
            await UniTask.WhenAll(
                InitializeUICreatureInformation(),
                InstantiateDamagePopUpSpawner()
                );
        }

        private async UniTask<TInterface> InstantiateAndBind<TInterface, TConcrete>(AssetReference prefab)
            where TInterface : class
            where TConcrete : MonoBehaviour, TInterface
        {
            var result = PhotonNetwork.Instantiate(prefab.AssetGUID, Vector3.zero, Quaternion.identity);
            var instance = result.GetComponent<TConcrete>();
            diContainer.Inject(instance);
            diContainer.Bind<TInterface>().FromInstance(instance).AsSingle();
            result.transform.SetParent(transform);
            return instance;
        }
        
        private async UniTask InitializeUICreatureInformation()
        {
            uiCreatureInformation = await InstantiateAndBind<UICreatureInformation, UICreatureInformation>(uiCreatureInformationPrefab);
            uiCreatureInformation.Initialize();
        }
        
        private async UniTask InstantiateDamagePopUpSpawner()
        {
            damagePopUpSpawner = await InstantiateAndBind<DamagePopUpSpawner, DamagePopUpSpawner>(damagePopUpSpawnerPrefab);
            await damagePopUpSpawner.Initialize();
        }
    }
}