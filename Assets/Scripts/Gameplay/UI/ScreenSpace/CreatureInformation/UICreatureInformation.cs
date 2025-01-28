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
        
        public void SetName(string name) => creatureStats.SetName(name);
        
        public void SetType(EntityType type) => creatureStats.SetType(type);

        public void SetLevel(int level) => creatureStats.SetLevel(level);
        
        public void SetExperience(int value) => creatureStats.SetExperience(value);
        
        public void SetAmount(int value) => creatureStats.SetAmount(value);
        
        public void SetDamage(int damage) => creatureStats.SetDamage(damage);

        public void SetAttackSpeed(int value) => creatureStats.SetAttackSpeed(value);
        
        public void SetAttackRange(int value) => creatureStats.SetAttackRange(value);
        
        public void SetDescription(string description) => descriptionTxt.text = description;
    }
}