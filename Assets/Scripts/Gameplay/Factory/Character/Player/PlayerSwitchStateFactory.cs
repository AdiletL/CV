using System;
using Photon.Pun;
using ScriptableObjects.Unit.Character.Player;
using Unit.Character;
using Unit.Character.Player;
using UnityEngine;

namespace Gameplay.Factory.Character.Player
{
    public class PlayerSwitchStateFactory : CharacterSwitchStateFactory
    {
        private CharacterController characterController;
        private StateMachine stateMachine;
        private PhotonView photonView;
        private Transform weaponParent;
        
        private CharacterAnimation characterAnimation;
        private CharacterSwitchMoveState characterSwitchMoveState;
        private CharacterEndurance characterEndurance;
        private CharacterStateFactory characterStateFactory;
        
        private SO_PlayerMove so_PlayerMove;
        private SO_PlayerAttack so_PlayerAttack;
        
        public void SetCharacterState(CharacterStateFactory characterStateFactory) => this.characterStateFactory = characterStateFactory;
        public void SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove) => this.so_PlayerMove = so_PlayerMove;
        public void SetStateMachine(StateMachine stateMachine) => this.stateMachine = stateMachine;
        public void SetCharacterAnimation(CharacterAnimation characterAnimation) => this.characterAnimation = characterAnimation;
        public void SetCharacterEndurance(CharacterEndurance characterEndurance) => this.characterEndurance = characterEndurance;
        public void SetPhotonView(PhotonView view) => this.photonView = view;
        public void SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack) => this.so_PlayerAttack = so_PlayerAttack;
        public void SetWeaponParent(Transform parent) => this.weaponParent = parent;


        public override void Initialize()
        {
            
        }

        public override CharacterSwitchAttackState CreateSwitchAttackState(Type stateType)
        {
            CharacterSwitchAttackState result = stateType switch
            {
                _ when stateType == typeof(PlayerSwitchAttackState) => CreateSwitchAttack(),
                _ => throw new ArgumentException($"Unknown switchState type: {stateType}")
            };
            return result;
        }
        public override CharacterSwitchMoveState CreateSwitchMoveState(Type stateType)
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
                .SetCharacterStateFactory(characterStateFactory)
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
                .SetCharacterStateFactory(characterStateFactory)
                .SetCharacterEndurance(characterEndurance)
                .SetConfig(so_PlayerAttack)
                .SetGameObject(gameObject)
                .SetCenter(unitCenter.Center)
                .SetStateMachine(stateMachine)
                .Build();
        }
    }

    public class PlayerSwitchStateFactoryBuilder : CharacterSwitchStateFactoryBuilder
    {
        public PlayerSwitchStateFactoryBuilder() : base(new PlayerSwitchStateFactory())
        {
            
        }
        
        public PlayerSwitchStateFactoryBuilder SetCharacterState(CharacterStateFactory characterStateFactory)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetCharacterState(characterStateFactory);
            return this;
        }
        
        public PlayerSwitchStateFactoryBuilder SetStateMachine(StateMachine stateMachine)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetStateMachine(stateMachine);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetCharacterAnimation(CharacterAnimation characterAnimation)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetCharacterAnimation(characterAnimation);
            return this;
        }
        
        public PlayerSwitchStateFactoryBuilder SetPhotonView(PhotonView view)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetPhotonView(view);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetCharacterEndurance(CharacterEndurance characterEndurance)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetCharacterEndurance(characterEndurance);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetPlayerMoveConfig(SO_PlayerMove so_PlayerMove)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetPlayerMoveConfig(so_PlayerMove);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetPlayerAttackConfig(SO_PlayerAttack so_PlayerAttack)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetPlayerAttackConfig(so_PlayerAttack);
            return this;
        }

        public PlayerSwitchStateFactoryBuilder SetWeaponParent(Transform parent)
        {
            if(factory is PlayerSwitchStateFactory playerSwitchStateFactory)
                playerSwitchStateFactory.SetWeaponParent(parent);
            return this;
        }
    }
}