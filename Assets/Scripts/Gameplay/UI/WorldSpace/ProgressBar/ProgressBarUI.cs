using ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI
{
    public abstract class ProgressBarUI : MonoBehaviour
    {
        [Inject] protected SO_GameUIColor so_GameUIColor;
        
        [SerializeField] protected Image progressBar;
        
        protected Gradient gradient;
        protected float result;
        
        public abstract void Initialize();
        
        public virtual void UpdateHealthBar(float current, float max)
        {
            result = current/max;
            progressBar.fillAmount = result;
        }
        public void UpdateGradient() => progressBar.color = gradient.Evaluate(result);
        
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}