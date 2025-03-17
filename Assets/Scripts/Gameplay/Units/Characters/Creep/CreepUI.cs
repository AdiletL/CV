using System;
using UnityEngine;

namespace Gameplay.Unit.Character.Creep
{
    public class CreepUI : CharacterUI
    {
        private float countCooldownHide;
        private readonly float cooldownHide = 3f;

        public override void Initialize()
        {
            base.Initialize();
            HideStat();
        }

        private void Update()
        {
            countCooldownHide += Time.deltaTime;
            if (countCooldownHide >= cooldownHide)
            {
                HideStat();
                countCooldownHide = 0;
            }
        }

        public void OnTakeDamage()
        {
            ShowStat();
            countCooldownHide = 0;
        }

        private void ShowStat()
        {
            healthBarUI.Show();
            enduranceBarUI.Show();
            manaBarUI.Show();
        }

        private void HideStat()
        {
            healthBarUI.Hide();
            enduranceBarUI.Hide();
            manaBarUI.Hide();
        }
    }
}