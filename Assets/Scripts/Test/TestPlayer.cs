using System;
using Gameplay.Damage;
using Gameplay.Manager;
using Gameplay.Weapon.Projectile;
using UnityEngine;
using Zenject;

public class TestPlayer : MonoInstaller
{
   [SerializeField] private Transform target;
   [SerializeField] private PoolManager pool;


   private PoolManager newPool;

   public override void InstallBindings()
   {
   }

   public void Awake()
   {
      newPool = Container.InstantiatePrefabForComponent<PoolManager>(pool);
      newPool.Initialize();
      Container.Bind<IPoolable>().FromInstance(newPool).AsSingle();
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.A))
      {
         var arrow = newPool.GetObject<ArrowController>();
         arrow.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
         arrow.GetComponent<ArrowController>()?.Initialize();
         arrow.GetComponent<ArrowController>()?.SetTargetPosition(target.position);
         arrow.GetComponent<ArrowController>()?.SetDamageable(new NormalDamage(1, gameObject));
      }
   }
}