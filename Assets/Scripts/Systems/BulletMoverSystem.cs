using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct BulletMoverSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
            foreach (var (
                         localTransform,
                         bullet,
                         target,
                         entity)
                     in SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<Bullet>,
                         RefRO<Target>>().WithEntityAccess())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }
                LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                
                float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);
                
                float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);
            
                localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

                float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

                if(distanceAfterSq > distanceBeforeSq)
                {
                    localTransform.ValueRW.Position = targetLocalTransform.Position;
                }
                
                float destroyDistance = .002f;
                if (math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < destroyDistance)
                {
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                
                    ecb.DestroyEntity(entity);
                }
            
            }
        }
    }
}
