using System;
using ScriptableObjects.Unit.Character.Creep;
using Unit.Character;
using Unit.Character.Creep;

namespace Gameplay.Factory.Character.Creep
{
    public class BeholderSwitchStateFactory : CreepSwitchStateFactory
    {
        private SO_BeholderAttack so_BeholderAttack;
        private SO_BeholderMove so_BeholderMove;
        private CreepStateFactory creepStateFactory;
        
        public void SetBeholderAttackConfig(SO_BeholderAttack config) => so_BeholderAttack = config;
        public void SetBeholderMoveConfig(SO_BeholderMove config) => so_BeholderMove = config;
        public void SetCreepStateFactory(CreepStateFactory factory) => creepStateFactory = factory;
        

        public override void Initialize()
        {
            
        }


        public override CharacterSwitchAttackState CreateSwitchAttackState(Type stateType)
        {
            CharacterSwitchAttackState result = stateType switch
            {
                _ when stateType == typeof(BeholderSwitchAttackState) => CreateBeholderSwitchAttackState(),
                _ => throw new ArgumentException($"Unknown switchState type: {stateType}")
            };
            return result;
        }

        public override CharacterSwitchMoveState CreateSwitchMoveState(Type stateType)
        {
            CharacterSwitchMoveState result = stateType switch
            {
                _ when stateType == typeof(BeholderSwitchMoveState) => CreateBeholderSwitchMove(),
                _ => throw new ArgumentException($"Unknown switchState type: {stateType}")
            };
            return result;
        }
        
        
        private BeholderSwitchMoveState CreateBeholderSwitchMove()
        {
            return (BeholderSwitchMoveState)new BeholderSwitchSwitchMoveStateBuilder()
                .SetNavMeshAgent(navMeshAgent)
                .SetCreepStateFactory(creepStateFactory)
                .SetEndurance(characterEndurance)
                .SetConfig(so_BeholderMove)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetUnitAnimation(characterAnimation)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private BeholderSwitchAttackState CreateBeholderSwitchAttackState()
        {
            return (BeholderSwitchAttackState)new BeholderSwitchAttackStateBuilder()
                .SetCreepStateFactory(creepStateFactory)
                .SetNavMeshAgent(navMeshAgent)
                .SetConfig(so_BeholderAttack)
                .SetCharacterEndurance(characterEndurance)
                .SetUnitAnimation(characterAnimation)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .SetCenter(unitCenter.Center)
                .Build();
        }
    }

    public class BeholderSwitchStateFactoryBuilder : CreepSwitchStateFactoryBuilder
    {
        public BeholderSwitchStateFactoryBuilder() : base(new BeholderSwitchStateFactory())
        {
        }

        public BeholderSwitchStateFactoryBuilder SetBeholderAttackConfig(SO_BeholderAttack config)
        {
            if(factory is BeholderSwitchStateFactory beholderSwitchStateFactory)
                beholderSwitchStateFactory.SetBeholderAttackConfig(config);
            return this;
        }
        
        public BeholderSwitchStateFactoryBuilder SetBeholderMoveConfig(SO_BeholderMove config)
        {
            if(factory is BeholderSwitchStateFactory beholderSwitchStateFactory)
                beholderSwitchStateFactory.SetBeholderMoveConfig(config);
            return this;
        }
        
        public BeholderSwitchStateFactoryBuilder SetCreepStateFactory(CreepStateFactory creepStateFactory)
        {
            if(factory is BeholderSwitchStateFactory beholderSwitchStateFactory)
                beholderSwitchStateFactory.SetCreepStateFactory(creepStateFactory);
            return this;
        }
    }
}