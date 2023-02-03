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

        public Camera Camera => _camera ??= Camera.main;
        public Transform Transform => _transform;

        public event Action<ICollectable> Interacted;
        public event Action<(ICollectable collectable, bool canBeCollected)> CollectableStatusChanged;

        public void SetupCollectAction(Action action)
        {
            _action = action;
        }

        public virtual void Interact(Action onInteractionFinished)
        {
            _action?.Invoke();
            Interacted?.Invoke(this);
            onInteractionFinished?.Invoke();
        }

        public bool CheckCanBeCollected(Vector3 playerPosition)
        {
            bool collectable = Vector3.Distance(playerPosition, _transform.position) <= interactionDistance ||
                               _isInsidePlayerARCollider;
            outline.enabled = collectable;

            OnCollectAbilityChanges(collectable);
            
            return collectable;
        }

        public void IsInsidePlayerARCollider(bool isInside)
        {
            _isInsidePlayerARCollider = isInside;
        }

        private void Update()
        {
            if (!Camera) return;

            bool collectable = CheckCanBeCollected(Camera.transform.position);

            CollectableStatusChanged?.Invoke((this, collectable));

            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        protected virtual void OnCollectAbilityChanges(bool canBeCollected)
        {
            
        }

        private void Awake()
        {
            _transform = transform;
            OnAwake();
        }

        protected virtual void OnAwake()
        {
           
        }
    }
}