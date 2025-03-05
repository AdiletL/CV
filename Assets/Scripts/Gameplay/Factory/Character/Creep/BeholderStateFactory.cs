using System;
using Machine;
using ScriptableObjects.Unit.Character.Creep;
using Unit.Character.Creep;
using UnityEngine;

namespace Gameplay.Factory.Character.Creep
{
    public class BeholderStateFactory : CreepStateFactory
    {
        private CreepSwitchMoveState creepSwitchMoveState;
        private CreepSwitchAttackState creepSwitchAttackState;

        private SO_BeholderAttack so_BeholderAttack;
        private SO_BeholderMove so_BeholderMove;
        private SO_BeholderHealth so_BeholderHealth;
        
        public void SetCreepSwitchMoveState(CreepSwitchMoveState creepSwitchMoveState) => this.creepSwitchMoveState = creepSwitchMoveState;
        public void SetCreepSwitchAttackState(CreepSwitchAttackState creepSwitchAttackState) => this.creepSwitchAttackState = creepSwitchAttackState;
        public void SetBeholderAttackConfig(SO_BeholderAttack config) => so_BeholderAttack = config;
        public void SetBeholderMoveConfig(SO_BeholderMove config) => so_BeholderMove = config;
        public void SetBeholderHealthConfig(SO_BeholderHealth config) => so_BeholderHealth = config;
        
        
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
                _ when stateType == typeof(BeholderDefaultAttackState) => CreateDefaultAttack(),
                _ when stateType == typeof(BeholderTakeDamageState) => CreateTakeDamageState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        
        private BeholderIdleState CreateIdleState()
        {
            return (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCreepSwitchAttackState(creepSwitchAttackState)
                .SetCreepSwitchMoveState(creepSwitchMoveState)
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
                .SetCreepSwitchAttackState(creepSwitchAttackState)
                .SetNavMeshAgent(navMeshAgent)
                .SetCharacterAnimation(characterAnimation)
                .SetWalkClips(so_BeholderMove.WalkClips)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetPatrolPoints(patrolPoints)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
            result.MovementSpeedStat.AddValue(so_BeholderMove.RunSpeed);
            return result;
        }
        
        private BeholderRunState CreateRunState()
        {
            var result = (BeholderRunState)new BeholderRunStateBuilder()
                .SetCharacterSwitchAttack(creepSwitchAttackState)
                .SetNavMesh(navMeshAgent)
                .SetTimerRunToTarget(so_BeholderMove.TimerRunToTarget)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetUnitAnimation(characterAnimation)
                .SetConfig(so_BeholderMove)
                .SetRunClips(so_BeholderMove.RunClips)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            result.MovementSpeedStat.AddValue(so_BeholderMove.RunSpeed);
            return result;
        }
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            var result = (BeholderDefaultAttackState)new BeholderDefaultAttackStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetSwitchMoveState(creepSwitchMoveState)
                .SetUnitAnimation(characterAnimation)
                .SetAttackClips(so_BeholderAttack.AttackClips)
                .SetCooldownClip(so_BeholderAttack.CooldownClip)
                .SetApplyDamageMoment(so_BeholderAttack.ApplyDamageMoment)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetAttackSpeed(so_BeholderAttack.AttackSpeed)
                .SetDamage(so_BeholderAttack.Damage)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            result.RangeStat.AddValue(so_BeholderAttack.Range);
            result.DamageStat.AddValue(so_BeholderAttack.Damage);
            return result;
        }

        private BeholderTakeDamageState CreateTakeDamageState()
        {
            return (BeholderTakeDamageState)new BeholderTakeDamageStateBuilder()
                .SetCharacterSwitchAttack(creepSwitchAttackState)
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