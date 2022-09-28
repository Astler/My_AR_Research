using UnityEngine;
using Vuforia;

namespace Prototype.AR
{
    public class CameraView : MonoBehaviour
    {
        private Camera _camera;

        public Vector3 CameraForwardVector => _camera.transform.forward;
        public int PixelHeight => _camera.pixelHeight;
        public int PixelWidth => _camera.pixelWidth;

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
            Camera foundCamera = null;

            if (VuforiaBehaviour.Instance != null)
            {
                foundCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
            }

            _camera = foundCamera ? foundCamera : Camera.main;
        }

        public Transform GetTransform() => _camera.transform;
    }
}