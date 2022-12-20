using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AR.World;
using AR.World.Collectable;
using ARLocation;
using Data;
using Data.Objects;
using GameCamera;
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
        [SerializeField] private ARAnchorFollower zonePrefab;
        [SerializeField] private MannaBoxView beamPrefab;
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private ARPlaneManager arPlaneManager;

        private ARAnchorFollower _pointMe;
        private readonly List<ARAnchorFollower> _anchors = new();
        private readonly List<MannaBoxView> _beams = new();

        private readonly List<BeamData> _beamsData = new();
        private ARAnchorFollower _zeroAnchor;

        private ARWorldCoordinator _coordinator;
        private IDataProxy _dataProxy;
        private CoinsController _coinsController;
        private CameraView _cameraView;
        private int _beamsInThisSession;

        [Inject]
        public void Construct(ARWorldCoordinator coordinator,
            IDataProxy dataProxy, CoinsController coinsController, CameraView cameraView)
        {
            _cameraView = cameraView;
            _coinsController = coinsController;
            _dataProxy = dataProxy;
            _coordinator = coordinator;
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

            _dataProxy.PlaceRewardBoxInsideZone.Subscribe(CreateNewReward).AddTo(this);
            _dataProxy.RemoveRewardBox.Subscribe(RemoveReward).AddTo(this);

            _dataProxy.ActiveEventData.Subscribe(zone =>
            {
                if (zone == null)
                {
                    _anchors.Clear();
                    _beams.Clear();
                    _beamsData.Clear();
                    Clear();
                    return;
                }

                foreach (ActiveBoxData boxData in zone.active_boxes)
                {
                    CreateNewReward(boxData);
                }
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            ARSession.stateChanged -= OnStateChanged;
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs stateArgs)
        {
            if (stateArgs.state != ARSessionState.SessionTracking) return;

            PlaceZonesByMap();
            ARSession.stateChanged -= OnStateChanged;
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

        private void CreateNewReward(ActiveBoxData boxData)
        {
            if (!_dataProxy.IsInsideEvent() || boxData == null) return;

            Vector3 playerPosition = _cameraView.transform.position;
            Vector3 beamPosition = Random.insideUnitSphere *
                                   float.Parse(boxData.point, CultureInfo.InvariantCulture.NumberFormat);
            Vector3 objectPosition = playerPosition + beamPosition;

            IEnumerable<ARRaycastHit> planes = RaycastFallback(xrOrigin, arPlaneManager,
                new Ray(new Vector3(beamPosition.x, 0f, beamPosition.y), Vector3.down),
                TrackableType.PlaneWithinInfinity);

            if (!planes.Any())
            {
                objectPosition.y = 0;
                Debug.Log("no planes found!");
                Debug.Log("leave beamPosition = " + objectPosition);
            }
            else
            {
                objectPosition.y = planes.Last().pose.position.y;
                Debug.Log("updated beamPosition = " + objectPosition);
            }

            _beamsInThisSession += 1;

            BeamData beamData = new()
            {
                Position = objectPosition,
                Name = "beam " + _beamsInThisSession,
                Url = "",
                Id = boxData.id
            };

            _beamsData.Add(beamData);

            PlaceBeamsInWorld();
        }

        private void RemoveReward(ActiveBoxData boxData)
        {
            BeamData beamData = _beamsData.FirstOrDefault(it => it.Id == boxData.id);

            if (beamData == null) return;

            MannaBoxView beam = _beams.FirstOrDefault(it => it.GetBeamData() == beamData);

            _beamsData.Remove(beamData);

            Debug.Log("RemoveReward " + beamData.Id);

            if (beam == null) return;

            _beams.Remove(beam);
            beam.gameObject.Destroy();
        }

        private void PlaceBeamsInWorld()
        {
            foreach (MannaBoxView arAnchorFollower in _beams)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _beams.Clear();

            foreach (BeamData data in _beamsData)
            {
                MannaBoxView follower =
                    Instantiate(beamPrefab, data.Position, Quaternion.identity);

                follower.SetBeamData(data);
                follower.SetBoxName(data.Name);

                follower.SetupCollectAction(() =>
                {
                    foreach (BeamData beamData in _beamsData.ToList().Where(beamData => beamData.Id == data.Id))
                    {
                        _beamsData.Remove(beamData);
                    }

                    if (follower)
                    {
                        _coinsController.SpawnCoinsAtPosition(follower.transform.position);
                        follower.gameObject.Destroy();
                    }

                    PlaceBeamsInWorld();
                });

                _beams.Add(follower);

                Debug.Log("Placed " + data.Name + " at " + data.Position);
            }
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

        private IEnumerable<ARRaycastHit> RaycastFallback(XROrigin origin, ARPlaneManager planeManager,
            Ray worldSpaceRay, TrackableType trackableTypeMask)
        {
            Transform trackablesParent = origin.TrackablesParent;
            Ray sessionSpaceRay = TransformExtensions.InverseTransformRay(trackablesParent, worldSpaceRay);
            NativeArray<XRRaycastHit> hits = planeManager.Raycast(sessionSpaceRay, trackableTypeMask, Allocator.Temp);

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