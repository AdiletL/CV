using System.Collections.Generic;
using ScriptableObjects.Gameplay;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        [SerializeField] private Transform txtParent;

        [Space] 
        [SerializeField] private AssetReferenceGameObject informationTxtPrefab;
        
        [Space] //TODO: Change on icon
        [SerializeField] private TextMeshProUGUI skillTxt;
        
        private Gradient healthBarGradient;
        private Gradient enduranceBarGradient;

        private readonly Vector2 sizeDelta = new Vector2(280, 25);
        private int countText;
        
        private const float baseFontSize = 25;
        
        private float resultHealth;
        private float resultEndurance;

        private List<TextMeshProUGUI> texts = new(10);

        private TextMeshProUGUI CreateText()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(informationTxtPrefab.AssetGUID).WaitForCompletion();
            var result = Instantiate(prefab, txtParent).GetComponent<TextMeshProUGUI>();
            result.rectTransform.sizeDelta = sizeDelta;
            return result.GetComponent<TextMeshProUGUI>();
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
            SetHealth(0, 0);
            SetEndurance(0, 0);
            countText = 0;
            foreach (var VARIABLE in texts)
                VARIABLE.text = string.Empty;
        }
        
        public void SetIcon(Sprite sprite) => icon.sprite = sprite;
        
        public void SetHealth(int current, int max)
        {
            healthTxt.text = StatsNames.HEALTH + current;
            
            if (current == 0 || max == 0) this.resultHealth = 0;
            else resultHealth = (float)current/max;
            
            healthBar.fillAmount = resultHealth;
            healthBar.color = healthBarGradient.Evaluate(resultHealth);
        }
        
        public void SetEndurance(float current, int max)
        {
            enduranceTxt.text = StatsNames.ENDURANCE + current;
            
            if (current == 0 || max == 0) this.resultEndurance = 0;
            else resultEndurance = (float)current/max;
            
            enduranceBar.fillAmount = resultEndurance;
            enduranceBar.color = enduranceBarGradient.Evaluate(resultEndurance);
        }

        public void AddText(string text)
        {
            if (countText >= texts.Count)
                texts.Add(CreateText());
            
            texts[countText].text = text;
            countText++;
        }

        public void Build()
        {
            var currentFontSize = baseFontSize - (countText / 2);
            foreach (var VARIABLE in texts)
                VARIABLE.fontSize = currentFontSize;
        }
    }

    public static class StatsNames
    {
        public const string HEALTH = "";
        public const string ENDURANCE = "";
        public const string NAME = "Name: ";
        public const string TYPE = "Type: ";
        public const string LEVEL = "Level: ";
        public const string DAMAGE = "Damage: ";
        public const string ATTACK_SPEED = "Attack Speed: ";
        public const string ATTACK_RANGE = "Attack Range: ";
        public const string EXPERIENCE = "Expirience: ";
        public const string AMOUNT = "Amount: ";
        
        public static string GetTypesString(EntityType type)
        {
            if (type == EntityType.nothing)
                return "";

            // Создаем список для хранения названий типов
            List<string> typeNames = new List<string>();

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
    }
}