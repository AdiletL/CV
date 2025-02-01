using System;
using Machine;
using Photon.Pun;
using ScriptableObjects.Gameplay;
using ScriptableObjects.Unit.Character.Player;
using Unit;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Factory
{
    public class PlayerStateFactory : Factory
    {
        [Inject] private SO_GameHotkeys so_GameHotkeys;
        
        private GameObject gameObject;
        private CharacterController characterController;
        private StateMachine stateMachine;
        private UnitCenter unitCenter;
        private PhotonView photonView;
        private Transform weaponParent;
        
        private CharacterAnimation characterAnimation;
        private CharacterEndurance characterEndurance;
        private CharacterSwitchMoveState characterSwitchMoveState;
        private CharacterSwitchAttackState characterSwitchAttackState;
        
        private SO_PlayerMove so_PlayerMove;
        private SO_PlayerAttack so_PlayerAttack;
        
        public void SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove) => this.so_PlayerMove = so_PlayerMove;
        public void SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack) => this.so_PlayerAttack = so_PlayerAttack;
        public void SetUnitCenter(UnitCenter unitCenter) => this.unitCenter = unitCenter;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
        public void SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMoveState = characterSwitchMoveState;
        public void SetCharacterSwitchAttack(CharacterSwitchAttackState characterSwitchAttackState) => this.characterSwitchAttackState = characterSwitchAttackState;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;
        public void SetPhotonView(PhotonView view) => photonView = view;
        public void SetWeaponParent(Transform parent) => weaponParent = parent;
        
        
        public State CreateState(Type stateType)
        {
            State result = stateType switch
            {
                _ when stateType == typeof(PlayerIdleState) => CreateIdleState(),
                _ when stateType == typeof(PlayerDefaultAttackState) => CreateDefaultAttackState(),
                _ when stateType == typeof(PlayerWeaponAttackState) => CreateWeaponState(),
                _ when stateType == typeof(PlayerRunToTargetState) => CreateRunState(),
                _ when stateType == typeof(PlayerRunState) => CreateRunStateOrig(),
                _ when stateType == typeof(PlayerJumpState) => CreateJumpState(),
                _ => throw new ArgumentException($"Unknown state type: {stateType}")
            };
            
            return result;
        }
        
        private PlayerIdleState CreateIdleState()
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetCharacterSwitchMove(characterSwitchMoveState)
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerDefaultAttackState CreateDefaultAttackState()
        {
            return (PlayerDefaultAttackState)new PlayerDefaultAttackStateBuilder()
                .SetSwitchMoveState(characterSwitchMoveState)
                .SetUnitAnimation(characterAnimation)
                .SetAttackClips(so_PlayerAttack.DefaultAttackClips)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetCooldownClip(so_PlayerAttack.DefaultCooldownClip)
                .SetRange(so_PlayerAttack.Range)
                .SetDamage(so_PlayerAttack.Damage)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetAttackSpeed(so_PlayerAttack.AttackSpeed)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerWeaponAttackState CreateWeaponState()
        {
            return (PlayerWeaponAttackState)new PlayerWeaponAttackStateStateBuilder()
                .SetSwordAttackClip(so_PlayerAttack.SwordAttackClip)
                .SetSwordCooldownClip(so_PlayerAttack.SwordCooldownClip)
                .SetBowAttackClip(so_PlayerAttack.BowAttackClip)
                .SetBowCooldownClip(so_PlayerAttack.BowCooldownClip)
                .SetUnitAnimation(characterAnimation)
                .SetWeaponParent(weaponParent)
                .SetUnitEndurance(characterEndurance)
                .SetBaseReductionEndurance(so_PlayerAttack.BaseReductionEndurance)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetCharacterSwitchMoveState(characterSwitchMoveState)
                .SetCenter(unitCenter.Center)
                .SetDamage(so_PlayerAttack.Damage)
                .SetGameObject(gameObject)
                .SetAttackSpeed(so_PlayerAttack.AttackSpeed)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerRunToTargetState CreateRunState()
        {
            return (PlayerRunToTargetState)new PlayerRunToTargetStateBuilder()
                .SetCharacterController(characterController)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetRunReductionEndurance(so_PlayerMove.BaseRunReductionEndurance)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetUnitEndurance(characterEndurance)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerRunState CreateRunStateOrig()
        {
            return (PlayerRunState)new PlayerRunStateBuilder()
                .SetCharacterController(characterController)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetReductionEndurance(so_PlayerMove.BaseRunReductionEndurance)
                .SetPhotonView(photonView)
                .SetUnitAnimation(characterAnimation)
                .SetRunClips(so_PlayerMove.RunClip)
                .SetUnitEndurance(characterEndurance)
                .SetGameObject(gameObject)
                .SetMovementSpeed(so_PlayerMove.RunSpeed)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerJumpState CreateJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetEndurance(characterEndurance)
                .SetJumpKey(so_GameHotkeys.JumpKey)
                .SetReductionEndurance(so_PlayerMove.JumpInfo.BaseReductionEndurance)
                .SetCharacterController(characterController)
                .SetMaxJumpCount(so_PlayerMove.JumpInfo.MaxCount)
                .SetJumpClip(so_PlayerMove.JumpInfo.Clip)
                .SetJumpHeight(so_PlayerMove.JumpInfo.Height)
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class PlayerStateFactoryBuilder
    {
        private PlayerStateFactory playerStateFactory;

        public PlayerStateFactoryBuilder(PlayerStateFactory playerStateFactory)
        {
            this.playerStateFactory = playerStateFactory;
        }

        public PlayerStateFactoryBuilder SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove)
        {
            playerStateFactory.SetPlayerMoveConfig(so_PlayerMove);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack)
        {
            playerStateFactory.SetPlayerAttackConfig(so_PlayerAttack);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetUnitCenter(UnitCenter unitCenter)
        {
            playerStateFactory.SetUnitCenter(unitCenter);
            return this;
        }

        public PlayerStateFactoryBuilder SetGameObject(GameObject gameObject)
        {
            playerStateFactory.SetGameObject(gameObject);
            return this;
        }

        public PlayerStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            playerStateFactory.SetStateMachine(stateMachine);
            return this;
        }

        public PlayerStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            playerStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }

        public PlayerStateFactoryBuilder SetCharacterController(CharacterController characterController)
        {
            playerStateFactory.SetCharacterController(characterController);
            return this;
        }

        public PlayerStateFactoryBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            playerStateFactory.SetCharacterEndurance(characterEndurance);
            return this;
        }

        public PlayerStateFactoryBuilder SetPhotonView(PhotonView view)
        {
            playerStateFactory.SetPhotonView(view);
            return this;
        }
        
        public PlayerStateFactoryBuilder SetWeaponParent(Transform parent)
        {
            playerStateFactory.SetWeaponParent(parent);
            return this;
        }

        public PlayerStateFactory Build()
        {
            return playerStateFactory;
        }
    }
}