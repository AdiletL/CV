namespace Gameplay.UI
{
    public abstract class DamageResistancePopUpUI : PopUpUI
    {
        protected virtual float[] CreateRandomValuesForPosition()
        {
            return new float[] { -0.3f, -0.25f, -0.2f, 0.2f, 0.25f, 0.3f };
        }
    }
}