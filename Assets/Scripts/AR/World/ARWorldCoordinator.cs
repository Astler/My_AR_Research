using UnityEngine;

namespace AR.World
{
    public class ARWorldCoordinator: MonoBehaviour
    {
        [SerializeField] private Transform arContentTransform;

        public Transform GetContentTransform() => arContentTransform;
    }
}