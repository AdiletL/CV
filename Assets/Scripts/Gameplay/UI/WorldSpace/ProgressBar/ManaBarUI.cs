namespace Gameplay.UI
{
    public class ManaBarUI : ProgressBarUI
    {
        public override void Initialize()
        {
            gradient = so_GameUIColor.ManaBarGradient;
        }
    }
}