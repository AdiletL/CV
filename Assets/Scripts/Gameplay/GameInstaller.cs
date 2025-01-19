using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        DontDestroyOnLoad(gameObject);
        
        Container.Bind<GameUnits>().AsSingle();
        Container.BindInterfacesAndSelfTo<ExperienceSystem>().AsSingle();
    }

    public GameObject InstantiatePrefab(GameObject prefab)
    {
        var newObject = Container.InstantiatePrefab(prefab);
        Container.Inject(newObject);
        return newObject;
    }
}
