using UnityEngine;

namespace Gameplay
{
    public class RangeDisplay : MonoBehaviour
    {
        public void SetRange(float scale) => transform.localScale = Vector3.one * (scale * 2);
        public void ShowRange() => gameObject.SetActive(true);
        public void HideRange() => gameObject.SetActive(false);
    }
}
