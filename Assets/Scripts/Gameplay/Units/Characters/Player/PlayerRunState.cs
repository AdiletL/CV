using System;
using System.Collections.Generic;
using Calculate;
using Movement;
using ScriptableObjects.Unit.Character.Player;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Player
{
    public class PlayerRunState : CharacterRunState
    {
        private PlayerSwitchAttackState playerSwitchAttackState;
        private PathFinding pathFinding;
        private Rotation rotation;

        private GameObject finalTarget;
        private GameObject currentTarget;

        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 direction;
        
        private Vector2Int finalTargetCoordinates;
        private Vector2Int currentTargetCoordinates;
        private Vector2Int previousTargetCoordinates;

        private readonly float cooldownCheckEnemy = 0.1f;
        private readonly float cooldownCheckTarget = .2f;
        private float countCooldownCheckEnemy;
        private float countCooldownCheckTarget;

        private bool isRotate;
        private bool isCheckEnemy;
        private bool isCheckJump;
        private bool isCheckPath;

        private Queue<CellController> pathToPoint = new();

        public SO_PlayerMove SO_PlayerMove { get; set; }
        public PlayerEndurance PlayerEndurance { get; set; }
        public Transform Center { get; set; }
        public CharacterController CharacterController { get; set; }
        public float RotationSpeed { get; set; }
        public float RunDecreaseEndurance { get; set; }

        private PlayerJumpState CreatePlayerJumpState()
        {
            return (PlayerJumpState)new PlayerJumpStateBuilder()
                .SetPlayerEndurance(PlayerEndurance)
                .SetDecreaseEndurance(SO_PlayerMove.JumpDecreaseEndurance)
                .SetMaxJumpCount(SO_PlayerMove.JumpInfo.MaxCount)
                .SetAnimationCurve(SO_PlayerMove.JumpInfo.Curve)
                .SetJumpDuration(SO_PlayerMove.JumpInfo.Duration)
                .SetJumpClip(SO_PlayerMove.JumpInfo.Clip)
                .SetJumpHeight(SO_PlayerMove.JumpInfo.Height)
                .SetGameObject(GameObject)
                .SetCharacterAnimation(CharacterAnimation)
                .SetStateMachine(this.StateMachine)
                .Build();
        }
        
        private bool isFinalPositionCorrect()
        {
            return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
        }

        public override void Initialize()
        {
            base.Initialize();
            playerSwitchAttackState = this.StateMachine.GetState<PlayerSwitchAttackState>();
            rotation = new Rotation(GameObject.transform, RotationSpeed);
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(GameObject.transform.position)
                .SetEndPosition(GameObject.transform.position)
                .Build();
        }

        public override void Enter()
        {
            base.Enter();
            countCooldownCheckEnemy = cooldownCheckEnemy;
            
            isCheckJump = !this.StateMachine.IsActivateType(typeof(PlayerJumpState));
            isCheckEnemy = isCheckJump;
            isCheckPath = isCheckJump;
            
            this.StateMachine.OnExitCategory += OnExitCategory;
        }

        public override void Update()
        {
            base.Update();
            if (!currentTarget)
                NewCurrentTarget();

            if (!currentTarget)
            {
                this.StateMachine.ExitCategory(Category, null);
                return;
            }

            CheckPath();
            Move();
        }   

        public override void LateUpdate()
        {
            CheckJump();
            //CheckEnemy();
        }

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
            this.StateMachine.OnExitCategory -= OnExitCategory;
        }
        
        private void OnExitCategory(Machine.IState state)
        {
            if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
            {
                PlayAnimation();
                isCheckEnemy = true;
                isCheckJump = true;
                isCheckPath = true;
            }
        }
        
        public void SetTarget(GameObject target)
        {
            currentTarget = null;
            finalTarget = target;
            finalTargetCoordinates = target.GetComponent<CellController>().CurrentCoordinates;
            FindNewPathToPoint();
        }
        
        private void ResetColorOnPath()
        {
            var currentCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);
            if (currentCell && currentCell.TryGetComponent(out UnitRenderer renderer))
            {
                if(currentCell.CurrentCoordinates != finalTargetCoordinates)
                    renderer?.ResetColor();
            }
            
            CellController cellController;
            for (int i = pathToPoint.Count - 1; i >= 0; i--)
            {
                cellController = pathToPoint.Dequeue();
                if (cellController && cellController.TryGetComponent(out UnitRenderer unitRenderer))
                {
                    if(cellController.CurrentCoordinates == finalTargetCoordinates)
                        continue;
                    
                    unitRenderer?.ResetColor();
                }
            }
        }

        private void FindNewPathToPoint(bool isCompareDistance = false)
        {
            ResetColorOnPath();
            pathToPoint.Clear();
            finalTargetPosition = finalTarget.transform.position;
            pathFinding.SetStartPosition(GameObject.transform.position);
            pathFinding.SetTargetPosition(finalTargetPosition);
            NewCurrentTarget(isCompareDistance);
        }

        private void NewCurrentTarget(bool isCompareDistance = false)
        {
            if(pathToPoint.Count == 0)
                pathToPoint = pathFinding.GetPath(true, isCompareDistance);
                
            if(pathToPoint.Count == 0) return;
            
            currentTarget = pathToPoint.Peek()?.gameObject;
            rotation.SetTarget(currentTarget.transform);
            currentTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
        }

        private void CheckPath()
        {
            if(!isCheckPath) return;
            
            countCooldownCheckTarget += Time.deltaTime;
            if (countCooldownCheckTarget < cooldownCheckTarget) return;
            countCooldownCheckTarget = 0;

            if (!isFinalPositionCorrect())
            {
                FindNewPathToPoint(true);
                return;
            }
            
            var hitInCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);
            
            if(!hitInCell || previousTargetCoordinates == Vector2Int.zero) return;
            
            if(currentTargetCoordinates == hitInCell.CurrentCoordinates || previousTargetCoordinates == hitInCell.CurrentCoordinates)
                return;

            FindNewPathToPoint();
        }
        
        
        private void CheckFinishedToFinalTarget()
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, finalTarget.transform.position)
                || pathToPoint.Count == 0)
                this.StateMachine.ExitCategory(Category, null);
        }

        
        public override void Move()
        {
            currentTargetPosition = new Vector3(currentTarget.transform.position.x, GameObject.transform.position.y, currentTarget.transform.position.z);

            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTargetPosition))
            {
                if (currentTarget)
                {
                    currentTarget?.GetComponent<UnitRenderer>()?.ResetColor();
                    previousTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
                    currentTarget = null;
                    if(pathToPoint.Count != 0)
                        pathToPoint.Dequeue();
                }

                CheckFinishedToFinalTarget();
            }
            else
            {
                if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform.position, GameObject.transform.forward, currentTargetPosition))
                {
                    rotation.Rotate();
                    return;
                }
                
                direction = (currentTargetPosition - GameObject.transform.position).normalized;
                CharacterController.Move(direction * (MovementSpeed * Time.deltaTime));

                DecreaseEndurance();
            }
        }

 
        private void CheckEnemy()
        {
            if(!isCheckEnemy) return;
            
            countCooldownCheckEnemy += Time.deltaTime;
            if (countCooldownCheckEnemy > cooldownCheckEnemy)
            {
                if (playerSwitchAttackState.IsFindUnitInRange())
                {
                    foreach (var VARIABLE in pathToPoint)
                        VARIABLE.SetColor(Color.white);
                    
                    this.StateMachine.ExitCategory(Category, typeof(PlayerSwitchAttackState));
                }
                countCooldownCheckEnemy = 0;
            }
        }

        private void CheckJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isCheckJump)
            {
                StartJump();
            }
        }
        private void StartJump()
        {
            if (!this.StateMachine.IsStateNotNull(typeof(PlayerJumpState)))
            {
                var playerJumpState = CreatePlayerJumpState();           
                playerJumpState.Initialize();
                this.StateMachine.AddStates(playerJumpState);
            }
            this.StateMachine.SetStates(typeof(PlayerJumpState));
            isCheckPath = false;
            isCheckEnemy = false;
            isCheckJump = false;
        }

        private void DecreaseEndurance()
        {
            PlayerEndurance.RemoveEndurance(RunDecreaseEndurance);
        }
    }

    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
        
        
        public PlayerRunStateBuilder SetCenter(Transform center)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.Center = center;
            
            return this;
        }

        public PlayerRunStateBuilder SetMoveConfig(SO_PlayerMove config)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SO_PlayerMove = config;
            
            return this;  
        }
        public PlayerRunStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.CharacterController = characterController;
            
            return this;  
        }
        public PlayerRunStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.RotationSpeed = rotationSpeed;
            
            return this;  
        }
        public PlayerRunStateBuilder SetRunDecreaseEndurance(float value)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.RunDecreaseEndurance = value;
            
            return this;  
        }
        public PlayerRunStateBuilder SetPlayerEndurance(PlayerEndurance endurance)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.PlayerEndurance = endurance;
            
            return this;  
        }
    }
}