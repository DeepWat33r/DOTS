using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct ZombieSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();

            EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (
                         localTransform,
                         zombieSpawner)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<ZombieSpawner>>())
            {
                zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (zombieSpawner.ValueRW.timer > 0f)
                {
                    continue;
                }
                zombieSpawner.ValueRW.timer = zombieSpawner.ValueRW.timerMax;
                
                Entity zombieEntity = state.EntityManager.Instantiate(entitiesReference.zombiePrefabEntity);
                SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                
                entityCommandBuffer.AddComponent(zombieEntity, new RandomWalking
                {
                    targetPosition = localTransform.ValueRO.Position,
                    originPosition = localTransform.ValueRO.Position,
                    distanceMin = zombieSpawner.ValueRO.randomWalkingDistanceMin,
                    distanceMax = zombieSpawner.ValueRO.randomWalkingDistanceMax,
                    random = new Random((uint)zombieEntity.Index)
                });
            }
        }
        
    }
}
