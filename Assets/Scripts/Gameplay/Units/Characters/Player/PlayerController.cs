using Gameplay.Damage;
using Gameplay.Effect;
using Gameplay.Skill;
using Gameplay.Weapon;
using Machine;
using ScriptableObjects.Unit.Character.Player;
using ScriptableObjects.Weapon;
using Unit.Character.Player.Unit.Character.Player;
using Unity.Collections;
using UnityEngine;

namespace Unit.Character.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerEndurance))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(PlayerExperience))]
    [RequireComponent(typeof(PlayerGravity))]
    [RequireComponent(typeof(HandleEffect))]
    [RequireComponent(typeof(HandleSkill))]
    
    public class PlayerController : CharacterMainController
    {
        [SerializeField] private SO_PlayerMove so_PlayerMove;
        [SerializeField] private SO_PlayerAttack so_PlayerAttack;
        [SerializeField] private SO_PlayerControlDesktop so_PlayerControlDesktop;
        
        [Space]
        [SerializeField] private SO_Sword so_Sword;
        [SerializeField] private SO_Bow so_Bow;
        [SerializeField] private Transform weaponParent;
        
        [ReadOnly] public StateCategory currentStateCategory;
        [ReadOnly] public string currentStateName;

        private PlayerControlDesktop playerControlDesktop;
        private PlayerSwitchMove playerSwitchMove;
        private PlayerSwitchAttack playerSwitchAttack;
        private GameObject finish;
        
        private CharacterController characterController;
        
        private PlayerIdleState CreateIdleState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerIdleState)new PlayerIdleStateBuilder()
                .SetMoveConfig(so_PlayerMove)
                .SetPlayerEndurance(GetComponentInUnit<PlayerEndurance>())
                .SetFinishTargetToMove(finish)
                .SetIdleClips(so_PlayerMove.IdleClip)
                .SetCharacterSwitchMove(playerSwitchMove)
                .SetCharacterController(GetComponentInUnit<CharacterController>())
                .SetCharacterAnimation(characterAnimation)
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchMove CreateSwitchMoveState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchMove)new PlayerSwitchMoveBuilder()
                .SetCharacterController(characterController)
                .SetCenter(center)
                .SetPlayerEndurance(GetComponentInUnit<PlayerEndurance>())
                .SetCharacterAnimation(characterAnimation)
                .SetConfig(so_PlayerMove)
                .SetGameObject(gameObject)
                .SetRotationSpeed(so_PlayerMove.RotateSpeed)
                .SetStateMachine(StateMachine)
                .Build();
        }

        private PlayerSwitchAttack CreateSwitchAttackState(CharacterAnimation characterAnimation, Transform center)
        {
            return (PlayerSwitchAttack)new PlayerSwitchAttackBuilder()
                .SetWeaponParent(weaponParent)
                .SetCenter(center)
                .SetCharacterEndurance(GetComponentInUnit<PlayerEndurance>())
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetGameObject(gameObject)
                .SetCharacterAnimation(characterAnimation)
                .SetStateMachine(StateMachine)
                .SetConfig(so_PlayerAttack)
                .Build();
        }

        private PlayerControlDesktop CreatePlayerControlDesktop()
        {
            return (PlayerControlDesktop)new PlayerControlDesktopBuilder()
                .SetCharacterController(GetComponentInUnit<CharacterController>())
                .SetGameObject(gameObject)
                .SetPlayerSwitchAttack(playerSwitchAttack)
                .SetPlayerSwitchMove(playerSwitchMove)
                .SetHandleSkill(GetComponentInUnit<HandleSkill>())
                .SetPlayerMoveConfig(so_PlayerMove)
                .SetPlayerControlDesktopConfig(so_PlayerControlDesktop)
                .SetEnemyLayer(so_PlayerAttack.EnemyLayer)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        public override void Initialize()
        {
            characterController = GetComponent<CharacterController>();

            base.Initialize();

            playerControlDesktop = CreatePlayerControlDesktop();
            diContainer.Inject(playerControlDesktop);
            playerControlDesktop.Initialize();
            
            StateMachine.Initialize();
            
            TestWeapon();
                        
            StateMachine.OnChangedState += OnChangedState;
            StateMachine.SetStates(desiredStates: typeof(PlayerIdleState));
        }

        protected override void CreateStates()
        {
            var characterAnimation = GetComponentInUnit<CharacterAnimation>();
            var center = GetComponentInUnit<UnitCenter>().Center;

            var idleState = CreateIdleState(characterAnimation, center);
            diContainer.Inject(idleState);
            
            StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            var characterAnimation = GetComponentInUnit<CharacterAnimation>();
            var center = GetComponentInUnit<UnitCenter>().Center;

            playerSwitchMove = CreateSwitchMoveState(characterAnimation, center);
            diContainer.Inject(playerSwitchMove);
            
            playerSwitchAttack = CreateSwitchAttackState(characterAnimation, center);
            diContainer.Inject(playerSwitchAttack);
            
            playerSwitchMove.SetSwitchAttack(playerSwitchAttack);
            playerSwitchAttack.SetSwitchMove(playerSwitchMove);
            
            playerSwitchAttack.Initialize();
            playerSwitchMove.Initialize();
        }

        
        public override void Appear()
        {
            
        }

        private void TestWeapon()
        {
            //TEST
            var swordDamageable = new NormalDamage(so_Sword.Damage, gameObject);
            diContainer.Inject(swordDamageable);
            var sword = (Sword)new SwordBuilder()
                .SetDecreaseEndurance(so_Sword.DecreaseEndurance)
                .SetAngleToTarget(so_Sword.DecreaseEndurance)
                .SetWeaponParent(weaponParent)
                .SetGameObject(gameObject)
                .SetRange(so_Sword.Range)
                .SetWeaponPrefab(so_Sword.WeaponPrefab)
                .SetDamageable(swordDamageable)
                .Build();
            sword.Initialize();
            SetWeapon(sword);

            /*var projectile = new NormalDamage(so_Bow.Damage, gameObject);
            var bow = (Bow)new BowBuilder()
                .SetDamageable(projectile)
                .SetRange(so_Bow.Range)
                .SetAmountAttack(so_Bow.AmountAttack)
                .SetWeaponParent(weaponParent)
                .SetWeaponPrefab(so_Bow.WeaponPrefab)
                .Build();
            diContainer.Inject(bow);
            bow.Initialize();
            SetWeapon(bow);*/
        }

        private void Update()
        {
            playerControlDesktop?.HandleHotkey();
            StateMachine?.Update();
        }

        private void LateUpdate()
        {
            playerControlDesktop?.HandleInput();
            StateMachine?.LateUpdate();
        }
        
        public void SetWeapon(Weapon weapon)
        {
            playerSwitchAttack.SetWeapon(weapon);
        }

        public void IncreaseWeaponStates()
        {
            
        }
        public void SetFinishTarget(GameObject target)
        { 
            finish = target;
            this.StateMachine?.GetState<PlayerIdleState>()?.SetTarget(finish);
        }

        private void OnChangedState(Machine.IState state)
        {
            currentStateCategory = state.Category;
            currentStateName = state.GetType().Name;
        }

        protected override void OnDestroy()
        {
            StateMachine.OnChangedState -= OnChangedState;
        }
    }
}
