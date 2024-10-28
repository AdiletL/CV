using System;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerController : CharacterMainController
    {
        public static event Action OnFinished;

        public GameObject target { get; private set; }


        private void OnEnable()
        {
            components.GetComponentInGameObjects<PlayerMove>().OnFinishedToTarget += OnFinihedToTarget;
        }

        private void OnDisable()
        {
            components.GetComponentInGameObjects<PlayerMove>().OnFinishedToTarget -= OnFinihedToTarget;
        }

        private void OnFinihedToTarget()
        {
            components.GetComponentInGameObjects<PlayerMove>().enabled = false;
            OnFinished?.Invoke();
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
            components.GetComponentInGameObjects<PlayerMove>().SetTarget(target);
        }
    
    
    }
}
