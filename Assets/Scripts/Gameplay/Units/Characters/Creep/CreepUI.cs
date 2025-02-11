using System;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepUI : CharacterUI
    {
        [Space] 
        [SerializeField] private Transform healthParent;
        [SerializeField] private Transform enduranceParent;

        private float countCooldownHide;
        private readonly float cooldownHide = 3f;

        public override void Initialize()
        {
            base.Initialize();
            HideHealth();
            HideEndurance();
        }

        private void Update()
        {
            countCooldownHide += Time.deltaTime;
            if (countCooldownHide >= cooldownHide)
            {
                HideHealth();
                HideEndurance();
                countCooldownHide = 0;
            }
        }

        public void OnTakeDamage()
        {
            ShowHealth();
            ShowEndurance();
            countCooldownHide = 0;
        }
        
        private void ShowHealth() => healthParent.gameObject.SetActive(true);
        private void HideHealth() => healthParent.gameObject.SetActive(false);
        
        private void ShowEndurance() => enduranceParent.gameObject.SetActive(true);
        private void HideEndurance() => enduranceParent.gameObject.SetActive(false);
    }
}