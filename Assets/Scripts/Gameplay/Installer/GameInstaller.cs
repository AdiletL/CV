using System.Collections;
using Gameplay.Installer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private AssetReferenceGameObject gameScriptableObjectInstallerPrefab;
    [SerializeField] private AssetReferenceGameObject gameManagerInstallerPrefab;

    public override void InstallBindings()
    {
        Container = new DiContainer();
    }

    public override void Start()
    {
        base.Start();
        Binding();
    }

    private void Binding()
    {
        var loadGameScriptableObjectInstaller = Addressables.LoadAssetAsync<GameObject>(gameScriptableObjectInstallerPrefab).WaitForCompletion();
        var gameScriptableObjectInstallerObject = Container.InstantiatePrefab(loadGameScriptableObjectInstaller);
        gameScriptableObjectInstallerObject.GetComponent<GameScriptableObjectInstaller>().Install(Container);

        var gameSystemInstaller = new GameSystemInstaller();
        gameSystemInstaller.Install(Container);
        
        var gameFactoryInstaller = new GameFactoryInstaller();
        gameFactoryInstaller.Install(Container);
        
        var loadGameManagerInstaller = Addressables.LoadAssetAsync<GameObject>(gameManagerInstallerPrefab).WaitForCompletion();
        var gameManagerInstallerObject = Container.InstantiatePrefab(loadGameManagerInstaller);
        gameManagerInstallerObject.GetComponent<GameManagerInstaller>().Install(Container);
    }
}
