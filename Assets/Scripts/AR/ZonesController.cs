using System.Collections.Generic;
using System.Linq;
using AR.FoundationAR;
using AR.World;
using ARLocation;
using Assets;
using Data;
using Data.Objects;
using Geo;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Niantic.ARDK.LocationService;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;
using Zenject;

namespace AR
{
    public class ZonesController : MonoBehaviour
    {
        [SerializeField] private ARAnchorFollower zonePrefab;
        [SerializeField] private ARAnchorFollower beamPrefab;

        private ARAnchorFollower _pointMe;
        private readonly List<ARAnchorFollower> _anchors = new();
        private readonly List<ARAnchorFollower> _beams = new();

        private readonly List<BeamData> _beamsData = new();
        private ARAnchorFollower _zeroAnchor;
        private float _startDeviceRotation;
        private ILocationService _locationService;
        private double _lastRotation;
        private Vector2d _lastPosition;

        private IARController _arController;
        private LocationController _locationController;
        private AssetsScriptableObject _assetsScriptableObject;
        private ARWorldCoordinator _coordinator;
        private IDataProxy _dataProxy;
        private CoinsController _coinsController;

        [Inject]
        public void Construct(IARController arController, LocationController locationController,
            AssetsScriptableObject assetsScriptableObject, ARWorldCoordinator coordinator,
            IDataProxy dataProxy, CoinsController coinsController)
        {
            _coinsController = coinsController;
            _dataProxy = dataProxy;
            _coordinator = coordinator;
            _arController = arController;
            _locationController = locationController;
            _assetsScriptableObject = assetsScriptableObject;
        }

        private void Start()
        {
            _dataProxy.Clear.Subscribe(_ => Clear());

            if (Application.isEditor)
            {
                _dataProxy.PlayerLocationChanged.Subscribe(delegate(Vector2 position) { PlaceZonesByMap(); })
                    .AddTo(this);
            }
            else
            {
                ARSession.stateChanged += OnStateChanged;
            }

            _dataProxy.PlaceRandomBeamForSelectedZone.Subscribe(_ => { CreateNewBeam(); }).AddTo(this);
        }

        private void Clear()
        {
            foreach (ARAnchorFollower arAnchorFollower in _anchors)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _anchors.Clear();

            foreach (ARAnchorFollower arAnchorFollower in _beams)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _beams.Clear();

            _beamsData.Clear();

            ARLocationManager.Instance.Restart();
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs stateArgs)
        {
            if (stateArgs.state != ARSessionState.SessionTracking) return;

            PlaceZonesByMap();
            ARSession.stateChanged -= OnStateChanged;
        }

        private void PlaceObjectsAnchors(Vector2d playerPositionRaw)
        {
            float compass = Input.compass.trueHeading;

            foreach (ARAnchorFollower arAnchorFollower in _anchors)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _anchors.Clear();

            Vector2 playerPosition = Conversions.GeoToWorldPosition(playerPositionRaw,
                Vector2d.zero).ToUnityVector();

            //TODO active check .Where(it => it.isActive)
            foreach (PortalViewInfo portalZoneModel in _dataProxy.GetAllZones())
            {
                Vector2 objectPosition = Conversions.GeoToWorldPosition(portalZoneModel.Coordinates.ToVector2d(),
                    playerPosition.ToVector2d()).ToUnityVector();

                ARAnchorFollower follower =
                    Instantiate(zonePrefab, new Vector3(objectPosition.x, 0f, objectPosition.y), Quaternion.identity,
                        _coordinator.GetContentTransform());
                follower.gameObject.AddComponent<ARAnchor>();
                follower.name = portalZoneModel.Name;

                _anchors.Add(follower);

                Debug.Log("Placed " + portalZoneModel.Name + " at " + objectPosition);
            }
        }

