using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class HedgehogSwitchMoveState : CreepSwitchMoveState
    {
        private SO_HedgehogMove so_HedgehogMove;
        private HedgehogAnimation hedgehogAnimation;
        
        public Platform StartPlatform { get; set; }
        public Platform EndPlatform { get; set; }
        
        
        private HedgehogPatrolState CreatePatrolState()
        {
            return (HedgehogPatrolState)new HedgehogPatrolStateBuilder()
                .SetCenter(Center)
                .SetEnemyAnimation(hedgehogAnimation)
                .SetWalkClip(so_HedgehogMove.WalkClip)
                .SetStartPoint(StartPlatform)
                .SetEndPoint(EndPlatform)
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

        public HedgehogSwitchMoveStateBuilder SetStartPlatform(Platform startPlatform)
        {
            if (state is HedgehogSwitchMoveState hedgehogMoveState)
            {
                hedgehogMoveState.StartPlatform = startPlatform;
            }

            return this;
        }
        public HedgehogSwitchMoveStateBuilder SetEndPlatform(Platform endPlatform)
        {
            if (state is HedgehogSwitchMoveState hedgehogMoveState)
            {
                hedgehogMoveState.EndPlatform = endPlatform;
            }

            return this;
        }
    }
}