using Photon.Pun;
using TMPro;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.ScreenSpace.CreatureInformation
{
    public class UICreatureInformation : UIInformation
    {
        [Inject] private DiContainer diContainer;
        
        [SerializeField] private UICreatureStats creatureStats;
        
        [Space(10)]
        [SerializeField] private TextMeshProUGUI descriptionTxt;

        [Space(10)] 
        [SerializeField] private GameObject parent;
        
        
        public override void Initialize()
        {
            diContainer.Inject(creatureStats);
            creatureStats.Initialize();
            ClearInformation();
            Hide();
        }

        public void ClearInformation()
        {
            creatureStats.ClearStats();
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
        
        public void SetIcon(Sprite sprite) => creatureStats.SetIcon(sprite);
        public void SetHealth(int current, int max) => creatureStats.SetHealth(current, max);
        public void SetEndurance(float current, int max) => creatureStats.SetEndurance(current, max);
        public void AddText(string name) => creatureStats.AddText(name);
        public void SetDescription(string description) => descriptionTxt.text = description;
        public void Build()
        { 
            creatureStats.Build();
        }
    }
}