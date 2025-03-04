using System;
using UnityEngine;

namespace MonoBehaviours
{
    public class GameAssets : MonoBehaviour
    {
        public const int UNIT_LAYER = 6;
        public static GameAssets Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
