using UnityEngine;

namespace GameCamera
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _transform;
        private Transform _cameraControlTransform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            if (Camera.main) _cameraControlTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            _transform.eulerAngles = _cameraControlTransform.eulerAngles;
        }
    }
}