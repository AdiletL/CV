using Calculate;
using Gameplay.Resistance;
using Gameplay.Ability;
using Gameplay.Spawner;
using Unit;
using Unit.Character.Creep;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Damage
{
    public abstract class Damage : IDamageable
    {
        [Inject] private HealPopUpPopUpSpawner healPopUpPopUpSpawner;
        
        public GameObject Owner { get; }
        public AbilityHandler AbilityHandler { get; }
        public int CurrentDamage { get; }
        public int AdditionalDamage { get; protected set; }
        
        protected UnitCenter ownerUnitCenter;
        protected int result;
        
        public Damage(int amount, GameObject owner)
        {
            this.CurrentDamage = amount;
            this.Owner = owner;
            this.AbilityHandler = owner.GetComponent<AbilityHandler>();
            ownerUnitCenter = owner.GetComponent<UnitCenter>();
            
            if (CurrentDamage < 0) CurrentDamage = 999999;
        }
        
        public void AddAdditionalDamage(int value)
        {
            AdditionalDamage += value;
        }
        public void RemoveAdditionalDamage(int value)
        {
            AdditionalDamage -= value;
        }

        public virtual int GetTotalDamage(GameObject gameObject)
        {
            result = CurrentDamage + AdditionalDamage;
            
            var resistanceHandler = gameObject.GetComponent<ResistanceHandler>();
            if (resistanceHandler && resistanceHandler.TryGetResistance<NormalDamageResistance>(out var normalResistance))
            {
                var resistanceValue = new GameValue(normalResistance.Value, normalResistance.ValueType);
                result -= resistanceValue.Calculate(result);
                if (result < 0) result = 0;
            }
            return result;
        }

        protected virtual void CheckSkill(int totalDamage, UnitCenter targetUnitCenter)
        {
            if(!AbilityHandler || totalDamage <= 0) return;

            if (AbilityHandler.IsAbilityNotNull(AbilityType.ApplyDamageHeal))
            {
                var skills = AbilityHandler.GetSkills(AbilityType.ApplyDamageHeal);
                if (skills.Count > 0)
                {
                    ApplyDamageHeal skill = null;
                    for (int i = skills.Count - 1; i >= 0; i--)
                    {
                        skill = skills[i] as ApplyDamageHeal;
                        Heal(totalDamage, skill.ValueType, skill.Value);
                    }
                }
            }
        }

        private void Heal(int totalDamage, ValueType valueType, int value)
        {
            var gameValue = new Calculate.GameValue(value, valueType);
            var result = gameValue.Calculate(totalDamage);
            Owner.GetComponent<UnitHealth>().AddHealth(result);
            healPopUpPopUpSpawner.CreatePopUp(ownerUnitCenter.Center.position, result);
        }
    }
}