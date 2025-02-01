using System;
using Photon.Pun;
using ScriptableObjects.Unit.Character.Player;
using Unit;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Factory
{
    public class PlayerSwitchStateFactory : Factory
    {
        private GameObject gameObject;
        private CharacterController characterController;
        private StateMachine stateMachine;
        private UnitCenter unitCenter;
        private PhotonView photonView;
        private Transform weaponParent;
        
        private CharacterAnimation characterAnimation;
        private CharacterSwitchMoveState characterSwitchMoveState;
        private CharacterEndurance characterEndurance;
        private PlayerStateFactory playerStateFactory;
        
        private SO_PlayerMove so_PlayerMove;
        private SO_PlayerAttack so_PlayerAttack;
        
        public void SetPlayerState(PlayerStateFactory playerStateFactory) => this.playerStateFactory = playerStateFactory;
        public void SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove) => this.so_PlayerMove = so_PlayerMove;
        public void SetUnitCenter(UnitCenter unitCenter) => this.unitCenter = unitCenter;
        public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
        public void SetCharacterSwitchMove(CharacterSwitchMoveState characterSwitchMoveState) => this.characterSwitchMoveState = characterSwitchMoveState;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;
        public void SetPhotonView(PhotonView view) => this.photonView = view;
        public void SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack) => this.so_PlayerAttack = so_PlayerAttack;
        public void SetWeaponParent(Transform parent) => this.weaponParent = parent;
        
        
        
        public CharacterSwitchAttackState CreateSwitchAttackState(Type stateType)
        {
            CharacterSwitchAttackState result = stateType switch
            {
                _ when stateType == typeof(PlayerSwitchAttackState) => CreateSwitchAttack(),
                _ => throw new ArgumentException($"Unknown switchState type: {stateType}")
            };
            return result;
        }
        public CharacterSwitchMoveState CreateSwitchMoveState(Type stateType)
        {
            CharacterSwitchMoveState result = stateType switch
            {
                _ when stateType == typeof(PlayerSwitchMoveState) => CreateSwitchMove(),
                _ => throw new ArgumentException($"Unknown switchState type: {stateType}")
            };
            return result;
        }
        
        private PlayerSwitchMoveState CreateSwitchMove()
        {
            return (PlayerSwitchMoveState)new PlayerSwitchMoveStateBuilder()
                .SetPlayerStateFactory(playerStateFactory)
                .SetConfig(so_PlayerMove)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetEndurance(characterEndurance)
                .SetUnitAnimation(characterAnimation)
                .SetCenter(unitCenter.Center)
                .SetGameObject(gameObject)
                .SetStateMachine(stateMachine)
                .Build();
        }
        
        private PlayerSwitchAttackState CreateSwitchAttack()
        {
            return (PlayerSwitchAttackState)new PlayerSwitchAttackStateBuilder()
                .SetPlayerStateFactory(playerStateFactory)
                .SetCharacterEndurance(characterEndurance)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetConfig(so_PlayerAttack)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class PlayerSwitchStateFactoryBuilder
    {
        private PlayerSwitchStateFactory playerSwitchStateFactory;

        public PlayerSwitchStateFactoryBuilder(PlayerSwitchStateFactory playerSwitchStateFactory)
        {
            this.playerSwitchStateFactory = playerSwitchStateFactory;
        }

        public PlayerSwitchStateFactoryBuilder SetCharacterController(CharacterController characterController)
        {
            playerSwitchStateFactory.SetCharacterController(characterController);
            return this;
        }
        public PlayerSwitchStateFactoryBuilder SetPlayerState(PlayerStateFactory playerStateFactory)
        {
            playerSwitchStateFactory.SetPlayerState(playerStateFactory);
            return this;
        }
        public PlayerSwitchStateFactoryBuilder SetGameObject(GameObject gameObject)
        {
            playerSwitchStateFactory.SetGameObject(gameObject);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            playerSwitchStateFactory.SetStateMachine(stateMachine);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            playerSwitchStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetUnitCenter(UnitCenter unitCenter)
        {
            playerSwitchStateFactory.SetUnitCenter(unitCenter);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetPhotonView(PhotonView view)
        {
            playerSwitchStateFactory.SetPhotonView(view);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            playerSwitchStateFactory.SetCharacterEndurance(characterEndurance);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove)
        {
            playerSwitchStateFactory.SetPlayerMoveConfig(so_PlayerMove);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack)
        {
            playerSwitchStateFactory.SetPlayerAttackConfig(so_PlayerAttack);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetWeaponParent(Transform parent)
        {
            playerSwitchStateFactory.SetWeaponParent(parent);
            return this;
        }

        public PlayerSwitchStateFactory Build()
        {
            return playerSwitchStateFactory;
        }
    }
}