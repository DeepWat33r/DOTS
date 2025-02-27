using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        public GameObject bulletPrefabGameObject;
        public class Baker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReference()
                {
                    bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
    
    public struct EntitiesReference : IComponentData
    {
        public Entity bulletPrefabEntity;
    }
}

