using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ShootLightSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            foreach (var shootAttack in SystemAPI.Query<RefRO<ShootAttack>>())
            {
                if (shootAttack.ValueRO.onShoot.isTriggered)
                {
                    Entity shootLightEntity = state.EntityManager.Instantiate(entitiesReference.shootLightPrefabEntity);
                    SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(shootAttack.ValueRO.onShoot.shootFromPosition));
                }
            }
        }
    
    }
}
