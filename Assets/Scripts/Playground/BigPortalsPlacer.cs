using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Prototype;
using Prototype.Assets;
using UniRx;
using UnityEngine;

namespace Playground
{
    public class BigPortalsPlacer : MonoBehaviour
    {
        [SerializeField] private SpawnOnMapAR portalPrefab;
        [SerializeField] private ARAnchorFollower bigPortal;
        [SerializeField] private ARAnchorFollower controlPoint;
        [SerializeField] private ARAnchorFollower movePoint;
        [SerializeField] private AbstractMap abstractMap;
        [SerializeField] private Transform arMapCamera;
        [SerializeField] private Transform mainCamera;
        private ProjectContext _context;
        private ARAnchorFollower _pointMe;
        private readonly List<SpawnOnMapAR> _anchors = new();
        private ARAnchorFollower _zeroAnchor;
        private float _deviceRotation;
        private float _startCompassRotation;
        private Gyroscope _gyro;
        private Quaternion _rotation = new(0, 0, 1, 0);
        private bool _gyroEnabled;
        private ARAnchorFollower _mapFollower;
        private float _headingVelocity = 0f;
        private float _rotationToNorth;

        private void Start()
        {
            _context = FindObjectOfType<ProjectContext>();

            _context.GetARController().Initialized
                .Where(x => x)
                .Subscribe(_ => PlaceObjects());

            _gyroEnabled = EnableGyro();
        }

        private bool EnableGyro()
        {
            if (SystemInfo.supportsGyroscope)
            {
                _gyro = Input.gyro;
                _gyro.enabled = true;
                return true;
            }

            return false;
        }

        private void PlaceObjects()
        {
            _startCompassRotation = Input.compass.trueHeading;
            // _deviceRotation = -_startCompassRotation + camera.transform.rotation.eulerAngles.y;
            abstractMap.Root.rotation =
                Quaternion.Euler(new Vector3(0f, -_startCompassRotation + arMapCamera.rotation.eulerAngles.y, 0f));

            _context.GetARController().GetSession().CameraTrackingStateChanged += changes =>
            {
                _startCompassRotation = Input.compass.trueHeading;
                Debug.Log("rebuild this shit plase");
                Debug.Log($"pre rebuild rotation = {abstractMap.Root.rotation.eulerAngles.y}");
                abstractMap.Root.rotation = Quaternion.Euler(new Vector3(0f,
                    -_startCompassRotation + changes.Camera.Transform.rotation.eulerAngles.y, 0f));
                Debug.Log($"post rebuild rotation = {abstractMap.Root.rotation.eulerAngles.y}");
            };

            _context.GetARController().GetSession().Ran += args => { Debug.Log("RAN!"); };

            _context.GetLocationController().PlayerLocationChanged.Subscribe(delegate(Vector2 position)
            {
                _context.ClearAnchors();

                foreach (SpawnOnMapAR arAnchorFollower in _anchors)
                {
                    arAnchorFollower.gameObject.Destroy();
                }

                _anchors.Clear();

                foreach (PortalZoneModel portalZoneModel in _context.GetAssets().portalZones.Where(it => it.isActive))
                {
                    SpawnOnMapAR element = Instantiate(portalPrefab);
                    element.enabled = false;
                    element.Map = abstractMap;
                    element.SpawnPoint = portalZoneModel.GetPosition2d();
                    element.enabled = true;

                    _anchors.Add(element);
                }
            }).AddTo(this);
        }

        private void Update()
        {
            arMapCamera.rotation = mainCamera.rotation;
            // arMapCamera.position = mainCamera.position;
        }
    }
}