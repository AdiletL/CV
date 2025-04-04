using System;
using Gameplay.Ability;
using Gameplay.Effect;
using Gameplay.Resistance;
using Gameplay.Spawner;
using UnityEngine;
using Zenject;

namespace Gameplay.Unit.Character
{
    public abstract class CharacterHealth : UnitHealth
    {
        [Inject] private ProtectionPopUpSpawner protectionPopUpSpawner;
        [Inject] private EvasionPopUpSpawner evasionPopUpSpawner;
        [Inject] private CriticalDamagePopUpSpawner criticalDamagePopUpSpawner;
        
        protected AbilityHandler abilityHandler;
        protected ResistanceHandler resistanceHandler;
        protected IEvasionApplier evasionApplier;

        
        private bool TryEvade()
        {
            if (evasionApplier != null && evasionApplier.Evasion.TryEvade())
            {
                evasionPopUpSpawner.CreatePopUp(unitCenter.Center.position);
                return true;
            }
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();
            TryGetComponent(out evasionApplier);
        }

        private void VampirismEffect(GameObject attacker, float totalDamage)
        {
            var effectHandler = attacker.GetComponent<EffectHandler>();
            if (!effectHandler) return;

            var vampirismEffect = effectHandler.GetEffects(EffectType.Vampirism);
            if(vampirismEffect == null) return;
            for (int i = vampirismEffect.Count - 1; i >= 0; i--)
                (vampirismEffect[i] as VampirismEffect)?.Heal(totalDamage);
        }

        private void ResistanceDamage(ref DamageData damageData)
        {
            float initialDamage = damageData.Amount;

            damageData = resistanceHandler.DamageResistanceModifiers(damageData);
            protectionPopUpSpawner.CreatePopUp(unitCenter.Center.position, initialDamage - damageData.Amount, damageData.DamageTypeID);

            damageData = abilityHandler.DamageResistanceModifiers(damageData);
        }
        
        public override void TakeDamage(DamageData damageData)
        {
            if (TryEvade())
                return;

            ResistanceDamage(ref damageData);
            VampirismEffect(damageData.Owner, damageData.Amount);
            base.TakeDamage(damageData);
        }
    }
}

