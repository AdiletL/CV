using Gameplay.Unit;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.Effect
{
    public class TransformationEffect : Effect
    {
        public override EffectType EffectTypeID { get; } = EffectType.Transformation;
        
        public TransformationEffect(TransformationEffectConfig transformationEffectConfig) : base(transformationEffectConfig)
        {
            
        }

        public override void ClearValues()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateEffect()
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }

    public class TransformationEffectConfig : EffectConfig
    {
        public AssetReferenceT<GameObject> ModelPrefab;
        public Shader animationShader;
        public float Timer;
        public UnitStatConfigData StatConfigData;
    }
}