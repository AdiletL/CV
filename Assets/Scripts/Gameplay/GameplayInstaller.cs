using Gameplay.Manager;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private SO_GameConfig so_GameConfig;
    [SerializeField] private SO_GameUIColor so_GameUIColor;
    [SerializeField] private SpawnerManager spawnerManager;
    [SerializeField] private PoolManager poolManager;

    [Space(10)] [SerializeField] private bool isStartGame;
    
    public override void InstallBindings()
    {
        Container.Bind<GameUnits>().AsSingle();
        Container.BindInterfacesAndSelfTo<ExperienceSystem>().AsSingle();
        Container.BindInstance(so_GameConfig).AsSingle();
        Container.BindInstance(so_GameUIColor).AsSingle();
    }

    private void Awake()
    {
        if(!isStartGame) return;

        var poolManager = Container.InstantiatePrefabForComponent<PoolManager>(this.poolManager, transform);
        poolManager.Initialize();
        Container.Bind<IPoolable>().FromInstance(poolManager).AsSingle();
        
        var spawnManager = Container.InstantiatePrefabForComponent<SpawnerManager>(spawnerManager, transform);
        spawnManager.Initialize();
    }
}
