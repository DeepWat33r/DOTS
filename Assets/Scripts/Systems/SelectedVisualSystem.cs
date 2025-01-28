using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    partial struct SelectedVisualSystem : ISystem
    {
    
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                if (selected.ValueRO.onSelected)
                {
                    Debug.Log("Selected");
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
                }
                if (selected.ValueRO.onDeselected)
                {
                    Debug.Log("Deselected");
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                    visualLocalTransform.ValueRW.Scale = 0f;
                }
            }
        }
    
    }
}
