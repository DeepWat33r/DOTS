using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static MonoBehaviours.MouseWorldPosition;

namespace Systems
{
    partial struct UnitMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((
                         RefRW<LocalTransform> localTransform,
                         RefRO<MoveSpeed> moveSpeed,
                         RefRW<PhysicsVelocity> physicsVelocity)
                     in (SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<MoveSpeed>,
                         RefRW<PhysicsVelocity>>()))
            {
                float3 targetPosition = Instance.GetMouseWorldPosition();
                float3 direction = math.normalize(targetPosition - localTransform.ValueRO.Position);

                float rotationSpeed = 10f;
                localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(direction, math.up()), SystemAPI.Time.DeltaTime * rotationSpeed);

                physicsVelocity.ValueRW.Linear = direction * moveSpeed.ValueRO.value;
                physicsVelocity.ValueRW.Angular = float3.zero;
                
                
            }

        }
        
    }
}
