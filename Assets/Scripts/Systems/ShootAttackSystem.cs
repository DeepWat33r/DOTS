using Authoring;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var
                         (shootAttack,
                         target)
                     in SystemAPI.Query<
                         RefRW<ShootAttack>,
                         RefRO<Target>>())
            {
                if (target.ValueRO.targetEntity == Entity.Null)
                    continue;
                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.timer > 0f)
                    continue;
                shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;
                
                Debug.Log("Shoot");
            }
        
        }
        
    }
}
