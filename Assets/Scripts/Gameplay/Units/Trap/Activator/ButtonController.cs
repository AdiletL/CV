using System;
using System.Collections;
using System.Collections.Generic;
using Unit.Cell;
using UnityEngine;

namespace Unit.Trap.Activator
{
    [RequireComponent(typeof(SphereCollider))]
    public class ButtonController : ActivatorController
    {
        private ButtonAnimation buttonAnimation;
        private SphereCollider sphereCollider;

        private Coroutine checkTargetCoroutine, startTimerCoroutine;
        private Coroutine cellMoveDirectionCoroutine;

        private float cooldownCheck = 1f, countCooldownCheck;
        private float radius;
        private float speed;
        
        private bool isReady = true;
        
        public override void Initialize()
        {
            base.Initialize();

            buttonAnimation = components.GetComponentFromArray<ButtonAnimation>();
            radius = .2f;
            speed = 100;
        }

        public override void Appear()
        {
            
        }
        
        
        public override void Activate()
        {
            base.Activate();
            isReady = false;
            buttonAnimation.ChangeAnimationWithDuration(activateClip);
            
            var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
            if (colliders.Length > 0)
            {
                var cell = colliders[0].GetComponent<CellController>();
                if(cellMoveDirectionCoroutine != null) StopCoroutine(cellMoveDirectionCoroutine);
                cellMoveDirectionCoroutine = StartCoroutine(CellMoveDirectionCoroutine(cell, Vector3.down));
            }

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
            
            var colliders = Physics.OverlapSphere(transform.position, radius, Layers.CELL_LAYER);
            if (colliders.Length > 0)
            {
                var cell = colliders[0].GetComponent<CellController>();
                if(cellMoveDirectionCoroutine != null) StopCoroutine(cellMoveDirectionCoroutine);
                cellMoveDirectionCoroutine = StartCoroutine(CellMoveDirectionCoroutine(cell, Vector3.up));
            }
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
            float maxDistance = .2f; // Сдвиг вниз на 2 единицы
            Vector3 startPos = cell.transform.position; // Начальная позиция
            Vector3 targetPos = startPos + direction * maxDistance; // Конечная позиция
            var cellTransform = cell.transform; // Кэширование transform
            float speedPerFrame;

            while ((cellTransform.position - targetPos).sqrMagnitude > 0.01f) // Проверка, пока не достигнута цель
            {
                yield return null; // Ожидание следующего кадра
        
                speedPerFrame = Time.deltaTime * speed; // Расстояние для перемещения за кадр
                cell.MoveDirection(direction, speedPerFrame);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isReady 
               || !Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || CurrentTarget) return;
            
            CurrentTarget = other.gameObject;
            if(startTimerCoroutine != null)
                StopCoroutine(startTimerCoroutine);
            startTimerCoroutine = StartCoroutine(StartTimerCoroutine(0.1f, Activate));
        }

        private void OnTriggerExit(Collider other)
        {
            if(!Calculate.GameLayer.IsTarget(EnemyLayers, other.gameObject.layer)
               || !CurrentTarget) return;
            
            Deactivate();
            CurrentTarget = null;
        }
    }
}