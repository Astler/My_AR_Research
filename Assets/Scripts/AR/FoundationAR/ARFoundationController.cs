using System.Collections.Generic;
using Data;
using GameCamera;
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
        [SerializeField] private ARPlaneManager planeManager;

        private XROrigin _origin;
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
        }
        
        public (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition)
        {
            List<ARRaycastHit> arHits = new();
            bool hit = raycastManager.Raycast(clickPosition, arHits, TrackableType.Planes);

            if (arHits.Count <= 0) return (hit, null);

            Pose pose = arHits[0].pose;
            return (hit, pose);
        }

        public Vector3 GetPointerPosition() => targetView.GetPointerPosition();

        public Vector3 GetCeilPosition() => Vector3.zero;

        public void Reset() => arSession.Reset();

        public void ClearAnchors() { }

        public CameraView GetCamera() => cameraView;

        private void Update()
        {
            if (_dataProxy.IsRequestedAreaScanned()) return;

            float totalArea = 0f;

            foreach (ARPlane plane in planeManager.trackables)
            {
                Vector2 size = plane.size;
                totalArea += size.x * size.y;
            }
            
            _dataProxy.SetScannedArea(totalArea);
        }
    }
}