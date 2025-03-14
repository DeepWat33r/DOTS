using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var 
                         target 
                     in SystemAPI.Query<
                         RefRW<Target>>())
            {
                if (target.ValueRO.targetEntity != Entity.Null)
                    if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
                        target.ValueRW.targetEntity = Entity.Null;
                
            }
        }

    }
}
