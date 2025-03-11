using System;
using Gameplay.Ability;
using Gameplay.Resistance;
using UnityEngine;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterHealth : UnitHealth
    {
        protected AbilityHandler abilityHandler;
        protected ResistanceHandler resistanceHandler;

        public override void Initialize()
        {
            base.Initialize();
            abilityHandler = GetComponent<AbilityHandler>();
            resistanceHandler = GetComponent<ResistanceHandler>();
        }

        private void VampirismAbility(GameObject attacker, float totalDamage)
        {
            var abilityHandler = attacker.GetComponent<AbilityHandler>();
            if (abilityHandler == null) return;

            var vampirismAbilities = abilityHandler.GetAbilities(AbilityType.Vampirism);
            if(vampirismAbilities == null) return;
            for (int i = vampirismAbilities.Count - 1; i >= 0; i--)
                (vampirismAbilities[i] as VampirismAbility)?.Heal(totalDamage);
        }
        
        public override void TakeDamage(DamageData damageData)
        {
            damageData = abilityHandler.DamageModifiers(damageData);
            damageData = resistanceHandler.DamageModifiers(damageData);
            VampirismAbility(damageData.Owner, damageData.Amount);
            base.TakeDamage(damageData);
        }
    }
}

