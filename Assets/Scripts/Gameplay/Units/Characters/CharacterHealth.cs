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
        
        protected AbilityHandler abilityHandler;
        protected ResistanceHandler resistanceHandler;
        protected CharacterStatsController characterStatsController;

        public override void Initialize()
        {
            base.Initialize();
            abilityHandler = GetComponent<AbilityHandler>();
            resistanceHandler = GetComponent<ResistanceHandler>();
            characterStatsController = GetComponent<CharacterStatsController>();
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
            if (characterStatsController &&
                ((EvasionStat)characterStatsController.GetStat(StatType.Evasion)).TryEvade())
            {
                evasionPopUpSpawner.CreatePopUp(unitCenter.Center.position);
                return;
            }
            
            damageData = abilityHandler.DamageModifiers(damageData);
            
            var currentDamage = damageData.Amount;
            damageData = resistanceHandler.DamageModifiers(damageData);
            protectionPopUpSpawner.CreatePopUp(unitCenter.Center.position, currentDamage - damageData.Amount, damageData.DamageTypeID);
            
            VampirismEffect(damageData.Owner, damageData.Amount);
            base.TakeDamage(damageData);
        }
    }
}

