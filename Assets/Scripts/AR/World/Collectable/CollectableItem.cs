using System;
using UnityEngine;

namespace AR.World.Collectable
{
    public abstract class CollectableItem : MonoBehaviour, ICollectable
    {
        [SerializeField] private Outline outline;
        [SerializeField] private float interactionDistance = 5f;

        private Camera _camera;
        private Transform _transform;
        private bool _isInsidePlayerARCollider;
        private Action _action;

        public Camera Camera => _camera;
        public Transform Transform => _transform;

        public event Action<ICollectable> Interacted;
        public event Action<(ICollectable collectable, bool canBeCollected)> CollectableStatusChanged;

        public void SetupCollectAction(Action action)
        {
            _action = action;
        }

        public void Interact()
        {
            _action?.Invoke();
            Interacted?.Invoke(this);
        }

        public bool IsCanBeCollected(Vector3 playerPosition)
        {
            bool collectable = Vector3.Distance(playerPosition, _transform.position) <= interactionDistance ||
                               _isInsidePlayerARCollider;
            outline.enabled = collectable;

            return collectable;
        }

        public void IsInsidePlayerARCollider(bool isInside)
        {
            _isInsidePlayerARCollider = isInside;
        }

        private void Update()
        {
            if (!Camera) return;

            bool collectable =
                Vector3.Distance(Camera.transform.position, _transform.position) <= interactionDistance ||
                _isInsidePlayerARCollider;
            
            outline.enabled = collectable;

            CollectableStatusChanged?.Invoke((this, collectable));

            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        private void Awake()
        {
            _camera = Camera.main;
            _transform = transform;
        }
    }
}