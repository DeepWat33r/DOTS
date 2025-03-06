using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct HealthBarSystem : ISystem
    {
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Vector3 cameraForward = Vector3.zero;
            if (Camera.main !=null)
            {
                cameraForward = Camera.main.transform.forward;
            }
            foreach (var (
                         healthBar,
                         localTransform) 
                     in SystemAPI.Query<
                        RefRO<HealthBar>,
                        RefRW<LocalTransform>>())
            {
                LocalTransform parentLocalTransform =
                    SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);

                if (Mathf.Approximately(localTransform.ValueRO.Scale, 1f))
                {
                    localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                }
                
                Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

                if (!health.onHealthChanged)
                {
                    continue;
                }
                float healthNormalized = health.healthAmount / (float)health.healthAmountMax;

                if(Mathf.Approximately(healthNormalized, 1f))
                {
                    localTransform.ValueRW.Scale = 0f;
                }
                else
                {
                    localTransform.ValueRW.Scale = 1f;
                }
                RefRW<PostTransformMatrix> postTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
                postTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1f, 1f);
            }
        }
    }
}