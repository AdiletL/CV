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
        
        public int CalculateExperienceForNextLevel(int level, int experience)
        {
            return (int)(experience * Mathf.Pow(1.5f, level - 1)); // Экспоненциальный рост опыта
        }
    }
}