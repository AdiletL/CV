using Gameplay.Manager;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private SO_GameConfig so_GameConfig;
    [SerializeField] private SO_GameUIColor so_GameUIColor;

    public override void InstallBindings()
    {
        Container.Bind<GameUnits>().AsSingle();
        Container.BindInterfacesAndSelfTo<ExperienceSystem>().AsSingle();
        Container.BindInstance(so_GameConfig).AsSingle();
        Container.BindInstance(so_GameUIColor).AsSingle();
    }
}