        private void PlaceZonesByMap()
        {
            Vector2d playerPositionRaw = _dataProxy.GetPlayerPosition().ToVector2d();

            foreach (ARAnchorFollower arAnchorFollower in _anchors)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _anchors.Clear();

            Vector2 playerPosition = Conversions.GeoToWorldPosition(playerPositionRaw,
                Vector2d.zero).ToUnityVector();

            //TODO active check .Where(it => it.isActive)
            foreach (PortalViewInfo portalZoneModel in _dataProxy.GetAllZones())
            {
                Vector2 objectPosition = Conversions.GeoToWorldPosition(portalZoneModel.Coordinates.ToVector2d(),
                    playerPosition.ToVector2d()).ToUnityVector();

                ARAnchorFollower follower =
                    Instantiate(zonePrefab, new Vector3(objectPosition.x, 0f, objectPosition.y), Quaternion.identity,
                        _coordinator.GetContentTransform());

                //TODO restore radius 
                float radius = 100;
                follower.SetZoneScale(radius);

                Location location = new()
                {
                    Latitude = portalZoneModel.Coordinates.x,
                    Longitude = portalZoneModel.Coordinates.y,
                    Altitude = 0,
                    AltitudeMode = AltitudeMode.GroundRelative,
                };

                PlaceAtLocation.PlaceAtOptions options = new()
                {
                    HideObjectUntilItIsPlaced = true,
                    MaxNumberOfLocationUpdates = 1,
                    MovementSmoothing = 0.1f,
                    UseMovingAverage = false
                };

                PlaceAtLocation.AddPlaceAtComponent(follower.gameObject, location, options);
                follower.name = portalZoneModel.Name;

                _anchors.Add(follower);
            }
        }

        private void CreateNewBeam()
        {
            PortalViewInfo selectedZone = _dataProxy.SelectedPortalZone.Value;

            if (selectedZone == null) return;

            //TODO radius 
            float radius = 100;
            _beamsData.Add(new BeamData
            {
                Position = CoordinatesUtils.GetRandomWorldPositionInRadius(selectedZone.Coordinates,
                    radius),
                Name = "Beam" + _beamsData.Count,
            });

            PlaceBeamsByMap();
        }

        private void PlaceBeamsByMap()
        {
            Vector2d playerPositionRaw = _dataProxy.GetPlayerPosition().ToVector2d();

            foreach (ARAnchorFollower arAnchorFollower in _beams)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _beams.Clear();

            Vector2 playerPosition = Conversions.GeoToWorldPosition(playerPositionRaw,
                Vector2d.zero).ToUnityVector();

            foreach (BeamData data in _beamsData)
            {
                Vector2 objectPosition = Conversions.GeoToWorldPosition(data.Position.ToVector2d(),
                    playerPosition.ToVector2d()).ToUnityVector();

                ARAnchorFollower follower =
                    Instantiate(beamPrefab, new Vector3(objectPosition.x, 0f, objectPosition.y), Quaternion.identity,
                        _coordinator.GetContentTransform());

                follower.WorldCoordinates = data.Position;
                follower.SetName(data.Name);

                follower.SetupClick(() =>
                {
                    _beamsData.Remove(data);
                    _beams.Remove(follower);
                    follower.gameObject.Destroy();
                    _coinsController.SpawnCoinsAtPosition(follower.transform.position);
                });

                Location location = new()
                {
                    Latitude = data.Position.x,
                    Longitude = data.Position.y,
                    Altitude = 0,
                    AltitudeMode = AltitudeMode.GroundRelative,
                };

                PlaceAtLocation.PlaceAtOptions options = new()
                {
                    HideObjectUntilItIsPlaced = true,
                    MaxNumberOfLocationUpdates = 1,
                    MovementSmoothing = 0.1f,
                    UseMovingAverage = false
                };

                PlaceAtLocation.AddPlaceAtComponent(follower.gameObject, location, options);

                _beams.Add(follower);

                Debug.Log("Placed " + data.Name + " at " + objectPosition);
            }
        }
    }
}