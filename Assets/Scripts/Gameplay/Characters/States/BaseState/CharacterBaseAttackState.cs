using UnityEngine;

namespace Character
{
    public class CharacterBaseAttackState : State, IAttack
    {
        protected GameObject target;
        
        public IDamageble Damageble { get; set; }
        
        public override void Initialize()
        {
            
        }
        
        public override void Enter()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Exit()
        {
        }

        public virtual void ApplyDamage()
        {
            
        }
    }

    public class CharacterBaseAttackStateBuilder : StateBuilder<CharacterBaseAttackState>
    {
        public CharacterBaseAttackStateBuilder(CharacterBaseAttackState instance) : base(instance)
        {
        }

        public CharacterBaseAttackStateBuilder SetDamageble(IDamageble damageble)
        {
            state.Damageble = damageble;
            return this;
        }
    }
}