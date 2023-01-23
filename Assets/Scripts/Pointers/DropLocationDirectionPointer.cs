using UnityEngine;

namespace Pointers
{
    public interface IDropLocationDirectionPointer
    {
        void SetIsVisible(bool visible);
        void SetTarget(IPointerTarget target);
    }

    public class DropLocationDirectionPointer : MonoBehaviour, IDropLocationDirectionPointer
    {
        private Transform _transform;
        private Transform _cameraTransform;
        private IPointerTarget _target;

        #region IDropLocationDirectionPointer

        public void SetIsVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetTarget(IPointerTarget target)
        {
            _target = target;
        }
        
        #endregion

        private void SetRotation(Vector3 myPosition, Vector3 targetPosition)
        {
            _transform.rotation = Quaternion.LookRotation(
                targetPosition - myPosition);
        }

        #region Unity Events

        private void Awake()
        {
            _transform = transform;

            Camera mainCamera = Camera.main;

            if (!mainCamera) return;

            _cameraTransform = mainCamera.transform;
        }

        private void Update()
        {
            if (_target == null || !_cameraTransform) return;
            SetRotation(_cameraTransform.position, _target.Transform.position);
        }

        #endregion
    }
}