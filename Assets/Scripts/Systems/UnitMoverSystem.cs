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
                         RefRO<UnitMover> unitMover,
                         RefRW<PhysicsVelocity> physicsVelocity)
                     in (SystemAPI.Query<
                         RefRW<LocalTransform>,
                         RefRO<UnitMover>,
                         RefRW<PhysicsVelocity>>()))
            {
                float3 direction = math.normalize(unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position);
                
                localTransform.ValueRW.Rotation = math.slerp(
                    localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(direction, math.up()), 
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

                physicsVelocity.ValueRW.Linear = direction * unitMover.ValueRO.moveSpeed;
                physicsVelocity.ValueRW.Angular = float3.zero;
                
                
            }

        }
        
    }
}
