using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using UniRx;
using UnityEngine;

namespace Prototype.AR.FoundationAR
{
    public interface IARController
    {
        IReadOnlyReactiveProperty<bool> Initialized { get; }
        (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition);
        CameraView GetCamera();
        Vector3 GetPointerPosition();
        Vector3 GetCeilPosition();
        IARAnchor AddAnchor(Vector2 position, Quaternion rotation = default);
        void Reset();
        void ClearAnchors();
        IARSession GetSession();
    }
}