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
    private PlayerSwitchAttackState switchAttackState;
    private PathFinding pathFinder;
    private Rotation rotationController;

    private GameObject finalTarget;
    private GameObject currentTarget;

    private Vector3 finalTargetPosition;
    private Vector3 currentTargetPosition;
    private Vector3 movementDirection;

    private Vector2Int finalTargetCoordinates;
    private Vector2Int currentTargetCoordinates;
    private Vector2Int previousTargetCoordinates;

    private readonly float checkEnemyCooldown = 0.1f;
    private readonly float checkTargetCooldown = 0.2f;
    private float enemyCheckTimer;
    private float targetCheckTimer;

    private bool isCanRotate;
    private bool shouldCheckEnemy;
    private bool isShouldCheckJump;
    private bool isShouldCheckPath;

    private Queue<CellController> pathQueue = new();

    public SO_PlayerMove SO_PlayerMove { get; set; }
    public PlayerEndurance PlayerEndurance { get; set; }
    public Transform Center { get; set; }
    public CharacterController CharacterController { get; set; }
    public float RotationSpeed { get; set; }
    public float RunDecreaseEndurance { get; set; }

    private PlayerJumpState CreateJumpState()
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
            .SetStateMachine(StateMachine)
            .Build();
    }

    private bool IsFinalPositionValid()
    {
        return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
    }

    public override void Initialize()
    {
        base.Initialize();
        switchAttackState = StateMachine.GetState<PlayerSwitchAttackState>();
        rotationController = new Rotation(GameObject.transform, RotationSpeed);
        pathFinder = new PathFindingBuilder()
            .SetStartPosition(GameObject.transform.position)
            .SetEndPosition(GameObject.transform.position)
            .Build();
    }

    public override void Enter()
    {
        base.Enter();
        enemyCheckTimer = checkEnemyCooldown;

        isShouldCheckJump = !StateMachine.IsActivateType(typeof(PlayerJumpState));
        shouldCheckEnemy = isShouldCheckJump;
        isShouldCheckPath = isShouldCheckJump;

        StateMachine.OnExitCategory += HandleExitCategory;
    }

    public override void Update()
    {
        base.Update();

        if (!currentTarget)
        {
            AssignNewCurrentTarget();
            if (!currentTarget)
            {
                StateMachine.ExitCategory(Category, null);
                return;
            }
        }

        UpdatePath();
        ExecuteMovement();
    }

    public override void LateUpdate()
    {
        CheckForJumpInput();
    }

    public override void Exit()
    {
        base.Exit();
        currentTarget = null;
        StateMachine.OnExitCategory -= HandleExitCategory;
    }

    private void HandleExitCategory(Machine.IState state)
    {
        if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
        {
            PlayAnimation();
            shouldCheckEnemy = true;
            isShouldCheckJump = true;
            isShouldCheckPath = true;
        }
    }

    public void SetTarget(GameObject target)
    {
        currentTarget = null;
        finalTarget = target;
        finalTargetCoordinates = target.GetComponent<CellController>().CurrentCoordinates;
        UpdatePathToTarget();
    }

    private void ResetPathColors()
    {
        var currentCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);
        if (currentCell && currentCell.TryGetComponent(out UnitRenderer renderer))
        {
            if (currentCell.CurrentCoordinates != finalTargetCoordinates)
                renderer.ResetColor();
        }

        while (pathQueue.Count > 0)
        {
            var cell = pathQueue.Dequeue();
            if (cell && cell.TryGetComponent(out UnitRenderer unitRenderer))
            {
                if (cell.CurrentCoordinates != finalTargetCoordinates)
                    unitRenderer.ResetColor();
            }
        }
    }

    private void UpdatePathToTarget(bool compareDistance = false)
    {
        ResetPathColors();
        pathQueue.Clear();
        finalTargetPosition = finalTarget.transform.position;
        pathFinder.SetStartPosition(GameObject.transform.position);
        pathFinder.SetTargetPosition(finalTargetPosition);
        AssignNewCurrentTarget(compareDistance);
    }

    private void AssignNewCurrentTarget(bool compareDistance = false)
    {
        if (pathQueue.Count == 0)
            pathQueue = pathFinder.GetPath(true, compareDistance);

        if (pathQueue.Count > 0)
        {
            currentTarget = pathQueue.Peek()?.gameObject;
            rotationController.SetTarget(currentTarget.transform);
            currentTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
        }
    }

    private void UpdatePath()
    {
        if (!isShouldCheckPath) return;

        targetCheckTimer += Time.deltaTime;
        if (targetCheckTimer < checkTargetCooldown) return;
        targetCheckTimer = 0;

        if (!IsFinalPositionValid())
        {
            UpdatePathToTarget(true);
            return;
        }

        var currentCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);

        if (!currentCell || previousTargetCoordinates == Vector2Int.zero) return;

        if (currentTargetCoordinates == currentCell.CurrentCoordinates || previousTargetCoordinates == currentCell.CurrentCoordinates)
            return;

        UpdatePathToTarget();
    }

    private void CheckIfTargetReached()
    {
        if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, finalTarget.transform.position) || pathQueue.Count == 0)
            StateMachine.ExitCategory(Category, null);
    }

    public override void ExecuteMovement()
    {
        currentTargetPosition = new Vector3(currentTarget.transform.position.x, GameObject.transform.position.y, currentTarget.transform.position.z);

        if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTargetPosition))
        {
            if (currentTarget)
            {
                currentTarget.GetComponent<UnitRenderer>()?.ResetColor();
                previousTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
                currentTarget = null;
                if (pathQueue.Count > 0)
                    pathQueue.Dequeue();
            }

            CheckIfTargetReached();
        }
        else
        {
            if (!Calculate.Move.IsFacingTargetUsingAngle(GameObject.transform.position, GameObject.transform.forward, currentTargetPosition))
            {
                rotationController.Rotate();
                return;
            }

            movementDirection = (currentTargetPosition - GameObject.transform.position).normalized;
            CharacterController.Move(movementDirection * (MovementSpeed * Time.deltaTime));

            ReduceEndurance();
        }
    }

    private void CheckForJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isShouldCheckJump)
        {
            TriggerJump();
        }
    }

    private void TriggerJump()
    {
        if (!StateMachine.IsStateNotNull(typeof(PlayerJumpState)))
        {
            var jumpState = CreateJumpState();
            jumpState.Initialize();
            StateMachine.AddStates(jumpState);
        }
        StateMachine.SetStates(typeof(PlayerJumpState));
        isShouldCheckPath = false;
        shouldCheckEnemy = false;
        isShouldCheckJump = false;
    }

    private void ReduceEndurance()
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