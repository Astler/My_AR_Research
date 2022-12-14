using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Items
{
    public class GiftView : MonoBehaviour, IInteractable<GiftView>
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

        public event Action<GiftView> Interacted;

        public void Interact()
        {
            Interacted?.Invoke(this);
        }

        public void AddForce(Vector3 force)
        {
            objectRigidbody.AddForce(force);
        }

        public Vector3 GetPosition() => _transform.position;
    }
}