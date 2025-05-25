using Zenject;

namespace Gameplay.Installer
{
    public class GameSystemInstaller
    {
        public void Install(DiContainer diContainer)
        {
            var gameUnits = new GameUnits();
            diContainer.Inject(gameUnits);
            diContainer.Bind<GameUnits>().FromInstance(gameUnits).AsSingle();
        
            var experienceSystem = new ExperienceSystem();
            diContainer.Inject(experienceSystem);
            diContainer.Bind<ExperienceSystem>().FromInstance(experienceSystem).AsSingle();
        }
    }
}