using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR.FoundationAR
{
    public class TargetSelectView : MonoBehaviour
    {
        [SerializeField] private Transform targetPlacement;
        [SerializeField] private ARRaycastManager rayManager;

        private Vector2 _screenCenter;

        private void Start()
        {
            int width = Screen.width / 2;
            int height = Screen.height / 2;
            _screenCenter = new Vector2(width, height);
        }

        private void Update()
        {
            List<ARRaycastHit> hits = new();

            rayManager.Raycast(_screenCenter, hits, TrackableType.Planes);

            if (hits.Count <= 0)
            {
                targetPlacement.gameObject.SetActive(false);
                return;
            }

            targetPlacement.position = hits[0].pose.position;
            targetPlacement.rotation = hits[0].pose.rotation;
            
            if (!targetPlacement.gameObject.activeSelf)
                targetPlacement.gameObject.SetActive(true);
        }

        public Vector3 GetPointerPosition() => targetPlacement.position;

        public Quaternion GetPointerRotation() => targetPlacement.rotation;
    }
}