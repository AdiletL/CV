using System;
using System.Collections;
using Unit.Cell;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Unit.Trap.Activator
{
    [RequireComponent(typeof(SphereCollider))]
    public class ButtonController : ActivatorController
    {
        private ButtonAnimation buttonAnimation;
        private SphereCollider sphereCollider;
        
        private CellController currentCell;

        private Coroutine checkTargetCoroutine, startTimerCoroutine;
        private Coroutine cellMoveDirectionCoroutine;

        private Vector3 baseCellPosition;

        private float cooldownCheck = 1f, countCooldownCheck;
        private float radius;
        private float speed;
        
        private bool isReady = true;
        
        public override void Initialize()
        {
            base.Initialize();

            buttonAnimation = components.GetComponentFromArray<ButtonAnimation>();
            radius = .2f;
            speed = 20;
        }

        public override void Appear()
        {
            
        }
        
        
        public override void Activate()
        {
            base.Activate();
            isReady = false;
            buttonAnimation.ChangeAnimationWithDuration(activateClip);

            if (!currentCell)
            {
                var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
                if (colliders.Length > 0)
                {
                    currentCell = colliders[0].GetComponent<CellController>();
                    baseCellPosition = currentCell.transform.position;
                }
            }

            if(cellMoveDirectionCoroutine != null) StopCoroutine(cellMoveDirectionCoroutine);
            cellMoveDirectionCoroutine = StartCoroutine(CellMoveDirectionCoroutine(currentCell, Vector3.down));
            if(checkTargetCoroutine != null)
                StopCoroutine(checkTargetCoroutine);
            
            checkTargetCoroutine = StartCoroutine(CheckTargetCoroutine());
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            if(checkTargetCoroutine != null)
                StopCoroutine(checkTargetCoroutine);
            isReady = true;
            buttonAnimation.ChangeAnimationWithDuration(deactivateClip);

            ResetTargetPosition();
        }

        private void ResetTargetPosition()
        {
            currentCell.transform.position = baseCellPosition;
        }
        
        private IEnumerator StartTimerCoroutine(float waitTime, Action callback)
        {
            yield return new WaitForSeconds(waitTime);
            callback?.Invoke();
        }
        
        private IEnumerator CheckTargetCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldownCheck);
                if (isReady && Physics.OverlapSphere(transform.position, .3f, Layers.PLAYER_LAYER).Length > 0)
                {
                    Activate();
                }
            }
        }

        private IEnumerator CellMoveDirectionCoroutine(CellController cell, Vector3 direction)
        {
            float maxDistance = .15f;
            Vector3 startPos = cell.transform.position;
            Vector3 targetPos = startPos + direction * maxDistance;
            Vector3 currentPos = cell.transform.position;
            var cellTransform = cell.transform;

            while ((cellTransform.position - targetPos).sqrMagnitude > 0.01f) // Проверка, пока не достигнута цель
            {
                yield return null; 
                currentPos = Vector3.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
                cell.ChangePosition(currentPos);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady 
               || !Calculate.GameLayer.IsTarget(EnemyLayer, other.gameObject.layer)
               || CurrentTarget) return;
            
            CurrentTarget = other.gameObject;
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(0.1f, Activate));
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(EnemyLayer, other.gameObject.layer)
               || !CurrentTarget) return;
            
            Deactivate();
            CurrentTarget = null;
        }
    }
}