using UnityEngine;

namespace Character.Enemy
{
    public class EnemyController : CharacterMainController
    {
        public override void Initialize()
        {
            base.Initialize();
            
            components.GetComponentInGameObjects<EnemyHealth>()?.Initialize();
        }
    }
}