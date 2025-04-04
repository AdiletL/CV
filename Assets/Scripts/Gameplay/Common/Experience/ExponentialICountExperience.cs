using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Gameplay.Experience
{
    public class ExponentialICountExperience : ICountExperience
    {
        [Inject] private SO_GameConfig so_GameConfig;

        public int CalculateExperienceForNextLevel(int level, int experience)
        {
            return (int)(experience * Mathf.Pow(1.5f, level - 1)); // Экспоненциальный рост опыта
        }
    }
}