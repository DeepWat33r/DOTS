using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    partial struct SetUnitDefaultPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer entityCommandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (
                         localTransform,
                         unitMover,
                         defaultPosition,
                         entity)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<UnitMover>, 
                         RefRO<SetUnitMoverDefaultPosition>>().WithEntityAccess())
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                entityCommandBuffer.RemoveComponent<SetUnitMoverDefaultPosition>(entity);
            }
        }
    }
}