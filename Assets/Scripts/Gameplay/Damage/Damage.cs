using System.Collections.Generic;
using Gameplay.Ability;
using Gameplay.Spawner;
using Gameplay.Units.Item;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Damage
{
    public abstract class Damage : IDamageable
    {
        [Inject] protected DamagePopUpPopUpSpawner damagePopUpPopUpSpawner;
        
        public GameObject Owner { get; }
        public Stat DamageStat { get; }
        public AbilityHandler AbilityHandler { get; }
        public ItemHandler ItemHandler { get; }
        
        protected float result;
        
        public Damage(GameObject owner, Stat damageStat)
        {
            this.Owner = owner;
            this.DamageStat = damageStat;
            this.AbilityHandler = owner.GetComponent<AbilityHandler>();
            this.ItemHandler = owner.GetComponent<ItemHandler>();
        }

        public virtual int GetTotalDamage(GameObject gameObject)
        {
            result = DamageStat.CurrentValue;
            var targetUnitCenter = gameObject.GetComponent<UnitCenter>();
            
            CheckAbility(result, targetUnitCenter);
            CheckItem(result, targetUnitCenter);
            
            damagePopUpPopUpSpawner?.CreatePopUp(targetUnitCenter.Center.position, result);
            return (int)result;
        }

        protected virtual void CheckAbility(float totalDamage, UnitCenter targetUnitCenter)
        {
            if(!AbilityHandler || totalDamage <= 0) return;

            if (!AbilityHandler.IsAbilityNull(AbilityType.Vampirism))
            {
                var abilities = AbilityHandler.GetAbilities(AbilityType.Vampirism);
                VampirismAbility(abilities, totalDamage);
            }
        }

        protected virtual void CheckItem(float totalDamage, UnitCenter targetUnitCenter)
        {
            if(!ItemHandler || totalDamage <= 0) return;

            if (!ItemHandler.IsAbilityNull(AbilityType.Vampirism))
            {
                var abilities = ItemHandler.GetAbilities(AbilityType.Vampirism);
                VampirismAbility(abilities, totalDamage);
            }
        }

        private void VampirismAbility(List<Ability.Ability> abilities, float totalDamage)
        {
            VampirismAbility ability = null;
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                ability = abilities[i] as VampirismAbility;
                ability?.Heal(totalDamage);
            }
        }
    }
}