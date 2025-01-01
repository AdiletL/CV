using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class BeholderSwitchMoveState : CreepSwitchMoveState
    {
        private SO_BeholderMove so_BeholderMove;
        private BeholderAnimation beholderAnimation;
        
        public Transform Start { get; set; }
        public Transform End { get; set; }
        public LayerMask EnemyLayer { get; set; }
        

        public override bool IsCanMovement()
        {
            return Start && End;
        }

        private BeholderPatrolState CreatePatrolState()
        {
            return (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetCenter(Center)
                .SetEnemyAnimation(beholderAnimation)
                .SetWalkClip(so_BeholderMove.WalkClip)
                .SetEnemyLayer(EnemyLayer)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetStart(Start)
                .SetEnd(End)
                .SetGameObject(GameObject)
                .SetMovementSpeed(so_BeholderMove.RunSpeed)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            so_BeholderMove = (SO_BeholderMove)SO_CharacterMove;
            beholderAnimation = (BeholderAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!this.StateMachine.IsStateNotNull(typeof(BeholderPatrolState)))
            {
                var patrolState = CreatePatrolState();
                
                patrolState.Initialize();
                this.StateMachine.AddStates(patrolState);
            }
            
            this.StateMachine.SetStates(typeof(BeholderPatrolState));
        }
    }
    
    public class BeholderSwitchMoveStateBuilder : CreepSwitchMoveStateBuilder
    {
        public BeholderSwitchMoveStateBuilder() : base(new BeholderSwitchMoveState())
        {
        }
        
        public BeholderSwitchMoveStateBuilder SetStart(Transform start)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.Start = start;
            }

            return this;
        }
        public BeholderSwitchMoveStateBuilder SetEnd(Transform end)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.End = end;
            }

            return this;
        }
        public BeholderSwitchMoveStateBuilder SetEnemyLayer(LayerMask layer)
        {
            if (state is BeholderSwitchMoveState characterSwitchMoveState)
            {
                characterSwitchMoveState.EnemyLayer = layer;
            }

            return this;
        }
    }
}