using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static MonoBehaviours.MouseWorldPosition;

namespace Systems
{
    partial struct UnitMoverSystem : ISystem
    {
        public const float ReachedTargetPositionDistanceSq = 2f;
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            UnitMoverJob unitMoverJob = new UnitMoverJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            };
            unitMoverJob.ScheduleParallel();
            
        }
        
    }
    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        public float deltaTime;

        private void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
        {
            float3 direction = unitMover.targetPosition - localTransform.Position;
            
            float targetDistance = UnitMoverSystem.ReachedTargetPositionDistanceSq;
            if (math.lengthsq(direction) <= targetDistance)
            {
                physicsVelocity.Linear = float3.zero;
                physicsVelocity.Angular = float3.zero;
                return;
            }
            
            direction = math.normalize(direction);
            
            localTransform.Rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(direction, math.up()),
                deltaTime * unitMover.rotationSpeed);

            physicsVelocity.Linear = direction * unitMover.moveSpeed;
            physicsVelocity.Angular = float3.zero;
        }
    }
}
