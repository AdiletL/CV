namespace Gameplay.UI
{
    public class EnduranceBarUI : ProgressBarUI
    {
        public override void Initialize()
        {
            gradient = so_GameUIColor.EnduranceBarGradient;
        }
    }
}