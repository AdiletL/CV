using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public abstract class DamagePopUpUI : PopUpUI
    {
        protected override float[] CreateRandomValuesForPositionX()
        {
            return new float[] { -0.15f, -0.1f, -0.05f, 0.05f, 0.1f, 0.15f };
        }
    }
}
