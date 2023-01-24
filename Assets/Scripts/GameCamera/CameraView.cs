using UnityEngine;

namespace GameCamera
{
    public interface ICameraView
    {
        Transform Transform { get; }
        CameraType CameraType { get; }
        void ActivateByCameraType(CameraType type);
        Vector3 GetFacePosition();
    }

    public class CameraView : MonoBehaviour, ICameraView
    {
        [SerializeField] private Transform facePosition;
        [SerializeField] private CameraType cameraType;

        private Camera _camera;
        private Transform _transform;

        public Vector3 CameraForwardVector => _camera.transform.forward;
        public int PixelHeight => _camera.pixelHeight;
        public int PixelWidth => _camera.pixelWidth;

        public Transform Transform => _transform;
        public CameraType CameraType => cameraType;

        public void ActivateByCameraType(CameraType type)
        {
            gameObject.SetActive(CameraType == type);
        }

        public Ray CameraRay(Vector2 clickPosition) => _camera.ScreenPointToRay(clickPosition);

        public RaycastHit[] GetHitsByMousePosition(Vector2 clickPosition)
        {
            return Physics.RaycastAll(CameraRay(clickPosition));
        }

        public RaycastHit FirstHitByMousePosition(Vector2 clickPosition)
        {
            return GetHitsByMousePosition(clickPosition)[0];
        }

        private void Awake()
        {
            _camera = Camera.main;
            _transform = transform;
        }

        public Transform GetTransform() => _camera.transform;

        public Vector3 GetFacePosition() => facePosition.position;

        public Vector3 GetPosition() => _camera.transform.position;
    }
}