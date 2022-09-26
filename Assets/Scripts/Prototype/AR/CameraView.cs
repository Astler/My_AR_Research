using UnityEngine;
using Vuforia;

namespace Prototype.AR
{
    public class CameraView : MonoBehaviour
    {
        private Camera _camera;

        public Vector3 CameraForwardVector => _camera.transform.forward;

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
            Camera vuforiaCamera = VuforiaBehaviour.Instance.GetComponent<Camera>();
            _camera = vuforiaCamera ? vuforiaCamera : Camera.main;
        }

        public Transform GetTransform() => _camera.transform;
    }
}