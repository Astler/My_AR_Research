using GameCamera;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.AR.FoundationAR
{
    public interface IARController
    {
        IReadOnlyReactiveProperty<bool> Initialized { get; }
        (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition);
        CameraView GetCamera();
        Vector3 GetPointerPosition();
        Vector3 GetCeilPosition();
        void Reset();
        void ClearAnchors();
    }
}