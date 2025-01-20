using ScriptableObjects.Gameplay;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI.ScreenSpace.CreatureInformation
{
    public class UICreatureStats : MonoBehaviour
    {
        [Inject] private SO_GameUIColor so_GameUIColor;

        [SerializeField] private Image icon;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image enduranceBar;
        [SerializeField] private TextMeshProUGUI healthTxt;
        [SerializeField] private TextMeshProUGUI enduranceTxt;
        
        [Space]
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI typeTxt;
        [SerializeField] private TextMeshProUGUI levelTxt;
        [SerializeField] private TextMeshProUGUI damageTxt;
        [SerializeField] private TextMeshProUGUI attackSpeedTxt;
        [SerializeField] private TextMeshProUGUI attackRangeTxt;
        [SerializeField] private TextMeshProUGUI experienceTxt;
        [SerializeField] private TextMeshProUGUI amountTxt;
        
        [Space] //TODO: Change on icon
        [SerializeField] private TextMeshProUGUI skillTxt;
        
        private Gradient healthBarGradient;
        private Gradient enduranceBarGradient;

        private const string HEALTH = "";
        private const string ENDURANCE = "";
        private const string NAME = "Name: ";
        private const string TYPE = "Type: ";
        private const string LEVEL = "Level: ";
        private const string DAMAGE = "Damage: ";
        private const string ATTACK_SPEED = "Attack Speed: ";
        private const string ATTACK_RANGE = "Attack Range: ";
        private const string EXPERIENCE = "Expirience: ";
        private const string AMOUNT = "Amount: ";
        
        private string GetTypesString(EntityType type)
        {
            if (type == EntityType.nothing)
                return "";

            // Создаем список для хранения названий типов
            System.Collections.Generic.List<string> typeNames = new System.Collections.Generic.List<string>();

            // Проверяем каждый флаг
            foreach (EntityType flag in System.Enum.GetValues(typeof(EntityType)))
            {
                // Пропускаем None
                if (flag == EntityType.nothing)
                    continue;

                // Если флаг установлен, добавляем его название в список
                if (type.HasFlag(flag))
                {
                    typeNames.Add(flag.ToString());
                }
            }

            // Объединяем названия через слеш
            return string.Join("/", typeNames);
        }

        public void Initialize()
        {
            healthBarGradient = so_GameUIColor.HealthBarGradient;
            enduranceBarGradient = so_GameUIColor.EnduranceBarGradient;
            ClearStats();
        }

        public void ClearStats()
        {
            SetIcon(null);
            SetHealth(1, 1);
            SetEndurance(1, 1);
            SetName(null);
            SetType(EntityType.nothing);
            SetLevel(0);
            SetDamage(0);
            SetAttackSpeed(0);
            SetAttackRange(0);
        }
        
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        
        public void SetHealth(int current, int max)
        {
            healthTxt.text = HEALTH + current;
            var resultHealth = (float)current/max;
            healthBar.fillAmount = resultHealth;
            healthBar.color = healthBarGradient.Evaluate(resultHealth);
        }
        
        public void SetEndurance(float current, int max)
        {
            enduranceTxt.text = ENDURANCE + current;
            var resultEndurance = current/max;
            enduranceBar.fillAmount = resultEndurance;
            enduranceBar.color = enduranceBarGradient.Evaluate(resultEndurance);
        }
        
        public void SetName(string name) => nameTxt.text = NAME + name;
        
        public void SetType(EntityType type)
        {
            string typesString = GetTypesString(type);
            typeTxt.text = TYPE + typesString;
        }

        public void SetLevel(int level) => levelTxt.text = LEVEL + level;
        
        public void SetExperience(int value) => experienceTxt.text = EXPERIENCE + value;
        
        public void SetAmount(int value) => amountTxt.text = AMOUNT + value;
        
        public void SetDamage(int damage) => damageTxt.text = DAMAGE + damage;
        
        public void SetAttackSpeed(int value) => attackSpeedTxt.text = ATTACK_SPEED + value;
        
        public void SetAttackRange(int value) => attackRangeTxt.text = ATTACK_RANGE + value;
        
    }
}