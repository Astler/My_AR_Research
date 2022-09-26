using UnityEngine;

namespace Prototype.AR
{
    public interface IARController
    {
        (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition);
        CameraView GetCamera();
        Vector3 GetPointerPosition();
        void Reset();
    }
}