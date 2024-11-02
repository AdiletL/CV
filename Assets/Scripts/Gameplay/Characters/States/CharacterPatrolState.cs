using UnityEngine;

namespace Character
{
    public class CharacterPatrolState : State, IPatrol
    {
        public StateMachine PatrolState { get; private set; } = new ();

        public GameObject GameObject { get; set; }
        public Transform EndPoint { get; set; }

        public override void Initialize()
        {
            
        }

        public override void Enter()
        {
            DestermineState();
        }

        public override void Update()
        {
            DestermineState();
        }

        public override void Exit()
        {
            PatrolState?.SetStates(typeof(CharacterPatrolState));
        }

        protected virtual void DestermineState()
        {
            
        }
    }

    public class CharacterPatrolStateBuilder : StateBuilder<CharacterPatrolState>
    {
        public CharacterPatrolStateBuilder(CharacterPatrolState instance) : base(instance)
        {
        }

        public CharacterPatrolStateBuilder SetRunState(IState runState)
        {
            state.PatrolState.AddState(runState);
            return this;
        }

        public CharacterPatrolStateBuilder SetAttackState(IState attackState)
        {
            state.PatrolState.AddState(attackState);
            return this;
        }
        
        public CharacterPatrolStateBuilder SetEndPoint(Transform end)
        {
            state.EndPoint = end;
            return this;
        }
    }
}