using Calculate;
using Gameplay.Skill;
using Gameplay.Spawner;
using Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Damage
{
    public abstract class Damage : IDamageable
    {
        [Inject] private HealPopUpPopUpSpawner healPopUpPopUpSpawner;
        
        public GameObject Owner { get; }
        public SkillHandler SkillHandler { get; }
        public int CurrentDamage { get; }
        public int AdditionalDamage { get; protected set; }
        
        private UnitCenter ownerUnitCenter;

        public Damage(int amount, GameObject owner)
        {
            this.CurrentDamage = amount;
            this.Owner = owner;
            this.SkillHandler = owner.GetComponent<SkillHandler>();
            ownerUnitCenter = owner.GetComponent<UnitCenter>();
            
            if (CurrentDamage < 0) CurrentDamage = 999999;
        }
        
        public void AddAdditionalDamage(int value)
        {
            AdditionalDamage += value;
        }
        public void RemoveAdditionalDamage(int value)
        {
            AdditionalDamage -= value;
        }

        public abstract int GetTotalDamage(GameObject gameObject);

        protected virtual void CheckSkill(int totalDamage, UnitCenter targetUnitCenter)
        {
            if(!SkillHandler || totalDamage <= 0) return;

            if (SkillHandler.IsSkillNotNull(SkillType.applyDamageHeal))
            {
                var skills = SkillHandler.GetSkills(SkillType.applyDamageHeal);
                if (skills.Count > 0)
                {
                    ApplyDamageHeal skill = null;
                    for (int i = skills.Count - 1; i >= 0; i--)
                    {
                        skill = skills[i] as ApplyDamageHeal;
                        Heal(totalDamage, skill.ValueType, skill.Value);
                    }
                }
            }
        }

        private void Heal(int totalDamage, ValueType valueType, int value)
        {
            var gameValue = new Calculate.GameValue(value, valueType);
            var result = gameValue.Calculate(totalDamage);
            Owner.GetComponent<UnitHealth>().AddHealth(result);
            healPopUpPopUpSpawner.CreatePopUp(ownerUnitCenter.Center.position, result);
        }
    }
}