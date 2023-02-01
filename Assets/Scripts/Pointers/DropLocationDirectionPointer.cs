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

        private void SetRotation(Vector3 myPosition, Vector3 targetPosition, Quaternion cameraTransformRotation)
        {
            Quaternion targetRotation = Quaternion.LookRotation(
                                            targetPosition - myPosition) *
                                        Quaternion.Inverse(Quaternion.Euler(new Vector3(0f,
                                            cameraTransformRotation.eulerAngles.y, 0f)));
            _transform.rotation = Quaternion.Euler(new Vector3(0f, targetRotation.eulerAngles.y, 0f));
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
            SetRotation(_cameraTransform.position, _target.Transform.position, _cameraTransform.rotation);
        }

        #endregion
    }
}