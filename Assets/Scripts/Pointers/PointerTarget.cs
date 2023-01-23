using UnityEngine;
using Zenject;

namespace Pointers
{
    public interface IPointerTarget
    {
        TargetType TargetType { get; }
        Transform Transform { get; }

        Vector3 GetPosition();
    }

    public class PointerTarget : MonoBehaviour, IPointerTarget
    {
        [SerializeField] private TargetType targetType;

        private IPointersController _pointersController;
        private Transform _transform;

        public Transform Transform => _transform;

        [Inject]
        public void Construct(IPointersController pointersController)
        {
            _pointersController = pointersController;
            _pointersController.AddTarget(this);
        }

        public TargetType TargetType => targetType;

        public Vector3 GetPosition() => _transform.position;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnDestroy()
        {
            _pointersController.RemoveTarget(this);
        }
    }
}