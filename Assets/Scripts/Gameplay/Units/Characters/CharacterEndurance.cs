using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterEndurance : UnitEndurance
    {
        [Inject] private SO_GameConfig gameConfig;

        private float amountReductionEndurance;
        private float cooldownReductionEndurance;
        private float countCooldownReductionEndurance;
        
        private bool isInitialized;

        public override void Initialize()
        {
            base.Initialize();
            cooldownReductionEndurance = gameConfig.CooldownReductionEndurance;
            amountReductionEndurance = gameConfig.AmountReductionEndurance;
            isInitialized = true;
        }

        private void Update()
        {
            if(!isInitialized) return;
            ReductionEndurance();
        }

        protected virtual void ReductionEndurance()
        {
            countCooldownReductionEndurance += Time.deltaTime;

            if (countCooldownReductionEndurance >= cooldownReductionEndurance)
            {
                RemoveEndurance(amountReductionEndurance);
                countCooldownReductionEndurance = 0;
            }
        }
    }
}