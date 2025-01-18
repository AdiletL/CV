using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private AssetReference gameInstallerPrefab;
    [SerializeField] private AssetReference gameManagerPrefab;
    [SerializeField] private AssetReference laboratoryManagerPrefab;

    private GameInstaller currentGameInstaller;

    public async UniTask Initialize()
    {
        var installerPrefab = await Addressables.LoadAssetAsync<GameObject>(gameInstallerPrefab).Task;
        currentGameInstaller = Instantiate(installerPrefab).GetComponent<GameInstaller>();
    }

    public async UniTask TransitionToScene(string sceneName)
    {
        await CreateManagerAsync(sceneName);
        Scenes.TransitionToScene(sceneName);
    }

    private async UniTask CreateManagerAsync(string sceneName)
    {
        switch (sceneName)
        {
            case Scenes.GAMEPLAY_NAME:
                await InstantiateManagerAsync<Gameplay.Manager.GameManager>(gameManagerPrefab);
                break;
            case Scenes.LABORATORY_NAME:
                await InstantiateManagerAsync<Laboratory.Manager.LaboratoryManager>(laboratoryManagerPrefab);
                break;
            default:
                throw new ArgumentException($"Unknown scene name: {sceneName}");
        }
    }

    private async UniTask<T> InstantiateManagerAsync<T>(AssetReference prefabReference) where T : MonoBehaviour
    {
        var prefab = await Addressables.LoadAssetAsync<GameObject>(prefabReference).Task;
        var instance = currentGameInstaller.InstantiatePrefab(prefab).GetComponent<T>();
        return instance;
    }
}