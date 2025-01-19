using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.UI
{
    public abstract class HotkeyUI : MonoBehaviour
    {
        [SerializeField] protected GameObject parent;
        [SerializeField] protected TextMeshProUGUI keyTxt;
        
        public void SetText(string key) => keyTxt.text = key;

        public void Show() => parent.SetActive(true);
        public void Hide() => parent.SetActive(false);
    }
}