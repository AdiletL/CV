﻿using System;
using Gameplay.Ability;
using Gameplay.Unit.Character;
using Gameplay.Unit.Character.Player;
using Machine;
using Photon.Pun;
using ScriptableObjects.Unit.Character;
using ScriptableObjects.Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory.Character.Player
{
    public class PlayerStateFactory : CharacterStateFactory
    {
        private PlayerKinematicControl playerKinematicControl;
        private PlayerItemInventory playerItemInventory;
        private UnitRenderer unitRenderer;
        private StateMachine stateMachine;
        private PhotonView photonView;
        private Camera baseCamera;
        
        private CharacterAnimation characterAnimation;
        private CharacterStatsController characterStatsController;
        private AbilityHandler abilityHandler;
        
        private SO_PlayerMove so_PlayerMove;
        private SO_PlayerAttack so_PlayerAttack;
        private SO_PlayerSpecialAction so_PlayerSpecialAction;
        private SO_PlayerItemUsage so_PlayerItemUsage;
        private SO_PlayerAbilityUsage so_PlayerAbilityUsage;
        private SO_PlayerDisable so_PlayerDisable;
        
        public void SetUnitRenderer(UnitRenderer unitRenderer) => this.unitRenderer = unitRenderer;
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetPlayerItemInventory(PlayerItemInventory itemInventory) => this.playerItemInventory = itemInventory;
        public void SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove) => this.so_PlayerMove = so_PlayerMove;
        public void SetPlayerDisableConfig(SO_PlayerDisable playerDisable) => this.so_PlayerDisable = playerDisable;
        public void SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack) => this.so_PlayerAttack = so_PlayerAttack;
        public void SetPlayerSpecialAction(SO_PlayerSpecialAction so_PlayerSpecialAction) => this.so_PlayerSpecialAction = so_PlayerSpecialAction;
        public void SetPlayerItemUsageConfig(SO_PlayerItemUsage so_PlayerItemUsage) => this.so_PlayerItemUsage = so_PlayerItemUsage;
        public void SetPlayerAbilityUsageConfig(SO_PlayerAbilityUsage so_PlayerAbilityUsage) => this.so_PlayerAbilityUsage = so_PlayerAbilityUsage;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterStatsController(CharacterStatsController characterStatsController) => this.characterStatsController = characterStatsController;
        public void SetAbilityHandler(AbilityHandler abilityHandler) => this.abilityHandler = abilityHandler;
        public void SetPhotonView(PhotonView view) => photonView = view;
        public void SetBaseCamera(Camera camera) => baseCamera = camera;


        public override State CreateState(Type stateType)
        {
            State result = stateType switch
            {
                _ when stateType == typeof(PlayerIdleState) => CreateIdleState(),
                _ when stateType == typeof(PlayerAttackState) => CreateWeaponState(),
                _ when stateType == typeof(PlayerMoveToTargetState) => CreateRunState(),
                _ when stateType == typeof(PlayerMoveState) => CreateRunStateOrig(),
                _ when stateType == typeof(PlayerJumpState) => CreateJumpState(),
                _ when stateType == typeof(PlayerSpecialActionState) => CreateSpecialActionState(),
                _ when stateType == typeof(PlayerItemUsageState) => CreateItemUsageState(),
                _ when stateType == typeof(PlayerAbilityUsageState) => CreateAbilityUsageState(),
                _ when stateType == typeof(PlayerDisableState) => CreateDisableState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        private PlayerIdleState CreateIdleState()
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerAttackState CreateWeaponState()
        {
            var result = (PlayerAttackState)new PlayerAttackStateBuilder()
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetBaseCamera(baseCamera)
                .SetUnitAnimation(characterAnimation)
                .SetAbilityHandler(abilityHandler)
                .SetUnitRenderer(unitRenderer)
                .SetCenter(unitCenter.Center)
                .SetConfig(so_PlayerAttack)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
            return result;
        }
        
        private PlayerMoveToTargetState CreateRunState()
        {
            var result = (PlayerMoveToTargetState)new PlayerMoveToTargetStateBuilder()
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetRunReductionEndurance(so_PlayerMove.ConsumptionEnduranceRate)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            result.MovementSpeedStat.AddCurrentValue(so_PlayerMove.RunSpeed);
            return result;
        }
        
        private PlayerMoveState CreateRunStateOrig()
        {
            var result = (PlayerMoveState)new PlayerMoveStateBuilder()
                .SetCharacterStatsController(characterStatsController)
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetConsumptionEnduranceRate(so_PlayerMove.ConsumptionEnduranceRate)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            return result;
        }
        
        private PlayerJumpState CreateJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetCharacterStatsController(characterStatsController)
                .SetGameObject(gameObject)
                .SetConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerSpecialActionState CreateSpecialActionState()
        {
            return (PlayerSpecialActionState)new PlayerSpecialActionStateBuilder()
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerSpecialAction)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerItemUsageState CreateItemUsageState()
        {
            return (PlayerItemUsageState)new PlayerItemUsageStateBuilder()
                .SetInventory(playerItemInventory)
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerItemUsage)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerAbilityUsageState CreateAbilityUsageState()
        {
            return (PlayerAbilityUsageState)new PlayerAbilityUsageStateBuilder()
                .SetConfig(so_PlayerAbilityUsage)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerDisableState CreateDisableState()
        {
            return (PlayerDisableState)new PlayerDisableStateBuilder()
                .SetConfig(so_PlayerDisable)
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class PlayerStateFactoryBuilder : CharacterStateFactoryBuilder
    {
        public PlayerStateFactoryBuilder() : base(new PlayerStateFactory())
        {
        }

        public PlayerStateFactoryBuilder SetUnitRenderer(UnitRenderer unitRenderer)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetUnitRenderer(unitRenderer);
            return this;
        }
        public PlayerStateFactoryBuilder SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerMoveConfig(so_PlayerMove);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerAttackConfig(so_PlayerAttack);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetPlayerDisableConfig(SO_PlayerDisable so_PlayerDisable)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerDisableConfig(so_PlayerDisable);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetPlayerSpecialActionConfig(SO_PlayerSpecialAction so_PlayerSpecialAction)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerSpecialAction(so_PlayerSpecialAction);
            return this;
        }

        public PlayerStateFactoryBuilder SetPlayerItemInventory(PlayerItemInventory playerItemInventory)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerItemInventory(playerItemInventory);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetStateMachine(stateMachine);
            return this;
        }

        public PlayerStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetCharacterStatsController(CharacterStatsController characterStatsController)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetCharacterStatsController(characterStatsController);
            return this;
        }

        public PlayerStateFactoryBuilder SetPhotonView(PhotonView view)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPhotonView(view);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerKinematicControl(playerKinematicControl);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetBaseCamera(Camera camera)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetBaseCamera(camera);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetAbilityHandler(AbilityHandler abilityHandler)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetAbilityHandler(abilityHandler);
            return this;
        }
    }
}