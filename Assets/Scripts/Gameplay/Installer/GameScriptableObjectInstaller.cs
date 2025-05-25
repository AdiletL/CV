using ScriptableObjects.Ability;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using Zenject;

namespace Gameplay.Installer
{
    public class GameScriptableObjectInstaller : MonoBehaviour
    {
        [SerializeField] private SO_GameConfig so_GameConfig;
        [SerializeField] private SO_GameHotkeys so_GameHotkeys;
        [SerializeField] private SO_GameUIColor so_GameUIColor;
        [SerializeField] private SO_GameStatIcon so_GameStatIcon;
        [SerializeField] private SO_GameDisable so_GameDisable;
        [SerializeField] private SO_GameRange so_GameRange;
        [SerializeField] private SO_AbilityContainer so_AbilityContainer;
        [SerializeField] private SO_ItemContainer so_ItemContainer;

        public void Install(DiContainer diContainer)
        {
            diContainer.Bind<SO_GameConfig>().FromInstance(so_GameConfig).AsSingle();
            diContainer.Bind<SO_GameHotkeys>().FromInstance(so_GameHotkeys).AsSingle();
            diContainer.Bind<SO_GameUIColor>().FromInstance(so_GameUIColor).AsSingle();
            diContainer.Bind<SO_GameStatIcon>().FromInstance(so_GameStatIcon).AsSingle();
            diContainer.Bind<SO_GameDisable>().FromInstance(so_GameDisable).AsSingle();
            diContainer.Bind<SO_GameRange>().FromInstance(so_GameRange).AsSingle();
            diContainer.Bind<SO_AbilityContainer>().FromInstance(so_AbilityContainer).AsSingle();
            diContainer.Bind<SO_ItemContainer>().FromInstance(so_ItemContainer).AsSingle();
        }
    }
}