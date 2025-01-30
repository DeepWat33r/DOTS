using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthDeadSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (
                         health,
                         entity)
                     in SystemAPI.Query<
                             RefRO<Health>>()
                         .WithEntityAccess())
            {
                if (health.ValueRO.healthAmount <= 0)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}