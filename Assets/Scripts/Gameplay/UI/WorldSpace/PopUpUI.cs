using Gameplay.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.UI
{
    public abstract class PopUpUI : MonoBehaviour
    {
        [Inject] protected PoolManager poolManager;
        
        [SerializeField] protected TextMeshProUGUI valueText;
        [SerializeField] protected AnimationCurve alphaCurve;
        [SerializeField] protected float animationDuration = 1.0f;

        protected Color textColor;
        protected bool isAnimating;

        public abstract void Play(int amount);
        
    }
}