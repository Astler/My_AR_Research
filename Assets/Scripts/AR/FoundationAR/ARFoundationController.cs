using System.Collections.Generic;
using AR.World;
using GameCamera;
using Geo;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using UniRx;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;

namespace AR.FoundationAR
{
    public class ARFoundationController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private TargetSelectView targetView;
        [SerializeField] private ARSession arSession;
        [SerializeField] private Transform content;

        private ARWorldCoordinator _coordinator;
        private LocationController _locationController;
        private XROrigin _origin;

        [Inject]
        public void Construct(ARWorldCoordinator coordinator, LocationController locationController)
        {
            _coordinator = coordinator;
            _locationController = locationController;
        }

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

        public IARAnchor AddAnchor(Vector3 position, Quaternion rotation = default)
        {
            return null;
        }

        public void Reset() => arSession.Reset();
        public void ClearAnchors() { }

        public IARSession GetSession()
        {
            return null;
        }

        public CameraView GetCamera() => cameraView;
    }
}