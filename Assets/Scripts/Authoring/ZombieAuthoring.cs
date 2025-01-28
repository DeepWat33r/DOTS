using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    class ZombieAuthoring : MonoBehaviour
    {
        class Baker : Baker<ZombieAuthoring>
        {
            public override void Bake(ZombieAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Zombie
                {
                });
            }
        }
    }
}

public struct Zombie : IComponentData
{
    
}


