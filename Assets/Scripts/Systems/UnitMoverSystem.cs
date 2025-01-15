using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
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
            UnitMoverJob unitMoverJob = new UnitMoverJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            };
            unitMoverJob.ScheduleParallel();

            /*foreach ((
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


            }*/
        }
        
    }
    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
        {
            float3 direction = math.normalize(unitMover.targetPosition - localTransform.Position);
                
            localTransform.Rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(direction, math.up()),
                deltaTime * unitMover.rotationSpeed);

            physicsVelocity.Linear = direction * unitMover.moveSpeed;
            physicsVelocity.Angular = float3.zero;
        }
    }
}
