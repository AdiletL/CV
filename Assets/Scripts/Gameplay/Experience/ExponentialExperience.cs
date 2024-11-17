using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Gameplay.Experience
{
    public class ExponentialExperience : IExperience
    {
        private SO_GameConfig so_GameConfig;
        
        [Inject]
        public void Contruct(SO_GameConfig so_GameConfig)
        {
            this.so_GameConfig = so_GameConfig;
        }
        
        public int CalculateExperienceForNextLevel(int currentLevel)
        {
            return (int)(so_GameConfig.Experience * Mathf.Pow(1.5f, currentLevel - 1)); // Экспоненциальный рост опыта
        }
    }
}