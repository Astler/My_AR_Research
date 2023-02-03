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

        private readonly List<MannaBoxView> _beams = new();
        private readonly List<ARAnchorFollower> _anchors = new();

        private ARWorldCoordinator _coordinator;
        private IDataProxy _dataProxy;
        private ICameraView _arCameraView;
        private IDisposable _zonesPlacer;

        [Inject]
        public void Construct(ARWorldCoordinator coordinator,
            IDataProxy dataProxy, CamerasController camerasController)
        {
            _arCameraView = camerasController.GetArCameraView();
            _dataProxy = dataProxy;
            _coordinator = coordinator;
        }

        private void Start()
        {
            if (Application.isEditor)
            {
                _dataProxy.DropZones.ObserveCountChanged().Subscribe(_ => PlaceZonesByMap()).AddTo(this);
            }
            else
            {
                ARSession.stateChanged += OnStateChanged;
            }

            _dataProxy.ActiveBoxes.ObserveCountChanged().Subscribe(_ => TryToUpdateBoxes()).AddTo(this);

            _dataProxy.ActiveEventData
                .CombineLatest(_dataProxy.ScannedArea, (data, scanned) => (data, scanned)).Subscribe(
                    _ => TryToUpdateBoxes()).AddTo(this);
        }

        private void TryToUpdateBoxes()
        {
            if (_dataProxy.ActiveEventData.Value == null)
            {
                Clear();
                return;
            }

            if (!_dataProxy.IsRequestedAreaScanned())
            {
                return;
            }

            UpdateDropBoxes();
        }

        private void UpdateDropBoxes()
        {
            List<ActiveBoxData> activeBoxes = _dataProxy.ActiveBoxes.ToList();

            foreach (MannaBoxView mannaBoxView in _beams.ToList())
            {
                ActiveBoxData correspondingBox = activeBoxes.FirstOrDefault(it => it.id == mannaBoxView.DropId);

                if (correspondingBox == null)
                {
                    mannaBoxView.gameObject.Destroy();
                    _beams.Remove(mannaBoxView);
                }
                else
                {
                    activeBoxes.Remove(correspondingBox);
                }
            }

            foreach (ActiveBoxData activeBoxData in activeBoxes)
            {
                CreateNewDropBox(activeBoxData);
            }
        }

        private void OnDestroy()
        {
            ARSession.stateChanged -= OnStateChanged;
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs stateArgs)
        {
            if (stateArgs.state != ARSessionState.SessionTracking)
            {
                _zonesPlacer?.Dispose();
                return;
            }

            _zonesPlacer = _dataProxy.ScannedArea.Where(it => it >= 1).Take(1).Subscribe(_ => { PlaceZonesByMap(); })
                .AddTo(this);
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

            foreach (DropZoneViewInfo portalZoneModel in _dataProxy.GetAllActiveZones())
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

        private void CreateNewDropBox(ActiveBoxData boxData)
        {
            if (!_dataProxy.IsInsideEvent() || boxData == null) return;

            Vector3 playerPosition = _arCameraView.Transform.position;
            Vector3 shiftPosition = Random.insideUnitSphere *
                                    float.Parse(boxData.point, CultureInfo.InvariantCulture.NumberFormat);
            Vector3 dropPosition = playerPosition + shiftPosition;

            List<ARRaycastHit> planes = RaycastFallback(xrOrigin, arPlaneManager,
                new Ray(new Vector3(shiftPosition.x, 0f, shiftPosition.y), Vector3.down),
                TrackableType.PlaneWithinInfinity).ToList();

            if (!planes.Any())
            {
                dropPosition.y = 0;
                Debug.Log("no planes found!");
                Debug.Log("leave beamPosition = " + dropPosition);
            }
            else
            {
                foreach (ARRaycastHit arRaycastHit in planes)
                {
                    Debug.Log("i found plane! " + arRaycastHit.pose.position);
                }

                dropPosition.y = planes.Last().pose.position.y;
                Debug.Log("updated beamPosition = " + dropPosition);
            }

            dropPosition.y -= 1;

            PlaceDropBoxInWorld(boxData, dropPosition);
        }

        private void PlaceDropBoxInWorld(ActiveBoxData boxData, Vector3 position)
        {
            MannaBoxView mannaBox = Instantiate(beamPrefab, position, Quaternion.identity);

            mannaBox.SetBeamData(boxData.id);

            mannaBox.Interacted += collectable =>
            {
                _dataProxy.RemoveFromAvailableBoxes(mannaBox.DropId);
                _dataProxy.RemoveFromAvailableToCollectDrops(collectable);
            };

            mannaBox.CollectableStatusChanged += tuple =>
            {
                (ICollectable collectable, bool canBeCollected) = tuple;

                if (canBeCollected)
                {
                    _dataProxy.AddToAvailableToCollectDrops(collectable);
                }
                else
                {
                    _dataProxy.RemoveFromAvailableToCollectDrops(collectable);
                }
            };

            _beams.Add(mannaBox);

            Debug.Log("Placed " + boxData.id + " at " + position);
        }

        private void Clear()
        {
            foreach (ARAnchorFollower arAnchorFollower in _anchors)
            {
                arAnchorFollower.gameObject.Destroy();
            }

            _anchors.Clear();

            foreach (MannaBoxView mannaBoxView in _beams)
            {
                mannaBoxView.gameObject.Destroy();
            }

            _beams.Clear();

            ARLocationManager.Instance.Restart();

            PlaceZonesByMap();
        }

        private IEnumerable<ARRaycastHit> RaycastFallback(XROrigin origin, ARPlaneManager planeManager,
            Ray worldSpaceRay, TrackableType trackableTypeMask)
        {
            Transform trackablesParent = origin.TrackablesParent;
            Ray sessionSpaceRay = TransformExtensions.InverseTransformRay(trackablesParent, worldSpaceRay);
            NativeArray<XRRaycastHit> hits = planeManager.Raycast(sessionSpaceRay, trackableTypeMask, Allocator.Temp);

            if (!hits.IsCreated) return Enumerable.Empty<ARRaycastHit>();

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
    }
}