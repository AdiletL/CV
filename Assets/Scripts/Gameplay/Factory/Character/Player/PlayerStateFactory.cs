using System;
using Machine;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory.Character.Player
{
    public class PlayerStateFactory : CharacterStateFactory
    {
        private PlayerKinematicControl playerKinematicControl;
        private UnitRenderer unitRenderer;
        private StateMachine stateMachine;
        private PhotonView photonView;
        private Transform weaponParent;
        private Camera baseCamera;
        
        private CharacterAnimation characterAnimation;
        private CharacterEndurance characterEndurance;
        private CharacterSwitchMoveState characterSwitchMoveState;
        private CharacterSwitchAttackState characterSwitchAttackState;
        
        private SO_PlayerMove so_PlayerMove;
        private SO_PlayerAttack so_PlayerAttack;
        private SO_PlayerSpecialAction so_PlayerSpecialAction;
        
        public void SetUnitRenderer(UnitRenderer unitRenderer) => this.unitRenderer = unitRenderer;
        public void SetPlayerKinematicControl(PlayerKinematicControl playerKinematicControl) => this.playerKinematicControl = playerKinematicControl;
        public void SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove) => this.so_PlayerMove = so_PlayerMove;
        public void SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack) => this.so_PlayerAttack = so_PlayerAttack;
        public void SetPlayerSpecialAction(SO_PlayerSpecialAction so_PlayerSpecialAction) => this.so_PlayerSpecialAction = so_PlayerSpecialAction;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMoveState = characterSwitchMoveState;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) => this.characterSwitchAttackState = characterSwitchAttackState;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;
        public void SetPhotonView(PhotonView view) => photonView = view;
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        public void SetBaseCamera(Camera camera) => baseCamera = camera;


        public override void Initialize()
        {
            
        }
        
        public override State CreateState(Type stateType)
        {
            State result = stateType switch
            {
                _ when stateType == typeof(PlayerIdleState) => CreateIdleState(),
                _ when stateType == typeof(PlayerDefaultAttackState) => CreateDefaultAttackState(),
                _ when stateType == typeof(PlayerWeaponAttackState) => CreateWeaponState(),
                _ when stateType == typeof(PlayerRunToTargetState) => CreateRunState(),
                _ when stateType == typeof(PlayerRunState) => CreateRunStateOrig(),
                _ when stateType == typeof(PlayerJumpState) => CreateJumpState(),
                _ when stateType == typeof(PlayerSpecialActionState) => CreateSpecialActionState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        private PlayerIdleState CreateIdleState()
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetCharacterSwitchMove(characterSwitchMoveState)
                .SetConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerDefaultAttackState CreateDefaultAttackState()
        {
            var result = (PlayerDefaultAttackState)new PlayerDefaultAttackStateBuilder()
                .SetSwitchMoveState(characterSwitchMoveState)
                .SetUnitAnimation(characterAnimation)
                .SetAttackClips(so_PlayerAttack.DefaultAttackClips)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetCooldownClip(so_PlayerAttack.DefaultCooldownClip)
                .SetApplyDamageMoment(so_PlayerAttack.ApplyDamageMoment)
                .SetDamage(so_PlayerAttack.Damage)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetAttackSpeed(so_PlayerAttack.AttackSpeed)
                .SetStateMachine(stateMachine)
                .Build();
            result.RangeStat.AddValue(so_PlayerAttack.Range);
            return result;
        }
        
        private PlayerWeaponAttackState CreateWeaponState()
        {
            var result = (PlayerWeaponAttackState)new PlayerWeaponAttackStateStateBuilder()
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetBaseCamera(baseCamera)
                .SetUnitAnimation(characterAnimation)
                .SetWeaponParent(weaponParent)
                .SetUnitEndurance(characterEndurance)
                .SetUnitRenderer(unitRenderer)
                .SetCenter(unitCenter.Center)
                .SetConfig(so_PlayerAttack)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
            result.DamageStat.AddValue(so_PlayerAttack.Damage);
            result.ReduceEnduranceStat.AddValue(so_PlayerAttack.BaseReductionEndurance);
            result.AttackSpeedStat.AddValue(100);
            result.RotationSpeed.AddValue(so_PlayerAttack.RotationSpeed);
            result.RangeStat.AddValue(so_PlayerAttack.Range);
            return result;
        }
        
        private PlayerRunToTargetState CreateRunState()
        {
            var result =(PlayerRunToTargetState)new PlayerRunToTargetStateBuilder()
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetRunReductionEndurance(so_PlayerMove.BaseRunReductionEndurance)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetUnitEndurance(characterEndurance)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            result.MovementSpeedStat.AddValue(so_PlayerMove.RunSpeed);
            return result;
        }
        
        private PlayerRunState CreateRunStateOrig()
        {
            var result = (PlayerRunState)new PlayerRunStateBuilder()
                .SetPlayerKinematicControl(playerKinematicControl)
                .SetReductionEndurance(so_PlayerMove.BaseRunReductionEndurance)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetConfig(so_PlayerMove)
                .SetUnitEndurance(characterEndurance)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
            result.MovementSpeedStat.AddValue(so_PlayerMove.RunSpeed);
            result.RotationSpeedStat.AddValue(so_PlayerMove.RotateSpeed);
            return result;
        }
        
        private PlayerJumpState CreateJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetEndurance(characterEndurance)
                .SetGameObject(gameObject)
                .SetConfig(so_PlayerMove)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
        }

        private PlayerSpecialActionState CreateSpecialActionState()
        {
            return (PlayerSpecialActionState)new PlayerSpecialActionStateBuilder()
                .SetBlockClip(so_PlayerSpecialAction.AbilityConfigData.BlockPhysicalDamageConfig.BlockClip)
                .SetCharacterAnimation(characterAnimation)
                .SetBlockPhysicalDamageConfig(so_PlayerSpecialAction.AbilityConfigData.BlockPhysicalDamageConfig)
                .SetGameObject(gameObject)
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
        
        public PlayerStateFactoryBuilder SetPlayerSpecialActionConfig(SO_PlayerSpecialAction so_PlayerSpecialAction)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPlayerSpecialAction(so_PlayerSpecialAction);
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
        
        public PlayerStateFactoryBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetCharacterEndurance(characterEndurance);
            return this;
        }

        public PlayerStateFactoryBuilder SetPhotonView(PhotonView view)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetPhotonView(view);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetWeaponParent(Transform parent)
        {
            if(factory is PlayerStateFactory playerStateFactory)
                playerStateFactory.SetWeaponParent(parent);
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
    }
}