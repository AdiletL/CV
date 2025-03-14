using Gameplay.UI.ScreenSpace.Stats;
using Photon.Pun;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Gameplay.UI.ScreenSpace.CreatureInformation
{
    public class UIUnitInformation : UIInformation
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private UIStats stats;
        
        [Space(10)]
        [SerializeField] private TextMeshProUGUI descriptionTxt;

        [Space(10)] 
        [SerializeField] private GameObject parent;
        
        
        public override void Initialize()
        {
            diContainer.Inject(stats);
            stats.Initialize();
            ClearInformation();
            Hide();
        }

        public void ClearInformation()
        {
            stats.ClearStats();
            descriptionTxt.text = "";
        }

        public void Show()
        {
            parent.SetActive(true);
        }
        public void Hide()
        {
            parent.SetActive(false);
        }
        
    }
}