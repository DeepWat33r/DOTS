using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    partial struct MeleeAttackSystem : ISystem
    {
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            NativeList<RaycastHit> hits = new NativeList<RaycastHit>(Allocator.Temp);
            foreach (var (
                         localTransform,
                         meleeAttack,
                         target,
                         unitMover)
                     in SystemAPI.Query<
                         RefRO<LocalTransform>,
                         RefRW<MeleeAttack>,
                         RefRO<Target>,
                         RefRW<UnitMover>>().WithDisabled<MoveOverride>())
                
            {
                if (target.ValueRO.targetEntity == Entity.Null) continue;

                float meleeAttackDistanceSq = 2f;
                LocalTransform targetLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
                
                bool isCloseEnough = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) <
                                     meleeAttackDistanceSq;

                bool isTouchingTarget = false;
                if (!isCloseEnough)
                {
                    float3 directionToTarget = math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);
                    float distanceExtratiTestRayCast =  0.4f;  
                    RaycastInput raycastInput = new RaycastInput
                    {
                        Start = localTransform.ValueRO.Position,
                        End = targetLocalTransform.Position + directionToTarget * meleeAttack.ValueRO.colliderSize + distanceExtratiTestRayCast,
                        Filter = CollisionFilter.Default,
                    };
                    hits.Clear();
                    if(collisionWorld.CastRay(raycastInput, ref hits))
                    {
                        foreach (RaycastHit hit in hits)
                        {
                            if (hit.Entity == target.ValueRO.targetEntity)
                            {
                                isTouchingTarget = true;
                                break;
                            }
                        }
                    }
                }
                
                if (!isCloseEnough && !isTouchingTarget)
                {
                    unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                }
                else
                {
                    unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                    meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                    if (meleeAttack.ValueRO.timer > 0)
                    {
                        continue;
                    }
                    meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;
                    
                    RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                    targetHealth.ValueRW.healthAmount -= meleeAttack.ValueRO.damageAmount;
                    targetHealth.ValueRW.onHealthChanged = true;
                    
                }
            }
        }
    }
}