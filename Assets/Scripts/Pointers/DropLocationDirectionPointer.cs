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
        private Transform CameraTransform => _cameraTransform ??= Camera.main?.transform;
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
                                        Quaternion.Inverse(Quaternion.Euler(new Vector3(cameraTransformRotation.eulerAngles.x,
                                            cameraTransformRotation.eulerAngles.y, -cameraTransformRotation.eulerAngles.z)));
            _transform.rotation = targetRotation;
        }

        #region Unity Events

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_target == null || !CameraTransform || !_target.IsActive) return;
            SetRotation(CameraTransform.position, _target.Transform.position, _cameraTransform.rotation);
        }

        #endregion
    }
}