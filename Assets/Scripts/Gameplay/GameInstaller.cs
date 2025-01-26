using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public GameObject InstantiatePrefab(AssetReference prefab)
    {
        if (!(PhotonNetwork.PrefabPool is AddressablesPrefabPool))
            PhotonNetwork.PrefabPool = new AddressablesPrefabPool();
        
        var newPrefab = PhotonNetwork.Instantiate(prefab.AssetGUID, Vector3.zero, Quaternion.identity);
        var comopnents = newPrefab.GetComponentsInChildren<MonoBehaviour>();
        foreach (var comopnent in comopnents)
            Container.Inject(comopnent);
        return newPrefab;
    }
}
