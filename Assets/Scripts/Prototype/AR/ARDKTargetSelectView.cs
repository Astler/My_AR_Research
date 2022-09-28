using System.Collections.ObjectModel;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;
using UnityEngine;

namespace Prototype.AR
{
    public class ARDKTargetSelectView : MonoBehaviour
    {
        public ARHitTestResultType HitTestType = ARHitTestResultType.ExistingPlane;
        [SerializeField] private Transform targetPlacement;

        private Vector2 _screenCenter;
        private Vector3 _position;
        private IARSession _session;
        private IARController _arController;

        public Vector3 GetPointerPosition() => _position;

        private void Start()
        {
            ARSessionFactory.SessionInitialized += OnAnyARSessionDidInitialize;
            int width = Screen.width / 2;
            int height = Screen.height / 2;
            _screenCenter = new Vector2(width, height);
        }

        private void OnAnyARSessionDidInitialize(AnyARSessionInitializedArgs args)
        {
            _session = args.Session;
        }

        private void OnDestroy()
        {
            ARSessionFactory.SessionInitialized -= OnAnyARSessionDidInitialize;

            _session = null;
        }

        private void Update()
        {
            GetARPositionByScreenPosition(_screenCenter);
            targetPlacement.position = _position;
        }

        public Vector3 GetARPositionByScreenPosition(Vector2 screenPosition)
        {
            IARFrame currentFrame = _session?.CurrentFrame;

            if (currentFrame == null) return Vector3.zero;

            ReadOnlyCollection<IARHitTestResult> results = currentFrame.HitTest
            (
                _arController.GetCamera().PixelWidth,
                _arController.GetCamera().PixelHeight,
                screenPosition,
                HitTestType
            );

            int count = results.Count;

            if (count <= 0)
                return Vector3.zero;

            // Get the closest result
            var result = results[0];

            _position = result.WorldTransform.ToPosition();

            return _position;
        }

        public Quaternion GetPointerRotation() => targetPlacement.rotation;

        private void Awake()
        {
            _arController = FindObjectOfType<ARDKController>();
        }
    }
}