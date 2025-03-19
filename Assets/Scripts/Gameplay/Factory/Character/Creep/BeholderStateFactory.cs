using System;
using Gameplay.Unit;
using Gameplay.Unit.Character;
using Gameplay.Unit.Character.Creep;
using Machine;
using ScriptableObjects.Unit.Character.Creep;

namespace Gameplay.Factory.Character.Creep
{
    public class BeholderStateFactory : CreepStateFactory
    {
        private SO_BeholderAttack so_BeholderAttack;
        private SO_BeholderMove so_BeholderMove;
        private SO_BeholderHealth so_BeholderHealth;
        
        public void SetBeholderAttackConfig(SO_BeholderAttack config) => so_BeholderAttack = config;
        public void SetBeholderMoveConfig(SO_BeholderMove config) => so_BeholderMove = config;
        public void SetBeholderHealthConfig(SO_BeholderHealth config) => so_BeholderHealth = config;
        
        
        public override State CreateState(Type stateType)
        {
            State result = stateType switch
            {
                _ when stateType == typeof(BeholderIdleState) => CreateIdleState(),
                _ when stateType == typeof(BeholderPatrolState) => CreatePatrolState(),
                _ when stateType == typeof(BeholderMoveState) => CreateRunState(),
                _ when stateType == typeof(BeholderAttackState) => CreateAttackState(),
                _ when stateType == typeof(BeholderTakeDamageState) => CreateTakeDamageState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        
        private BeholderIdleState CreateIdleState()
        {
            return (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_BeholderMove)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderPatrolState CreatePatrolState()
        {
            var result = (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetCharacterAnimation(characterAnimation)
                .SetPatrolPoints(patrolPoints)
                .SetCenter(unitCenter.Center)
                .SetConfig(so_BeholderMove)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
            return result;
        }
        
        private BeholderMoveState CreateRunState()
        {
            var result = (BeholderMoveState)new BeholderMoveStateBuilder()
                .SetNavMesh(navMeshAgent)
                .SetTimerRunToTarget(so_BeholderMove.TimerRunToTarget)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetUnitAnimation(characterAnimation)
                .SetConfig(so_BeholderMove)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            return result;
        }
        
        private BeholderAttackState CreateAttackState()
        {
            var result = (BeholderAttackState)new BeholderAttackStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetConfig(so_BeholderAttack)
                .SetUnitRenderer(gameObject.GetComponent<UnitRenderer>())
                .SetUnitAnimation(characterAnimation)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            return result;
        }

        private BeholderTakeDamageState CreateTakeDamageState()
        {
            return (BeholderTakeDamageState)new BeholderTakeDamageStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetClip(so_BeholderHealth.takeDamageClip)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class BeholderStateFactoryBuilder : CreepStateFactoryBuilder
    {
        public BeholderStateFactoryBuilder() : base(new BeholderStateFactory())
        {
        }

        public BeholderStateFactoryBuilder SetBeholderAttackConfig(SO_BeholderAttack config)
        {
            if(factory is BeholderStateFactory beholderStateFactory)
                beholderStateFactory.SetBeholderAttackConfig(config);
            return this;
        }
        public BeholderStateFactoryBuilder SetBeholderMoveConfig(SO_BeholderMove config)
        {
            if(factory is BeholderStateFactory beholderStateFactory)
                beholderStateFactory.SetBeholderMoveConfig(config);
            return this;
        }
    }
}