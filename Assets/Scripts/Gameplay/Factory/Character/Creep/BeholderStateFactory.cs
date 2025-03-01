﻿using System;
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
                .SetIdleClips(so_BeholderMove.IdleClip)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderPatrolState CreatePatrolState()
        {
            return (BeholderPatrolState)new BeholderPatrolStateBuilder()
                .SetCreepSwitchAttackState(creepSwitchAttackState)
                .SetNavMeshAgent(navMeshAgent)
                .SetCharacterAnimation(characterAnimation)
                .SetWalkClips(so_BeholderMove.WalkClips)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetPatrolPoints(patrolPoints)
                .SetCenter(unitCenter.Center)
                .SetMovementSpeed(so_BeholderMove.WalkSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderRunState CreateRunState()
        {
            return (BeholderRunState)new BeholderRunStateBuilder()
                .SetCharacterSwitchAttack(creepSwitchAttackState)
                .SetNavMesh(navMeshAgent)
                .SetTimerRunToTarget(so_BeholderMove.TimerRunToTarget)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetUnitAnimation(characterAnimation)
                .SetRunClips(so_BeholderMove.RunClips)
                .SetMovementSpeed(so_BeholderMove.RunSpeed)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private BeholderDefaultAttackState CreateDefaultAttack()
        {
            return (BeholderDefaultAttackState)new BeholderDefaultAttackStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetSwitchMoveState(creepSwitchMoveState)
                .SetUnitAnimation(characterAnimation)
                .SetAttackClips(so_BeholderAttack.AttackClips)
                .SetCooldownClip(so_BeholderAttack.CooldownClip)
                .SetApplyDamageMoment(so_BeholderAttack.ApplyDamageMoment)
                .SetRange(so_BeholderAttack.Range)
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetAttackSpeed(so_BeholderAttack.AttackSpeed)
                .SetDamage(so_BeholderAttack.Damage)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
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