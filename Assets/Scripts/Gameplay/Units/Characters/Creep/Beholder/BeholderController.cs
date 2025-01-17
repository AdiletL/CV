using ScriptableObjects.Unit.Character.Creep;
using UnityEngine;
using Zenject;

namespace Unit.Character.Creep
{
    public class BeholderController : CreepController
    {
        [SerializeField] private Transform finalPath;
        [SerializeField] private SO_BeholderAttack so_BeholderAttack;
        [SerializeField] private SO_BeholderMove so_BeholderMove;

        private BeholderSwitchMove beholderSwitchMove;
        private BeholderSwitchAttack beholderSwitchAttack;


        private BeholderSwitchMove CreateBeholderSwitchMove()
        {
            return (BeholderSwitchMove)new BeholderSwitchSwitchMoveBuilder()
                .SetCharacterController(GetComponent<CharacterController>())
                .SetStart(Calculate.FindCell.GetCell(transform.position, Vector3.down).transform)
                .SetEnd(finalPath)
                .SetCenter(center)
                .SetCharacterAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetConfig(so_BeholderMove)
                .SetRotationSpeed(so_BeholderMove.RotateSpeed)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        private BeholderIdleState CreateBeholderIdleState()
        {
            return (BeholderIdleState)new BeholderIdleStateBuilder()
                .SetCharacterAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetIdleClips(so_BeholderMove.IdleClip)
                .SetCharacterSwitchMove(beholderSwitchMove)
                .SetCharacterSwitchAttack(beholderSwitchAttack)
                .SetCharacterController(GetComponent<CharacterController>())
                .SetCenter(center)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .Build();
        }

        private BeholderSwitchAttack CreateBeholderSwitchAttackState()
        {
            return (BeholderSwitchAttack)new BeholderSwitchAttackBuilder()
                .SetCenter(center)
                .SetCharacterAnimation(GetComponentInUnit<BeholderAnimation>())
                .SetEnemyLayer(so_BeholderAttack.EnemyLayer)
                .SetGameObject(gameObject)
                .SetStateMachine(this.StateMachine)
                .SetConfig(so_BeholderAttack)
                .Build();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            this.StateMachine.Initialize();
            
            this.StateMachine.SetStates(desiredStates:typeof(CreepIdleState));
        }

        protected override void CreateStates()
        {
            var idleState = CreateBeholderIdleState();
            diContainer.Inject(idleState);
            this.StateMachine.AddStates(idleState);
        }

        protected override void CreateSwitchState()
        {
            beholderSwitchMove = CreateBeholderSwitchMove();
            diContainer.Inject(beholderSwitchMove);

            beholderSwitchAttack = CreateBeholderSwitchAttackState();
            diContainer.Inject(beholderSwitchAttack);
            
            beholderSwitchMove.SetSwitchAttack(beholderSwitchAttack);
            beholderSwitchAttack.SetSwitchMove(beholderSwitchMove);
            
            beholderSwitchMove.Initialize();
            beholderSwitchAttack.Initialize();
        }

        public override void Appear()
        {
            this.StateMachine.SetStates(desiredStates:typeof(BeholderIdleState));
        }
    }
}