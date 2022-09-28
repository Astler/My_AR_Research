using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Prototype.AR
{
    public class ARController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private TargetSelectView targetView;
        [SerializeField] private ARSession arSession;

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

        public void Reset() => arSession.Reset();
        
        public CameraView GetCamera() => cameraView;
    }
}