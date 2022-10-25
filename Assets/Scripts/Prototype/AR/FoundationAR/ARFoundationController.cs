using System.Collections.Generic;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Prototype.AR.FoundationAR
{
    public class ARFoundationController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private TargetSelectView targetView;
        [SerializeField] private ARSession arSession;

        public IReadOnlyReactiveProperty<bool> Initialized { get; }

        public (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition)
        {
            List<ARRaycastHit> arHits = new();
            bool hit = raycastManager.Raycast(clickPosition, arHits, TrackableType.Planes);

            if (arHits.Count > 0)
            {
                Pose pose = arHits[0].pose;
                return (hit, pose);
            }

            return (hit, null);
        }

        public Vector3 GetPointerPosition() => targetView.GetPointerPosition();
        
        public Vector3 GetCeilPosition()
        {
            return Vector3.zero;
        }

        public IARAnchor AddAnchor(Vector2 position)
        {
            return null;
        }

        public void Reset() => arSession.Reset();
        public void ClearAnchors()
        {
            
        }

        public IARSession GetSession()
        {
            return null;
        }

        public CameraView GetCamera() => cameraView;
    }
}