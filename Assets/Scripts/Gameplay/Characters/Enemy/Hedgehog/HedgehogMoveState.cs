using ScriptableObjects.Character.Enemy;
using UnityEngine;

namespace Character.Enemy
{
    public class HedgehogMoveState : EnemyMoveState
    {
        private SO_HedgehogMove so_HedgehogMove;
        private HedgehogAnimation hedgehogAnimation;
        
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            so_HedgehogMove = (SO_HedgehogMove)SO_CharacterMove;
            hedgehogAnimation = (HedgehogAnimation)CharacterAnimation;
        }

        protected override void DestermineState()
        {
            base.DestermineState();
            if (!movementStates.ContainsKey(typeof(HedgehogPatrolState)))
            {
                var patrolState = (HedgehogPatrolState)new HedgehogPatrolStateBuilder()
                    .SetEnemyAnimation(hedgehogAnimation)
                    .SetWalkClip(so_HedgehogMove.WalkClip)
                    .SetStartPoint(StartPlatform)
                    .SetEndPoint(EndPlatform)
                    .SetGameObject(GameObject)
                    .SetMovementSpeed(so_HedgehogMove.RunSpeed)
                    .SetStateMachine(this.StateMachine)
                    .Build();
                
                patrolState.Initialize();
                movementStates.TryAdd(typeof(HedgehogPatrolState), patrolState);
                this.StateMachine.AddStates(patrolState);
            }
            
            this.StateMachine.SetStates(typeof(HedgehogPatrolState));
        }
    }

    public class HedgehogMoveStateBuilder : EnemyMoveStateBuilder
    {
        public HedgehogMoveStateBuilder() : base(new HedgehogMoveState())
        {
        }

        public HedgehogMoveStateBuilder SetStartPlatform(Platform startPlatform)
        {
            if (state is HedgehogMoveState hedgehogMoveState)
            {
                hedgehogMoveState.StartPlatform = startPlatform;
            }

            return this;
        }
        public HedgehogMoveStateBuilder SetEndPlatform(Platform endPlatform)
        {
            if (state is HedgehogMoveState hedgehogMoveState)
            {
                hedgehogMoveState.EndPlatform = endPlatform;
            }

            return this;
        }
    }
}