using System;
using Unit;
using UnityEngine;

namespace  Movement
{
    public class ArcMovement : IMovement
    {
        public event Action OnFinished; 
        
        private AnimationCurve moveCurve;
        
        private GameObject gameObject;
        
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private Vector3 previousPosition;

        private float timerHeight;
        private float totalHeight;
        private float totalDuration;
        private float height;

        private bool isMoveable;

        public Stat MovementSpeedStat { get; private set; } = new Stat();
        

        public ArcMovement(GameObject gameObject, float movementSpeed, float height, AnimationCurve moveCurve)
        {
            this.gameObject = gameObject;
            this.MovementSpeedStat.AddValue(movementSpeed);
            this.height = height;
            this.moveCurve = moveCurve;
        }
        
        
        public void Initialize()
        {
        }



        public void ExecuteMovement()
        {
            if (isMoveable)
            {
                timerHeight += Time.deltaTime;
                float progress = Mathf.Clamp01(timerHeight / totalDuration);

                Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, progress);

                float verticalOffset = moveCurve.Evaluate(progress) * totalHeight;

                Vector3 currentPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + verticalOffset,
                    horizontalPosition.z);

                gameObject.transform.position = currentPosition;

                Vector3 direction = (currentPosition - previousPosition).normalized;

                if (direction != Vector3.zero)
                    gameObject.transform.rotation = Quaternion.LookRotation(direction);

                previousPosition = currentPosition;

                if (progress >= 1f)
                    isMoveable = false;
            }
            else
            {
               OnFinished?.Invoke();
            }
        }

        public void UpdateData(Vector3 target)
        {
            targetPosition = target;
            startPosition = gameObject.transform.position;
            var distance = Vector3.Distance(startPosition, targetPosition);
            totalHeight = (distance * .1f) * height;
            totalDuration = distance / MovementSpeedStat.CurrentValue;
            timerHeight = 0;
            isMoveable = true;

            previousPosition = gameObject.transform.position;
            
            Vector3 initialDirection = (targetPosition - startPosition).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(initialDirection);
        }
    }
}
