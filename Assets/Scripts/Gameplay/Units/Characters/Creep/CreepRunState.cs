using System.Collections.Generic;
using Calculate;
using Movement;
using Unit.Cell;
using UnityEngine;

namespace Unit.Character.Creep
{
    public class CreepRunState : CharacterRunState
    {
        private PathFinding pathFinding;
        private Rotation rotation;

        private GameObject finalTarget;
        private GameObject currentTarget;

        private Vector3 finalTargetPosition;
        private Vector3 currentTargetPosition;
        private Vector3 direction;

        private Vector2Int currentTargetCoordinates;
        private Vector2Int previousTargetCoordinates;

        private const float CooldownCheckTarget = 0.2f;
        private float countCooldownCheckTarget;

        private Queue<CellController> pathToPoint = new();

        public Transform Center { get; set; }
        public CharacterController CharacterController { get; set; }
        public float RotationSpeed { get; set; }


        private bool IsFinalTargetReached()
        {
            return Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, finalTarget.transform.position) ||
                   pathToPoint.Count == 0;
        }

        private bool IsTargetInvalid()
        {
            if (currentTarget != null)
                return false;

            AssignNextCurrentTarget();

            if (currentTarget == null)
            {
                StateMachine.ExitCategory(Category, null);
                return true;
            }

            return false;
        }

        private bool IsFinalPositionValid()
        {
            return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
        }

        private bool IsTargetCellUnchanged(CellController currentCell)
        {
            return currentTargetCoordinates == currentCell.CurrentCoordinates ||
                   previousTargetCoordinates == currentCell.CurrentCoordinates;
        }

        public override void Initialize()
        {
            base.Initialize();
            rotation = new Rotation(GameObject.transform, RotationSpeed);
            pathFinding = new PathFindingBuilder()
                .SetStartPosition(GameObject.transform.position)
                .SetEndPosition(GameObject.transform.position)
                .Build();
        }

        public override void Update()
        {
            base.Update();

            if (IsTargetInvalid())
                return;

            CheckPath();
            MoveToCurrentTarget();
        }

        public override void Exit()
        {
            base.Exit();
            ClearCurrentTarget();
        }

        public void SetTarget(GameObject target)
        {
            ClearCurrentTarget();
            finalTarget = target;
            FindNewPathToFinalTarget();
        }

        private void FindNewPathToFinalTarget()
        {
            pathToPoint.Clear();
            finalTargetPosition = finalTarget.transform.position;

            pathFinding.SetStartPosition(GameObject.transform.position);
            pathFinding.SetTargetPosition(finalTargetPosition);

            AssignNextCurrentTarget();
        }

        private void AssignNextCurrentTarget()
        {
            if (pathToPoint.Count == 0)
                pathToPoint = pathFinding.GetPath();

            if (pathToPoint.Count == 0) return;

            var nextCell = pathToPoint.Peek();
            if (nextCell == null) return;

            currentTarget = nextCell.gameObject;
            rotation.SetTarget(currentTarget.transform);
            currentTargetCoordinates = nextCell.CurrentCoordinates;
        }

        private void CheckPath()
        {
            countCooldownCheckTarget += Time.deltaTime;

            if (countCooldownCheckTarget < CooldownCheckTarget)
                return;

            countCooldownCheckTarget = 0;

            if (!IsFinalPositionValid())
            {
                FindNewPathToFinalTarget();
                return;
            }

            var currentCell = Calculate.FindCell.GetCell(GameObject.transform.position, Vector3.down);

            if (currentCell == null || previousTargetCoordinates == Vector2Int.zero)
                return;

            if (IsTargetCellUnchanged(currentCell))
                return;

            FindNewPathToFinalTarget();
        }


        private void ClearCurrentTarget()
        {
            currentTarget = null;
        }

        private void MoveToCurrentTarget()
        {
            currentTargetPosition = new Vector3(
                currentTarget.transform.position.x,
                GameObject.transform.position.y,
                currentTarget.transform.position.z
            );

            if (Calculate.Distance.IsNearUsingSqr(GameObject.transform.position, currentTargetPosition))
            {
                previousTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
                ClearCurrentTarget();

                if (pathToPoint.Count > 0)
                    pathToPoint.Dequeue();

                if (IsFinalTargetReached())
                    StateMachine.ExitCategory(Category, null);
            }
            else
            {
                if (!Calculate.Move.IsFacingTargetUsingAngle(
                        GameObject.transform.position,
                        GameObject.transform.forward,
                        currentTargetPosition))
                {
                    rotation.Rotate();
                    return;
                }

                direction = (currentTargetPosition - GameObject.transform.position).normalized;
                CharacterController.Move(direction * (MovementSpeed * Time.deltaTime));
            }
        }
    }

public class CreepRunStateBuilder : CharacterRunStateBuilder
    {
        public CreepRunStateBuilder(CreepRunState instance) : base(instance)
        {
        }


        public CreepRunStateBuilder SetCenter(Transform center)
        {
            if (state is CreepRunState playerRunState)
                playerRunState.Center = center;

            return this;
        }

        public CreepRunStateBuilder SetCharacterController(CharacterController characterController)
        {
            if (state is CreepRunState playerRunState)
                playerRunState.CharacterController = characterController;

            return this;
        }

        public CreepRunStateBuilder SetRotationSpeed(float rotationSpeed)
        {
            if (state is CreepRunState playerRunState)
                playerRunState.RotationSpeed = rotationSpeed;

            return this;
        }
    }
}
