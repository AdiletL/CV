namespace Gameplay.Experience
{
    public class LinearExperience : IExperience
    {
        public int CalculateExperienceForNextLevel(int currentLevel)
        {
            return currentLevel * 100;
        }
    }
}