using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public GameObject InstantiatePrefab(GameObject prefab)
    {
        var newObject = Container.InstantiatePrefab(prefab);
        Container.Inject(newObject);
        return newObject;
    }
}
