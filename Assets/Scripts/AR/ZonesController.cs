using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AR.World;
using AR.World.Collectable;
using ARLocation;
using Assets;
using Assets.Scripts.AR.FoundationAR;
using BestHTTP.SignalR;
using Data;
using Data.Objects;
using GameCamera;
using Geo;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UniRx;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Utils;
using Zenject;
using Random = UnityEngine.Random;
using TransformExtensions = UnityEngine.XR.ARFoundation.TransformExtensions;

namespace AR
{
    public class ZonesController : MonoBehaviour
    {
        [SerializeField] private float giftSpawnTime = 5f;
        [SerializeField] private ARAnchorFollower zonePrefab;
        [SerializeField] private MannaBoxView beamPrefab;
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private ARPlaneManager arPlaneManager;

        private ARAnchorFollower _pointMe;
        private readonly List<ARAnchorFollower> _anchors = new();
        private readonly List<MannaBoxView> _beams = new();

        private readonly List<BeamData> _beamsData = new();
        private ARAnchorFollower _zeroAnchor;
        private float _startDeviceRotation;
        private double _lastRotation;
        private Vector2d _lastPosition;

        private IARController _arController;
        private LocationController _locationController;
        private AssetsScriptableObject _assetsScriptableObject;
        private ARWorldCoordinator _coordinator;
        private IDataProxy _dataProxy;
        private CoinsController _coinsController;
        private IDisposable _newBeamTimer;
        private CameraView _cameraView;

        [Inject]
        public void Construct(IARController arController, LocationController locationController,
            AssetsScriptableObject assetsScriptableObject, ARWorldCoordinator coordinator,
            IDataProxy dataProxy, CoinsController coinsController, CameraView cameraView)
        {
            _cameraView = cameraView;
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

            _dataProxy.PlaceRandomBeamForSelectedZone.Subscribe(_ =>
            {
                _newBeamTimer?.Dispose();
                _newBeamTimer = CreateNewBeam().ToObservable().Subscribe();
            }).AddTo(this);
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs stateArgs)
        {
            if (stateArgs.state != ARSessionState.SessionTracking) return;

            PlaceZonesByMap();
            ARSession.stateChanged -= OnStateChanged;
        }

        private void Clear()
        {
            foreach (ARAnchorFollower arAnchorFollower in _anchors)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _anchors.Clear();

            foreach (MannaBoxView arAnchorFollower in _beams)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _beams.Clear();

            _beamsData.Clear();

            ARLocationManager.Instance.Restart();

            PlaceZonesByMap();
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

            foreach (ZoneViewInfo portalZoneModel in _dataProxy.GetAllActiveZones())
            {
                Vector2 objectPosition = Conversions.GeoToWorldPosition(portalZoneModel.Coordinates.ToVector2d(),
                    playerPosition.ToVector2d()).ToUnityVector();

                ARAnchorFollower follower =
                    Instantiate(zonePrefab, new Vector3(objectPosition.x, 0f, objectPosition.y), Quaternion.identity,
                        _coordinator.GetContentTransform());

                follower.SetZoneScale(portalZoneModel.Radius);

                Location location = new()
                {
                    Latitude = portalZoneModel.Coordinates.x,
                    Longitude = portalZoneModel.Coordinates.y,
                    Altitude = 0,
                    AltitudeMode = AltitudeMode.DeviceRelative,
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

        private IEnumerator CreateNewBeam()
        {
            ZoneViewInfo selectedZone = _dataProxy.SelectedPortalZone.Value;

            if (selectedZone == null) yield break;

            RewardViewInfo uncollectedReward = _dataProxy.GetAvailableRewardForZone();

            Debug.Log("there is no active rewards!");

            if (uncollectedReward == null)
            {
                Debug.Log("there is no active rewards!");
                yield break;
            }

            Vector3 beamPosition = Random.insideUnitCircle * selectedZone.MaximumDropDistance +
                                   Vector2.one * selectedZone.MinimumDropDistance;

            IEnumerable<ARRaycastHit> planes = RaycastFallback(xrOrigin, arPlaneManager,
                new Ray(new Vector3(beamPosition.x, 0f, beamPosition.y), Vector3.down),
                TrackableType.PlaneWithinInfinity);

            if (!planes.Any())
            {
                Debug.Log("no planes found!");
                Debug.Log("leave beamPosition = " + beamPosition);
            }
            else
            {
                beamPosition.y = planes.Last().pose.position.y;
                Debug.Log("updated beamPosition = " + beamPosition);
            }

            BeamData beamData = new()
            {
                Position = beamPosition,
                Name = uncollectedReward.Name,
                Url = uncollectedReward.Url,
                ZoneId = uncollectedReward.EventId,
                Id = uncollectedReward.Id
            };

            _beamsData.Add(beamData);

            for (int i = 0; i <= giftSpawnTime; i++)
            {
                _dataProxy.SetTimeToNextGift(giftSpawnTime - i);
                yield return new WaitForSeconds(1f);
            }

            PlaceBeamsByMap();
        }

        private void PlaceBeamsByMap()
        {
            Vector3 playerPosition = _cameraView.transform.position;

            foreach (MannaBoxView arAnchorFollower in _beams)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _beams.Clear();

            foreach (BeamData data in _beamsData)
            {
                Vector3 objectPosition = playerPosition + data.Position;

                MannaBoxView follower =
                    Instantiate(beamPrefab, objectPosition, Quaternion.identity);

                follower.SetBeamData(data);

                // follower.WorldCoordinates = data.Position;
                follower.SetBoxName(data.Name);

                follower.SetupCollectAction(() =>
                {
                    foreach (BeamData beamData in _beamsData.ToList().Where(beamData => beamData.Id == data.Id))
                    {
                        _beamsData.Remove(beamData);
                    }

                    follower.gameObject.Destroy();

                    _coinsController.SpawnCoinsAtPosition(follower.transform.position);
                    PlaceBeamsByMap();
                });

                // Location location = new()
                // {
                //     Latitude = data.Position.x,
                //     Longitude = data.Position.y,
                //     Altitude = 0,
                //     AltitudeMode = AltitudeMode.GroundRelative,
                // };
                //
                // PlaceAtLocation.PlaceAtOptions options = new()
                // {
                //     HideObjectUntilItIsPlaced = true,
                //     MaxNumberOfLocationUpdates = 1,
                //     MovementSmoothing = 0.1f,
                //     UseMovingAverage = false
                // };
                //
                // PlaceAtLocation.AddPlaceAtComponent(follower.gameObject, location, options);

                _beams.Add(follower);

                Debug.Log("Placed " + data.Name + " at " + objectPosition);
            }
        }

        public IEnumerable<ARRaycastHit> RaycastFallback(XROrigin origin, ARPlaneManager planeManager,
            Ray worldSpaceRay, TrackableType trackableTypeMask)
        {
            var trackablesParent = origin.TrackablesParent;
            var sessionSpaceRay = TransformExtensions.InverseTransformRay(trackablesParent, worldSpaceRay);
            var hits = planeManager.Raycast(sessionSpaceRay, trackableTypeMask, Allocator.Temp);
            if (hits.IsCreated)
            {
                using (hits)
                {
                    return hits.OrderBy(_ => _.distance).Select(hit =>
                    {
                        float distanceInWorldSpace = (hit.pose.position - worldSpaceRay.origin).magnitude;
                        return new ARRaycastHit(hit, distanceInWorldSpace, trackablesParent,
                            planeManager.GetPlane(hit.trackableId));
                    });
                }
            }
            else
            {
                return Enumerable.Empty<ARRaycastHit>();
            }
        }
    }
}