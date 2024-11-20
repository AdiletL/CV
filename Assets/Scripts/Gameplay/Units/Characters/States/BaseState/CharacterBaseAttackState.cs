using Machine;
using UnityEngine;

namespace Unit.Character
{
    public class CharacterBaseAttackState : State, IAttack
    {
        public override StateCategory Category { get; } = StateCategory.attack;
        
        protected GameObject currentTarget;
        
        public IDamageble Damageble { get; set; }
        public float AmountAttack { get; set; }

        
        public override void Initialize()
        {
            
        }
        
        public override void Enter()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void LateUpdate()
        {
            
        }

        public override void Exit()
        {
        }



        public virtual void ApplyDamage()
        {
            
        }

        public virtual void IncreaseStates(Unit.IState state)
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