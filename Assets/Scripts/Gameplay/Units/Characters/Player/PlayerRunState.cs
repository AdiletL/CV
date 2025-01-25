using System.Collections.Generic;
using Calculate;
using Movement;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Player
{
  public class PlayerRunState : CharacterRunState
{
    private PlayerSwitchAttack _playerSwitchAttack;
    private PathFinding pathFinder;
    private Rotation rotationController;
    private CellController finalCell;

    private GameObject finalTarget;
    private GameObject currentTarget;
    private GameObject enemy;

    private Vector3 finalTargetPosition;
    private Vector3 currentTargetPosition;
    private Vector3 movementDirection;

    private Vector2Int currentTargetCoordinates;
    private Vector2Int previousTargetCoordinates;

    private readonly float checkEnemyCooldown = 0.2f;
    private readonly float checkTargetCooldown = 0.2f;
    private float countCooldownheckEnemy;
    private float countCooldownCheckTarget;
    
    private bool isCanRotate;
    private bool isCheckPath;
    private bool isCheckAttack;

    private Queue<CellController> pathQueue = new();

    public PlayerEndurance PlayerEndurance { get; set; }
    public Transform Center { get; set; }
    public CharacterController CharacterController { get; set; }
    public float RotationSpeed { get; set; }
    public float RunReductionEndurance { get; set; }
    

    private bool IsFinalPositionValid()
    {
        return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
    }

    public override void Initialize()
    {
        base.Initialize();
        _playerSwitchAttack = (PlayerSwitchAttack)CharacterSwitchAttack;
        rotationController = new Rotation(GameObject.transform, RotationSpeed);
        pathFinder = new PathFindingBuilder()
            .SetStartPosition(GameObject.transform.position)
            .SetEndPosition(GameObject.transform.position)
            .Build();
    }

    public override void Enter()
    {
        base.Enter();
        
        countCooldownheckEnemy = checkEnemyCooldown;

        isCheckAttack = !StateMachine.IsActivateType(typeof(PlayerJumpState));
        isCheckPath = !StateMachine.IsActivateType(typeof(PlayerJumpState));
        UpdatePathToTarget();
        UpdateValuesCheckEnemy();
    }

    public override void Subscribe()
    {
        base.Subscribe();
        StateMachine.OnExitCategory += OnExitCategory;
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
        base.LateUpdate();
        CheckEnemy();
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        StateMachine.OnExitCategory -= OnExitCategory;
    }

    public override void Exit()
    {
        base.Exit();
        ClearColorsToPath();
        currentTarget = null;
        enemy = null;
        finalTarget = null;
    }

    private void OnExitCategory(Machine.IState state)
    {
        if(!isActive) return;
        
        if (state.GetType().IsAssignableFrom(typeof(PlayerJumpState)))
        {
            PlayAnimation();
            if(enemy) isCheckAttack = true;
            isCheckPath = true;
        }
    }

    public void SetTarget(GameObject target)
    {
        currentTarget = null;
        ClearColorsToPath();

        finalTarget = target;
        finalCell = Calculate.FindCell.GetCell(finalTarget.transform.position, Vector3.down);
        
        if(isActive)
        {
            UpdatePathToTarget();
            UpdateValuesCheckEnemy();
        }
    }

    private void UpdateValuesCheckEnemy()
    {
        if (!finalTarget.TryGetComponent(out CellController cellController))
        {
            enemy = finalTarget;
            isCheckAttack = !StateMachine.IsActivateType(typeof(PlayerJumpState));
        }
        else
        {
            isCheckAttack = false;
        }
    }

    private void ClearColorsToPath()
    {
        if (finalCell && finalCell.TryGetComponent(out UnitRenderer renderer))
        {
            renderer.ResetColor();
        }
        
        while (pathQueue.Count > 0)
        {
            var cell = pathQueue.Dequeue();
            if (cell && cell.TryGetComponent(out UnitRenderer unitRenderer))
            {
                unitRenderer.ResetColor();
            }
        }
    }

    private void UpdatePathToTarget(bool compareDistance = false)
    {
        pathQueue.Clear();
        finalTargetPosition = finalTarget.transform.position;
        pathFinder.SetStartPosition(GameObject.transform.position);
        pathFinder.SetTargetPosition(finalTargetPosition);
        AssignNewCurrentTarget(compareDistance);
    }

    private void AssignNewCurrentTarget(bool compareDistance = false)
    {
        if (pathQueue.Count == 0)
        {
            pathQueue = pathFinder.GetPath(compareDistance);
            
            foreach (var cell in pathQueue)
                cell.SetColor(Color.yellow);

            if(finalCell.TryGetComponent(out UnitRenderer unitRenderer))
                unitRenderer.SetColor(Color.red);
        }

        if (pathQueue.Count > 0)
        {
            currentTarget = pathQueue.Peek()?.gameObject;
            rotationController.SetTarget(currentTarget.transform);
            currentTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
        }
    }

    private void UpdatePath()
    {
        if (!isCheckPath) return;

        countCooldownCheckTarget += Time.deltaTime;
        if (countCooldownCheckTarget < checkTargetCooldown) return;
        countCooldownCheckTarget = 0;

        if (!IsFinalPositionValid())
        {
            ClearColorsToPath();
            UpdatePathToTarget(true);
            return;
        }

        var currentCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);

        if (!currentCell || previousTargetCoordinates == Vector2Int.zero) return;

        if (currentTargetCoordinates == currentCell.CurrentCoordinates || 
            previousTargetCoordinates == currentCell.CurrentCoordinates)
            return;

        ClearColorsToPath();
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
    
    private void CheckEnemy()
    {
        if(!isCheckAttack) return;
                
        countCooldownheckEnemy += Time.deltaTime;
        if (countCooldownheckEnemy > checkEnemyCooldown)
        {
            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, enemy.transform.position, _playerSwitchAttack.RangeAttackSqr))
            {
                _playerSwitchAttack.SetTarget(finalTarget);
                _playerSwitchAttack.ExitOtherStates();
            }

            countCooldownheckEnemy = 0;
        }
    }

    private void ReduceEndurance()
    {
        PlayerEndurance.RemoveEndurance(RunReductionEndurance);
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
        public PlayerRunStateBuilder SetRunReductionEndurance(float value)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.RunReductionEndurance = value;
            
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