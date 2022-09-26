using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Items
{
    public class Gift : MonoBehaviour, IInteractable
    {
        [SerializeField] private Rigidbody objectRigidbody;
        
        private Transform _transform;
        
        public bool IsOnPlane { private set; get; }

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            Ray ray = new() {origin = _transform.position + Vector3.up * 0.1f, direction = Vector3.down};
            
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                IsOnPlane = raycastHit.collider.gameObject.TryGetComponent(out ARPlane plane);
            }
        }

        public void Interact() { }

        public void AddForce(Vector3 force)
        {
            objectRigidbody.AddForce(force);
        }
    }
}