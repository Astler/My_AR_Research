using UnityEngine;

namespace Prototype.AR
{
    public class ARDKController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ARDKTargetSelectView targetSelectView;

        public (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition)
        {
            Vector3 position = targetSelectView.GetARPositionByScreenPosition(clickPosition);
            return (position != Vector3.zero, new Pose(position, Quaternion.identity));
        }

        public CameraView GetCamera() => cameraView;

        public Vector3 GetPointerPosition() => targetSelectView.GetPointerPosition();

        public void Reset()
        {
        }
    }
}