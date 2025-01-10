using System;
using UnityEngine;

namespace MonoBehaviours
{
    public class MouseWorldPosition : MonoBehaviour
    {
        public static MouseWorldPosition Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main!.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if(plane.Raycast(ray, out float distance)) return ray.GetPoint(distance);
            else return Vector3.zero;
            
        }
    }
}
