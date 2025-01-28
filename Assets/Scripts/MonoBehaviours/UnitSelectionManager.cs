using System;
using System.Numerics;
using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance { get; private set; }
        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;

        private Vector2 _selectionStartMousePosition;

        public void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectionStartMousePosition = Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonUp(0))
            {
                var selectionEndMousePosition = Input.mousePosition;

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery =
                    new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
                for (int i = 0; i < entityArray.Length; i++)
                {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                    Selected selected = selectedArray[i];
                    selected.onDeselected = true;
                    entityManager.SetComponentData(entityArray[i], selected);
                }
                
                Rect selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                float minimumSelectionAreaSize = 20f;
                bool isMultipleSelection = selectionAreaSize > minimumSelectionAreaSize;

                if (isMultipleSelection)
                {
                    //Multiple Selection
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>()
                        .WithPresent<Selected>().Build(entityManager);
                    entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                    NativeArray<LocalTransform> localTransformArray =
                        entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                    for (int i = 0; i < localTransformArray.Length; i++)
                    {
                        LocalTransform unitLocalTransform = localTransformArray[i];
                        Vector2 unitScreenPosition = Camera.main!.WorldToScreenPoint(unitLocalTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition))
                        {
                            entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                            Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                            selected.onSelected = true;
                            entityManager.SetComponentData(entityArray[i], selected);
                        }
                    }
                }
                else
                {
                    //Single Selection
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                    UnityEngine.Ray cameraRay = Camera.main!.ScreenPointToRay(Input.mousePosition);
                    int unitLayer = LayerMask.NameToLayer("Units");
                    RaycastInput raycastInput = new RaycastInput()
                    {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter()
                        {
                            BelongsTo = ~0u,
                            CollidesWith = 1u << unitLayer,
                            GroupIndex = 0
                        }
                    };
                    if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                    {
                        if (entityManager.HasComponent<Unit>(raycastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                            Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                            selected.onSelected = true;
                            entityManager.SetComponentData(raycastHit.Entity, selected);
                        }
                    }
                    
                }

                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetMouseWorldPosition();

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>()
                    .Build(entityManager);
                
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
                NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
                
                for (int i = 0; i < unitMoverArray.Length; i++)
                {
                    UnitMover unitMover = unitMoverArray[i];
                    unitMover.targetPosition = movePositionArray[i];
                    unitMoverArray[i] = unitMover;
                }

                entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(_selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Min(_selectionStartMousePosition.y, selectionEndMousePosition.y));
            Vector2 upperRightCorner = new Vector2(
                Mathf.Max(_selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Max(_selectionStartMousePosition.y, selectionEndMousePosition.y));

            return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x,
                upperRightCorner.y - lowerLeftCorner.y);
        }

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positonCount)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(positonCount, Allocator.Temp);
            if (positonCount == 0)
                return positionArray;

            positionArray[0] = targetPosition;
            if (positonCount == 1)
                return positionArray;

            float ringSize = 2.2f;
            int ring = 0;
            int positionIndex = 1;
            
            while (positionIndex < positonCount)
            {
                int ringPositionCount = 3 + ring * 2;
                for (int i = 0; i < ringPositionCount; i++)
                {
                    float angle = i * (2 * Mathf.PI / ringPositionCount);
                    float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                    float3 ringPosition = targetPosition + ringVector;
                    
                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    if (positionIndex >= positonCount)
                        break;
                }
                ring++;
            }

            return positionArray;
        }
    }
}