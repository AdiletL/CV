using System.Collections.Generic;
using System.Text;
using ScriptableObjects.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI.ScreenSpace.Stats
{
    public class UIStats : MonoBehaviour
    {
        [Inject] private SO_GameUIColor so_GameUIColor;

        [SerializeField] private Image icon;
        
        [Space] 
        [SerializeField] private UIStat damageStat;
        [SerializeField] private UIStat movementStat;
        [SerializeField] private UIStat armorStat;
        [SerializeField] private UIStat rangeStat;
        [SerializeField] private UIStat healthStat;
        [SerializeField] private UIStat enduranceStat;
        [SerializeField] private UIStat manaStat;
        [SerializeField] private UIStat regenerationHealthStat;
        [SerializeField] private UIStat regenerationEnduranceStat;
        [SerializeField] private UIStat regenerationManaStat;
        
        [Space]
        [SerializeField] private UIStat levelStat;
        [SerializeField] private UIStat experienceStat;
        
        private Gradient healthBarGradient;
        private Gradient enduranceBarGradient;
        private Gradient manaBarGradient;

        //private readonly Vector2 sizeDelta = new Vector2(280, 25);
        private StringBuilder textBuilder = new();
        private float resulBar;

        public void Initialize()
        {
            healthBarGradient = so_GameUIColor.HealthBarGradient;
            enduranceBarGradient = so_GameUIColor.EnduranceBarGradient;
            manaBarGradient = so_GameUIColor.ManaBarGradient;
            ClearStats();
        }

        public void ClearStats()
        {
            SetImage(null);
            SetHealth(0, 0);
            SetEndurance(0, 0);
        }
        
        public void SetImage(Sprite sprite) => icon.sprite = sprite;
        
        public void SetHealth(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current).Append("/").Append(max);
            healthStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            healthStat.SetBar(resulBar);
            healthStat.SetGradientBar(healthBarGradient.Evaluate(resulBar));
        }
        
        public void SetEndurance(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current).Append("/").Append(max);
            enduranceStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            enduranceStat.SetBar(resulBar);
            enduranceStat.SetGradientBar(enduranceBarGradient.Evaluate(resulBar));
        }

        public void SetMana(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current).Append("/").Append(max);
            manaStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            manaStat.SetBar(resulBar);
            manaStat.SetGradientBar(manaBarGradient.Evaluate(resulBar));
        }

        public void SetDamage(float value)
        {
            damageStat.SetText($"{value:0}");
        }

        public void SetMovement(float value)
        {
            movementStat.SetText($"{value:0}");
        }

        public void SetArmor(float value)
        {
            armorStat.SetText($"{value:0}");
        }

        public void SetRange(float value)
        {
            rangeStat.SetText($"{value:0}");
        }
        
        public void SetLevel(float value)
        {
            levelStat.SetText($"{value:0}");
        }
        
        public void SetExperience(float value)
        {
            experienceStat.SetText($"{value:0}");
        }

        public void SetRegenerationHealth(float value)
        {
            regenerationHealthStat.SetText($"{value:0.0}");
        }
        
        public void SetRegenerationEndurance(float value)
        {
            regenerationEnduranceStat.SetText($"{value:0.0}");
        }

        public void SetRegenerationMana(float value)
        {
            regenerationManaStat.SetText($"{value:0.0}");
        }
    }

    public static class UnitInformation
    {
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
            if (type == EntityType.Nothing)
                return "";

            // Создаем список для хранения названий типов
            List<string> typeNames = new List<string>();

            // Проверяем каждый флаг
            foreach (EntityType flag in System.Enum.GetValues(typeof(EntityType)))
            {
                // Пропускаем None
                if (flag == EntityType.Nothing)
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