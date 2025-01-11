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

        private readonly float cooldownCheckTarget = .2f;
        private float countCooldownCheckTarget;

        private bool isRotate;
        private bool isCheckPath;

        private Queue<CellController> pathToPoint = new();
        

        public Transform Center { get; set; }
        public CharacterController CharacterController { get; set; }
        public float RotationSpeed { get; set; }
        
        
        private bool isFinalPositionCorrect()
        {
            return Calculate.Distance.IsNearUsingSqr(finalTarget.transform.position, finalTargetPosition);
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

        public override void Enter()
        {
            base.Enter();

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
        

        public override void Exit()
        {
            base.Exit();
            currentTarget = null;
        }
        

        public void SetTarget(GameObject target)
        {
            currentTarget = null;
            finalTarget = target;
            FindNewPathToPoint();
        }
        

        private void FindNewPathToPoint()
        {
            pathToPoint.Clear();
            finalTargetPosition = finalTarget.transform.position;
            pathFinding.SetStartPosition(GameObject.transform.position);
            pathFinding.SetTargetPosition(finalTargetPosition);
            NewCurrentTarget();
        }

        private void NewCurrentTarget()
        {
            if(pathToPoint.Count == 0)
                pathToPoint = pathFinding.GetPath(true);
                
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
                FindNewPathToPoint();
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
                previousTargetCoordinates = currentTarget.GetComponent<CellController>().CurrentCoordinates;
                currentTarget = null; 
                pathToPoint.Dequeue();

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