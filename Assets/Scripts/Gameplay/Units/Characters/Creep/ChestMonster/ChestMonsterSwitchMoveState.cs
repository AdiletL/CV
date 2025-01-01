using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class ChestMonsterSwitchMoveState : CreepSwitchMoveState
    {
        private SO_ChestMonsterMove so_HedgehogMove;
        private ChestMonsterAnimation hedgehogAnimation;
        
        public Transform Start { get; set; }
        public Transform End { get; set; }
        public LayerMask EnemyLayer { get; set; }
        
        
        private ChestMonsterPatrolState CreatePatrolState()
        {
            return (ChestMonsterPatrolState)new ChestMonsterPatrolStateBuilder()
                .SetCenter(Center)
                .SetEnemyAnimation(hedgehogAnimation)
                .SetEnemyLayer(EnemyLayer)
                .SetWalkClip(so_HedgehogMove.WalkClip)
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
            so_HedgehogMove = (SO_ChestMonsterMove)SO_CharacterMove;
            hedgehogAnimation = (ChestMonsterAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!this.StateMachine.IsStateNotNull(typeof(ChestMonsterPatrolState)))
            {
                var newState = CreatePatrolState();
                
                newState.Initialize();
                this.StateMachine.AddStates(newState);
            }
            
            this.StateMachine.SetStates(typeof(ChestMonsterPatrolState));
        }
    }
    public class ChestMonsterSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public ChestMonsterSwitchMoveStateBuilder() : base(new ChestMonsterSwitchMoveState())
        {
        }
        public ChestMonsterSwitchMoveStateBuilder SetStart(Transform start)
        {
            if (state is ChestMonsterSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.Start = start;
            }

            return this;
        }
        public ChestMonsterSwitchMoveStateBuilder SetEnd(Transform end)
        {
            if (state is ChestMonsterSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.End = end;
            }

            return this;
        }
        public ChestMonsterSwitchMoveStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if (state is ChestMonsterSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.EnemyLayer = layer;
            }

            return this;
        }
    }
}