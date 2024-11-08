using UnityEngine;

namespace Gameplay.Experience
{
    public class ExponentialExperience : IExperience
    {
        public int CalculateExperienceForNextLevel(int currentLevel)
        {
            return (int)(100 * Mathf.Pow(1.5f, currentLevel - 1)); // Экспоненциальный рост опыта
        }
    }
}