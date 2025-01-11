using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class UnityMoverAuthoring : MonoBehaviour
    {
        public float moveSpeed;
        public float rotationSpeed;
        public class Baker : Baker<UnityMoverAuthoring>
        {
            public override void Bake(UnityMoverAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover
                {
                    moveSpeed = authoring.moveSpeed,
                    rotationSpeed = authoring.rotationSpeed
                });
            }
        }
    }
    public struct UnitMover : IComponentData
    {
        public float moveSpeed;
        public float rotationSpeed;
        public float3 targetPosition;
    }
}