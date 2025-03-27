using ScriptableObjects.Unit.Character;

namespace Gameplay.Effect
{
    public class DisableEffect : Effect
    {
        public override EffectType EffectTypeID { get; } = EffectType.Disable;
        
        private readonly DisableType disableTypeID;
        private readonly float timer;
        private float countTimer;
        
        
        public DisableEffect(DisableConfig disableConfig, string id) : base(disableConfig, id)
        {
            disableTypeID = disableConfig.DisableTypeID;
            timer = disableConfig.Timer;
        }

        public override void ClearValues()
        {
            countTimer = timer;
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        public override void UpdateEffect()
        {
            countTimer = timer;
        }

        public override void ApplyEffect()
        {
            
        }
    }

    [System.Serializable]
    public class DisableConfig : EffectConfig
    {
        public DisableType DisableTypeID;
        public float Timer;
    }
}