using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReference>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            foreach (var
                         (
                         localTransform,
                         shootAttack,
                         target,
                         unitMover)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRW<ShootAttack>,
                         RefRO<Target>,
                         RefRW<UnitMover>>())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;
                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                
                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) >
                    shootAttack.ValueRO.attackDistance)
                {
                    unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                }
                else
                {
                    unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                }
                
                float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);

                quaternion targetRotation = quaternion.LookRotation(aimDirection, new float3(0, 1, 0));
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation,
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                
                if (shootAttack.ValueRO.timer > 0f)
                    continue;
                shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

                Entity bulletEntity = state.EntityManager.Instantiate(entitiesReference.bulletPrefabEntity);
                float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

                RefRW<Bullet> bullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

                RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
            }
        }
    }
}