using System;
using ScriptableObjects.Gameplay;
using UnityEngine;
using Zenject;

namespace Unit.Character
{
    public abstract class CharacterEndurance : UnitEndurance
    {
        private SO_GameConfig gameConfig;

        private float amountDecreaseEndurance;
        private float cooldownDecreaseEndurance;
        private float countCooldownDecreaseEndurance;

        [Inject]
        private void Construct(SO_GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        public override void Initialize()
        {
            base.Initialize();
            cooldownDecreaseEndurance = gameConfig.CooldownDecreaseEndurance;
            amountDecreaseEndurance = gameConfig.AmountDecreaseEndurance;
        }

        private void Update()
        {
            DecreaseEndurance();
        }

        protected virtual void DecreaseEndurance()
        {
            countCooldownDecreaseEndurance += Time.deltaTime;

            if (countCooldownDecreaseEndurance >= cooldownDecreaseEndurance)
            {
                RemoveEndurance(amountDecreaseEndurance);
                countCooldownDecreaseEndurance = 0;
            }
        }
    }
}