namespace Gameplay.UI
{
    public class HealthBarUI : ProgressBarUI
    {
        
        public override void Initialize()
        {
            gradient = so_GameUIColor.HealthBarGradient;
        }
    }
}