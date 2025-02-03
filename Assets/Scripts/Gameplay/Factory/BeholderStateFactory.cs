using System;
using Machine;
using Unit.Character.Creep;

namespace Gameplay.Factory
{
    public class BeholderStateFactory : CreepStateFactory
    {
        private CreepSwitchMoveState creepSwitchMoveState;
        
        public void SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState) => this.creepSwitchMoveState = creepSwitchMoveState;
        
        public override void Initialize()
        {
            
        }

        public override State CreateState(Type stateType)
        {
            State result = stateType switch
            {
                _ when stateType == typeof(BeholderIdleState) => CreateIdleState(),
                _ when stateType == typeof(BeholderPatrolState) => CreatePatrolState(),
                _ when stateType == typeof(BeholderRunState) => CreateRunState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        
        private BeholderIdleState CreateIdleState()
        {
            return (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCreepSwitchMoveState(creepSwitchMoveState)
                .SetCharacterAnimation(characterAnimation)
                .SetIdleClips(so_CreepMove.IdleClip)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderPatrolState CreatePatrolState()
        {
            return (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetCharacterAnimation(characterAnimation)
                .SetWalkClips(so_CreepMove.WalkClips)
                .SetRotationSpeed(so_CreepMove.RotateSpeed)
                .SetPatrolPoints(patrolPoints)
                .SetCenter(unitCenter.Center)
                .SetMovementSpeed(so_CreepMove.WalkSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderRunState CreateRunState()
        {
            return (BeholderRunState)new BeholderRunStateBuilder()
                .SetCenter(unitCenter.Center)
                .SetRotationSpeed(so_CreepMove.RotateSpeed)
                .SetUnitAnimation(characterAnimation)
                .SetRunClips(so_CreepMove.RunClips)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_CreepMove.RunSpeed)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class BeholderStateFactoryBuilder : CreepStateFactoryBuilder
    {
        public BeholderStateFactoryBuilder() : base(new BeholderStateFactory())
        {
        }

        public BeholderStateFactoryBuilder SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState)
        {
            if(characterStateFactory is BeholderStateFactory beholderStateFactory)
                beholderStateFactory.SetCreepSwitchMoveState(creepSwitchMoveState);
            return this;
        }
    }
}