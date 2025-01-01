using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogSwitchMoveState : CreepSwitchMoveState
    {
        private SO_HedgehogMove so_HedgehogMove;
        private HedgehogAnimation hedgehogAnimation;
        
        public Transform Start { get; set; }
        public Transform End { get; set; }
        public LayerMask EnemyLayer { get; set; }
        
        private HedgehogPatrolState CreatePatrolState()
        {
            return (HedgehogPatrolState)new HedgehogPatrolStateBuilder()
                .SetCenter(Center)
                .SetEnemyAnimation(hedgehogAnimation)
                .SetWalkClip(so_HedgehogMove.WalkClip)
                .SetEnemyLayer(EnemyLayer)
                .SetStart(Start)
                .SetEnd(End)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_HedgehogMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            so_HedgehogMove = (SO_HedgehogMove)SO_CharacterMove;
            hedgehogAnimation = (HedgehogAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!this.StateMachine.IsStateNotNull(typeof(HedgehogPatrolState)))
            {
                var newState = CreatePatrolState();
                
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }
            
            this.StateMachine.SetStates(typeof(HedgehogPatrolState));
        }
    }

    public class HedgehogSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public HedgehogSwitchMoveStateBuilder() : base(new HedgehogSwitchMoveState())
        {
        }

        public HedgehogSwitchMoveStateBuilder SetStart(Transform start)
        {
            if (state is HedgehogSwitchMoveState hedgehogMoveState)
            {
                hedgehogMoveState.Start = start;
            }

            return this;
        }
        public HedgehogSwitchMoveStateBuilder SetEnd(Transform end)
        {
            if (state is HedgehogSwitchMoveState hedgehogMoveState)
            {
                hedgehogMoveState.End = end;
            }

            return this;
        }
        public HedgehogSwitchMoveStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if (state is HedgehogSwitchMoveState hedgehogMoveState)
            {
                hedgehogMoveState.EnemyLayer = layer;
            }

            return this;
        }
    }
}