using System.Collections.Generic;

namespace Character
{
    public class CharacterAttackState : State
    {
        public StateMachine AttackStateMachine;
        
        public IState meleeState;
        
        public override void Enter()
        {
            
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}