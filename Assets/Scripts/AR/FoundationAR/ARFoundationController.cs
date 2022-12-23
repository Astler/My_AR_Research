using System.Collections.Generic;
using AR.World;
using Assets.Scripts.AR.FoundationAR;
using Data;
using GameCamera;
using Geo;
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
        [SerializeField] private ARPlaneManager planeManager;

        private ARWorldCoordinator _coordinator;
        private LocationController _locationController;
        private XROrigin _origin;
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(ARWorldCoordinator coordinator, LocationController locationController,
            IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
            _coordinator = coordinator;
            _locationController = locationController;

            ARSession.stateChanged += OnStateChanged;
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs obj)
        {
            Debug.Log("session state changed: " + obj.state);
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

        public void Reset() => arSession.Reset();
        public void ClearAnchors() { }

        public CameraView GetCamera() => cameraView;

        private void Update()
        {
            if (_dataProxy.SurfaceScanned.Value) return;

            float totalArea = 0f;

            foreach (ARPlane plane in planeManager.trackables)
            {
                Vector2 size = plane.size;
                totalArea += size.x * size.y;
            }

            Debug.Log($"area size = {totalArea}");

            _dataProxy.SetScannedArea(totalArea);
        }
    }
}