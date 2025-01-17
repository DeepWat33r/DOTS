using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SelectedAuthoring : MonoBehaviour
    {
        
        public class Baker : Baker<SelectedAuthoring>
        {
            public override void Bake(SelectedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selected());
                SetComponentEnabled<Selected>(entity, false);
            }
        }
    }


    
    public struct Selected : IComponentData, IEnableableComponent
    {
    
    }
}

