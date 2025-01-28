using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    /*partial struct TestingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int unitCount = 0;
            foreach (var friendly
                     in SystemAPI.Query<
                         RefRW<Friendly>>())
            {
                unitCount++;
            }

            Debug.Log("Unit count: " + unitCount);
        }
    }*/
}