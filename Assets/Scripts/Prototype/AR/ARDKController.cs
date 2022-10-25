using System;
using System.Collections.Generic;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Anchors;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Prototype.AR.FoundationAR;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype.AR
{
    public class ARDKController : MonoBehaviour, IARController
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private ARDKTargetSelectView targetSelectView;

        private IARSession _session;
        private readonly List<IARAnchor> _anchors = new();
        private readonly ReactiveProperty<bool> _initialized = new();
        public IReadOnlyReactiveProperty<bool> Initialized => _initialized;

        public (bool hasHits, Pose? poseTransform) CheckIfRaycastHits(Vector2 clickPosition)
        {
            Vector3 position = targetSelectView.GetARPositionByScreenPosition(clickPosition);
            return (position != Vector3.zero, new Pose(position, Quaternion.identity));
        }

        public CameraView GetCamera() => cameraView;

        public Vector3 GetPointerPosition() => targetSelectView.GetPointerPosition();

        public Vector3 GetCeilPosition() => targetSelectView.GetCeilPosition();

        private void Start()
        {
            ARSessionFactory.SessionInitialized += OnAnyARSessionDidInitialize;
        }

        private void OnAnyARSessionDidInitialize(AnyARSessionInitializedArgs args)
        {
            _session = args.Session;
            _initialized.Value = true;
        }

        public IARAnchor AddAnchor(Vector2 position)
        {
            Matrix4x4 matrix = new();
            matrix.SetTRS(position, Quaternion.identity, Vector3.one);
            // matrix.SetTRS(new Vector3(position.x, 0f, position.y), Quaternion.identity, Vector3.one);
            
            IARAnchor anchor = _session.AddAnchor(matrix);
            
            _anchors.Add(anchor);

            return anchor;
        }

        public void ClearAnchors()
        {
            foreach (IARAnchor arAnchor in _anchors)
            {
                _session.RemoveAnchor(arAnchor);
            }

            _anchors.Clear();
        }

        public IARSession GetSession() => _session;

        public void Reset()
        {
            SceneManager.LoadScene("SelectedScene");
        }
    }
}