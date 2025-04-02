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

        public override void Initialize()
        {
            base.Initialize();
            abilityHandler = GetComponent<AbilityHandler>();
            resistanceHandler = GetComponent<ResistanceHandler>();
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
        
        public override void TakeDamage(DamageData damageData)
        {
            if (TryEvade())
                return;

            if (damageData.DamageTypeID.HasFlag(DamageType.Physical) &&
                damageData.Owner.TryGetComponent(out AbilityHandler abilityHandler))
            {
                var criticalDamages = abilityHandler.GetCriticalDamages(damageData.Amount);
                if (criticalDamages != null && criticalDamages.Count > 0)
                {
                    foreach (var criticalDamage in criticalDamages)
                    {
                        damageData.Amount += criticalDamage;
                        ApplyDamage(ref damageData, criticalDamage);
                    }
                    base.TakeDamage(damageData);
                    return;
                }
            }

            ApplyDamage(ref damageData);

            base.TakeDamage(damageData);
        }

        private bool TryEvade()
        {
            if (evasionApplier != null && evasionApplier.Evasion.TryEvade())
            {
                evasionPopUpSpawner.CreatePopUp(unitCenter.Center.position);
                return true;
            }
            return false;
        }

        private void ApplyDamage(ref DamageData damageData, float criticalDamage = 0)
        {
            float initialDamage = damageData.Amount;

            // Модификация урона резистами и защитой
            damageData = resistanceHandler.DamageModifiers(damageData);
            protectionPopUpSpawner.CreatePopUp(unitCenter.Center.position, initialDamage - damageData.Amount, damageData.DamageTypeID);

            // Модификация урона способностями
            damageData = abilityHandler.DamageResistanceModifiers(damageData);

            // Вампиризм
            VampirismEffect(damageData.Owner, damageData.Amount);

            // Поп-ап критического урона
            if (criticalDamage > 0)
                criticalDamagePopUpSpawner.CreatePopUp(unitCenter.Center.position, damageData.Amount);
        }
    }
}

