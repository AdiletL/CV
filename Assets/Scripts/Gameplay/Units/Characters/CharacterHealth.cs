using System;
using Gameplay.Ability;
using Gameplay.Effect;
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
            var effectHandler = attacker.GetComponent<EffectHandler>();
            if (!effectHandler) return;

            var vampirismEffect = effectHandler.GetEffects(EffectType.Vampirism);
            if(vampirismEffect == null) return;
            for (int i = vampirismEffect.Count - 1; i >= 0; i--)
                (vampirismEffect[i] as VampirismEffect)?.Heal(totalDamage);
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

