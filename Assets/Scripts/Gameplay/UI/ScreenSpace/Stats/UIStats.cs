using System.Collections.Generic;
using System.Text;
using ScriptableObjects.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI.ScreenSpace.Stats
{
    public class UIStats : MonoBehaviour
    {
        [Inject] private SO_GameUIColor so_GameUIColor;
        [Inject] private SO_GameStatIcon so_GameStatIcon;

        [SerializeField] private Image icon;
        
        [Space] 
        [SerializeField] private UIStat damageStat;
        [SerializeField] private UIStat movementSpeedStat;
        [SerializeField] private UIStat armorStat;
        [SerializeField] private UIStat rangeAttackStat;
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
            
            damageStat.SetIcon(so_GameStatIcon.Damage);
            movementSpeedStat.SetIcon(so_GameStatIcon.MovementSpeed);
            armorStat.SetIcon(so_GameStatIcon.Armor);
            rangeAttackStat.SetIcon(so_GameStatIcon.RangeAttack);
            
            ClearStats();
        }

        public void ClearStats()
        {
            SetImage(null);
            SetLevel(0);
            SetExperience(0, 0);
            SetDamage(0);
            SetMovementSpeed(0);
            SetArmor(0);
            SetRangeAttack(0);
            SetHealth(0, 0);
            SetEndurance(0, 0);
            SetMana(0, 0);
            SetRegenerationHealth(0);
            SetRegenerationEndurance(0);
            SetRegenerationMana(0);
        }
        
        public void SetImage(Sprite sprite) => icon.sprite = sprite;
        
        public void SetHealth(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current.ToString("0")).Append("/").Append(max.ToString("0"));
            healthStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            healthStat.SetBar(resulBar);
            healthStat.SetGradientBar(healthBarGradient.Evaluate(resulBar));
        }
        
        public void SetEndurance(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current.ToString("0")).Append("/").Append(max.ToString("0"));
            enduranceStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            enduranceStat.SetBar(resulBar);
            enduranceStat.SetGradientBar(enduranceBarGradient.Evaluate(resulBar));
        }

        public void SetMana(float current, float max)
        {
            textBuilder.Clear();
            textBuilder.Append(current.ToString("0")).Append("/").Append(max.ToString("0"));
            manaStat.SetText(textBuilder.ToString());
            
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            manaStat.SetBar(resulBar);
            manaStat.SetGradientBar(manaBarGradient.Evaluate(resulBar));
        }

        public void SetDamage(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0"));
            damageStat.SetText(textBuilder.ToString());
        }

        public void SetMovementSpeed(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0"));
            movementSpeedStat.SetText(textBuilder.ToString());
        }

        public void SetArmor(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0"));
            armorStat.SetText(textBuilder.ToString());
        }

        public void SetRangeAttack(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0"));
            rangeAttackStat.SetText(textBuilder.ToString());
        }
        
        public void SetLevel(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0"));
            levelStat.SetText(textBuilder.ToString());
        }
        
        public void SetExperience(float current, float max)
        {
            if (current == 0 || max == 0) this.resulBar = 0;
            else resulBar = current/max;
            
            experienceStat.SetBar(resulBar);
        }

        public void SetRegenerationHealth(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0.0"));
            regenerationHealthStat.SetText(textBuilder.ToString());
        }
        
        public void SetRegenerationEndurance(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0.0"));
            regenerationEnduranceStat.SetText(textBuilder.ToString());
        }

        public void SetRegenerationMana(float value)
        {
            textBuilder.Clear();
            textBuilder.Append(value.ToString("0.0"));
            regenerationManaStat.SetText(textBuilder.ToString());
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