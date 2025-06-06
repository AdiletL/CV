﻿using System.Collections.Generic;
using Calculate;
using Gameplay.Unit.Cell;
using Movement;
using Photon.Pun;
using UnityEngine;

namespace Gameplay.Unit.Character.Player
{
  public class PlayerMoveToTargetState : CharacterMoveState
{
    private PhotonView photonView;
    private PathFinding pathFinder;
    private Rotation rotation;
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
        return Calculate.Distance.IsDistanceToTargetSqr(finalTarget.transform.position, finalTargetPosition);
    }

    public override void Initialize()
    {
        base.Initialize();
        rotation = new Rotation(gameObject.transform, rotationSpeed);
        pathFinder = new PathFindingBuilder()
            .SetStartPosition(gameObject.transform.position)
            .SetEndPosition(gameObject.transform.position)
            .Build();
    }

    public override void Enter()
    {
        base.Enter();
        
        countCooldownheckEnemy = checkEnemyCooldown;

        isCheckAttack = !stateMachine.IsActivateType(typeof(PlayerJumpState));
        isCheckPath = !stateMachine.IsActivateType(typeof(PlayerJumpState));
        UpdatePathToTarget();
        UpdateValuesCheckEnemy();
    }

    public override void Subscribe()
    {
        base.Subscribe();
        stateMachine.OnExitCategory += OnExitCategory;
    }

    public override void Update()
    {
        base.Update();

        if (!currentTarget)
        {
            AssignNewCurrentTarget();
            if (!currentTarget)
            {
                stateMachine.ExitCategory(Category, null);
                return;
            }
        }

        UpdatePath();
        ExecuteMovement();
        CheckEnemy();
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        stateMachine.OnExitCategory -= OnExitCategory;
    }

    public override void Exit()
    {
        base.Exit();
        ClearColorsToPath();
        currentTarget = null;
        enemy = null;
        finalTarget = null;
    }

    private void OnExitCategory(IState state)
    {
        if(!IsActive) return;
        
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
        
        if(IsActive)
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
            isCheckAttack = !stateMachine.IsActivateType(typeof(PlayerJumpState));
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
            rotation.SetTarget(currentTarget.transform);
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
        if (Calculate.Distance.IsDistanceToTargetSqr(gameObject.transform.position, finalTarget.transform.position) || pathQueue.Count == 0)
            stateMachine.ExitCategory(Category, null);
    }

    public override void ExecuteMovement()
    {
        currentTargetPosition = new Vector3(currentTarget.transform.position.x, gameObject.transform.position.y, currentTarget.transform.position.z);

        if (Calculate.Distance.IsDistanceToTargetSqr(gameObject.transform.position, currentTargetPosition))
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
            if (!Calculate.Rotate.IsFacingTargetXZ(gameObject.transform.position, gameObject.transform.forward, currentTargetPosition))
            {
                rotation.RotateToTarget();
                return;
            }

            movementDirection = (currentTargetPosition - gameObject.transform.position).normalized;
            characterController.Move(movementDirection * (MovementSpeedStat.CurrentValue * Time.deltaTime));
        }
    }
    
    private void CheckEnemy()
    {
        if(!isCheckAttack) return;
                
        countCooldownheckEnemy += Time.deltaTime;
        if (countCooldownheckEnemy > checkEnemyCooldown)
        {

            countCooldownheckEnemy = 0;
        }
    }
}


    public class PlayerMoveToTargetStateBuilder : CharacterMoveStateBuilder
    {
        public PlayerMoveToTargetStateBuilder() : base(new PlayerMoveToTargetState())
        {
        }
        
        public PlayerMoveToTargetStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is PlayerMoveToTargetState playerRunState)
                playerRunState.SetCharacterController(characterController);
            
            return this;  
        }
        public PlayerMoveToTargetStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is PlayerMoveToTargetState playerRunState)
                playerRunState.SetRotationSpeed(rotationSpeed);
            
            return this;  
        }
        public PlayerMoveToTargetStateBuilder SetRunReductionEndurance(float value)
        {
            if (state is PlayerMoveToTargetState playerRunState)
                playerRunState.SetRunReductionEndurance(value);
            
            return this;  
        }

        public PlayerMoveToTargetStateBuilder SetPhotonView(PhotonView view)
        {
            if (state is PlayerMoveToTargetState playerRunState)
                playerRunState.SetPhotonView(view);
            
            return this;  
        }
    }
}