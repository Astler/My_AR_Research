using Niantic.ARDK.Extensions.Meshing;
using UnityEngine;

namespace Prototype.AR
{
    [RequireComponent(typeof(MeshRenderer), typeof(BoxCollider))]
    public class MeshCeilTrackerView : MonoBehaviour
    {
        [SerializeField] ARMeshManager meshUpdater;

        private MeshRenderer meshRenderer;
        private BoxCollider boxCollider;

        private Vector3 floorPosition = new(0, Mathf.Infinity, 0);

        [SerializeField] private bool floorVisible = true;
        public bool FloorVisible
        {
            get
            {
                return floorVisible;
            }
            set
            {
                ShowFloor(value);
                floorVisible = value;
            }
        }

        public bool FloorInvisible
        {
            get => !FloorVisible;
            set => FloorVisible = !value;
        }

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            boxCollider = GetComponent<BoxCollider>();

            if (meshUpdater != null) meshUpdater.MeshObjectsUpdated += RecalculateFloor;
        }

        void OnDestroy()
        {
            if (meshUpdater != null) meshUpdater.MeshObjectsUpdated -= RecalculateFloor;
        }

        void ShowFloor(bool visible)
        {
            meshRenderer.enabled = visible;
        }

        void RecalculateFloor(MeshObjectsUpdatedArgs args)
        {
            foreach (GameObject colliderGameObject in args.CollidersUpdated)
            {
                Collider collider = colliderGameObject.GetComponent<Collider>();
                
                if (collider != null)
                {
                    if (collider.bounds.max.y > floorPosition.y)
                    {
                        floorPosition.y = collider.bounds.max.y;
                        transform.position = floorPosition;
                        
                        meshRenderer.enabled = floorVisible;
                        boxCollider.enabled = true;

                        Debug.Log("MeshFloorFallback.RecalculateFloor: floorPosition.y:" + floorPosition.y);
                    }
                }
            }
        }
    }
}