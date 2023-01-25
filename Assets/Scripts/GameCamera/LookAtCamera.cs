using UnityEngine;

namespace GameCamera
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _transform;
        private Transform _cameraControlTransform;

        private Transform CameraTransform => _cameraControlTransform ??= Camera.main?.transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void LateUpdate()
        {
            if (!CameraTransform) return;

            _transform.eulerAngles = CameraTransform.eulerAngles;
        }
    }
}