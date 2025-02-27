using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            foreach (var
                         (
                         localTransform,
                         shootAttack,
                         target)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<ShootAttack>,
                         RefRO<Target>>())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;
                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.timer > 0f)
                    continue;
                shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;
                
                Entity bulletEntity = state.EntityManager.Instantiate(entitiesReference.bulletPrefabEntity);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;
                
                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
            }
        
        }
        
    }
}
