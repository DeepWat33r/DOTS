using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SetUnitMoverDefaultPositionAuthoring : MonoBehaviour
    {
        public class Baker : Baker<SetUnitMoverDefaultPositionAuthoring>
        {
            public override void Bake(SetUnitMoverDefaultPositionAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SetUnitMoverDefaultPosition());
            }
        }
    }

    public struct SetUnitMoverDefaultPosition : IComponentData
    {
        
    }
}
