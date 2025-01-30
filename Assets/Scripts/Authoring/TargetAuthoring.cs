using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TargetAuthoring : MonoBehaviour
    {
        public GameObject target;
        public class Baker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Target()
                {
                    targetEntity = GetEntity(authoring.target, TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct Target : IComponentData
    {
        public Entity targetEntity;
    }
}
