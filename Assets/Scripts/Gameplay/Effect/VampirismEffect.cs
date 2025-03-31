using Calculate;
using Gameplay.Spawner;
using Gameplay.Unit;
using UnityEngine;
using Zenject;

namespace Gameplay.Effect
{
    public class VampirismEffect : Effect
    {
        [Inject] private HealPopUpPopUpSpawner healPopUpPopUpSpawner;

        public override EffectType EffectTypeID { get; } = EffectType.Vampirism;
        
        private IHealth targetHealth;
        private UnitCenter targetUnitCenter;
        private GameValue gameValue;
        

        public VampirismEffect(VampirismConfig config) : base(config)
        {
            gameValue = new GameValue(config.Value, config.ValueType);
            IsBuff = true;
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);
            targetHealth = target.GetComponent<IHealth>();
            targetUnitCenter = target.GetComponent<UnitCenter>();
        }

        public override void ClearValues()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        public override void UpdateEffect()
        {
            
        }

        public override void ApplyEffect()
        {
            
        }
        
        public void Heal(float totalDamage)
        {
            var result = gameValue.Calculate(totalDamage);
            targetHealth.HealthStat.AddCurrentValue((int)result);
            healPopUpPopUpSpawner.CreatePopUp(targetUnitCenter.Center.position, result);
        }
    }

    [System.Serializable]
    public class VampirismConfig : EffectConfig
    {
        public ValueType ValueType;
        public int Value;
    }
}