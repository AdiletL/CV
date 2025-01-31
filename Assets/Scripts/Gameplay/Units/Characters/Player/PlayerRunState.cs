﻿using System.Collections.Generic;
using Calculate;
using Movement;
using Photon.Pun;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Player
{
  public class PlayerRunState : CharacterRunState
{
    private PhotonView photonView;
    private PathFinding pathFinder;
    private Rotation rotationController;
    private CellController finalCell;
    private CharacterController characterController;

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
    private float rotationSpeed;
    private float runReductionEndurance;
    
    private bool isCanRotate;
    private bool isCheckPath;
    private bool isCheckAttack;

    private Queue<CellController> pathQueue = new();

    public void SetCharacterController(CharacterController characterController) => this.characterController = characterController;
    public void SetPhotonView(PhotonView photonView) => this.photonView = photonView;
    public void SetRotationSpeed(float rotationSpeed) => this.rotationSpeed = rotationSpeed;
    public void SetRunReductionEndurance(float runReductionEndurance) => this.runReductionEndurance = runReductionEndurance;
    

    private bool IsFinalPositionValid()
    {
        return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
    }

    public override void Initialize()
    {
        base.Initialize();
        rotationController = new Rotation(gameObject.transform, rotationSpeed);
        pathFinder = new PathFindingBuilder()
            .SetStartPosition(gameObject.transform.position)
            .SetEndPosition(gameObject.transform.position)
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

    public override void SetTarget(GameObject target)
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
        pathFinder.SetStartPosition(gameObject.transform.position);
        pathFinder.SetTargetPosition(finalTargetPosition);
        AssignNewCurrentTarget(compareDistance);
    }

    private void AssignNewCurrentTarget(bool compareDistance = false)
    {
        if (pathQueue.Count == 0)
        {
            pathQueue = pathFinder.GetPath(compareDistance);

            if (photonView.IsMine)
            {
                foreach (var cell in pathQueue)
                    cell.SetColor(Color.yellow);

                if (finalCell.TryGetComponent(out UnitRenderer unitRenderer))
                    unitRenderer.SetColor(Color.red);
            }
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

        var currentCell = Calculate.FindCell.GetCell(gameObject.transform.position, Vector3.down);

        if (!currentCell || previousTargetCoordinates == Vector2Int.zero) return;

        if (currentTargetCoordinates == currentCell.CurrentCoordinates || 
            previousTargetCoordinates == currentCell.CurrentCoordinates)
            return;

        ClearColorsToPath();
        UpdatePathToTarget();
    }

    private void CheckIfTargetReached()
    {
        if (Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, finalTarget.transform.position) || pathQueue.Count == 0)
            StateMachine.ExitCategory(Category, null);
    }

    public override void ExecuteMovement()
    {
        currentTargetPosition = new Vector3(currentTarget.transform.position.x, gameObject.transform.position.y, currentTarget.transform.position.z);

        if (Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, currentTargetPosition))
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
            if (!Calculate.Move.IsFacingTargetUsingAngle(gameObject.transform.position, gameObject.transform.forward, currentTargetPosition))
            {
                rotationController.Rotate();
                return;
            }

            movementDirection = (currentTargetPosition - gameObject.transform.position).normalized;
            characterController.Move(movementDirection * (MovementSpeed * Time.deltaTime));

            ReduceEndurance();
        }
    }
    
    private void CheckEnemy()
    {
        if(!isCheckAttack) return;
                
        countCooldownheckEnemy += Time.deltaTime;
        if (countCooldownheckEnemy > checkEnemyCooldown)
        {
            if (Calculate.Distance.IsNearUsingSqr(gameObject.transform.position, enemy.transform.position, characterSwitchAttackState.RangeAttackSqr))
            {
                characterSwitchAttackState.SetTarget(finalTarget);
                characterSwitchAttackState.ExitOtherStates();
            }

            countCooldownheckEnemy = 0;
        }
    }

    private void ReduceEndurance()
    {
        unitEndurance.RemoveEndurance(runReductionEndurance);
    }
}


    public class PlayerRunStateBuilder : CharacterRunStateBuilder
    {
        public PlayerRunStateBuilder() : base(new PlayerRunState())
        {
        }
        
        public PlayerRunStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SetCharacterController(characterController);
            
            return this;  
        }
        public PlayerRunStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SetRotationSpeed(rotationSpeed);
            
            return this;  
        }
        public PlayerRunStateBuilder SetRunReductionEndurance(float value)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SetRunReductionEndurance(value);
            
            return this;  
        }

        public PlayerRunStateBuilder SetPhotonView(PhotonView view)
        {
            if (state is PlayerRunState playerRunState)
                playerRunState.SetPhotonView(view);
            
            return this;  
        }
    }
}