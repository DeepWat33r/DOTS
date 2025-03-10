using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefabGameObject;
        public GameObject zombiePrefabGameObject;
        public GameObject shootLightPrefabGameObject;
        public class Baker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReference()
                {
                    bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                    zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),
                    shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
    
    public struct EntitiesReference : IComponentData
    {
        public Entity bulletPrefabEntity;
        public Entity zombiePrefabEntity;
        public Entity shootLightPrefabEntity;
    }
}

