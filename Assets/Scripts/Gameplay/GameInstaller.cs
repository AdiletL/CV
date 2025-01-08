using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private SO_GameConfig so_GameConfig;
    [SerializeField] private SO_GameUIColor so_GameUIColor;

    public override void InstallBindings()
    {
        DontDestroyOnLoad(gameObject);
        
        Container.Bind<GameUnits>().AsSingle();
        Container.BindInterfacesAndSelfTo<ExperienceSystem>().AsSingle();
        Container.BindInstance(so_GameConfig).AsSingle();
        Container.BindInstance(so_GameUIColor).AsSingle();
    }

    public GameObject InstantiatePrefab(GameObject prefab)
    {
        var newObject = Container.InstantiatePrefab(prefab);
        Container.Inject(newObject);
        return newObject;
    }
}
