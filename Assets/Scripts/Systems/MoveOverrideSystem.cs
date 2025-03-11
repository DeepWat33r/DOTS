using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct MoveOverrideSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (
                         localTransform,
                         unitMover,
                         moveOverride,
                         enabledMoveOverride
                         )
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<UnitMover>,
                         RefRO<MoveOverride>,
                         EnabledRefRW<MoveOverride>>())
            {
                if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) >
                    UnitMoverSystem.ReachedTargetPositionDistanceSq)
                {
                    unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
                }
                else
                {
                    enabledMoveOverride.ValueRW = false;
                }
            }
        }
    }
}