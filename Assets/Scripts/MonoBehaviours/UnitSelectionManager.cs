using Authoring;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetMouseWorldPosition();
                
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).
                    WithAll<UnitMover, Selected>().
                    Build(entityManager);
                
                NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
                for (int i = 0; i < unitMoverArray.Length; i++)
                {
                    UnitMover unitMover = unitMoverArray[i];
                    unitMover.targetPosition = mouseWorldPosition;
                    unitMoverArray[i] = unitMover;
                }
                entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }
    }
}
