using ScriptableObjects.Gameplay;
using Zenject;

namespace Gameplay.Experience
{
    public class LinearICountExperience : ICountExperience
    {
        private SO_GameConfig so_GameConfig;
        
        [Inject]
        public void Contruct(SO_GameConfig so_GameConfig)
        {
            this.so_GameConfig = so_GameConfig;
        }
        
        public int CalculateExperienceForNextLevel(int level, int experience)
        {
            return level * experience;
        }
    }
}