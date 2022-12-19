using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AR.World;
using AR.World.Collectable;
using ARLocation;
using Assets;
using Assets.Scripts.AR.FoundationAR;
using Data;
using Data.Objects;
using GameCamera;
using Geo;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;
using Vuforia;
using Zenject;
using Random = UnityEngine.Random;

namespace AR
{
    public class ZonesController : MonoBehaviour
    {
        [SerializeField] private float giftSpawnTime = 5f;
        [SerializeField] private ARAnchorFollower zonePrefab;
        [SerializeField] private MannaBoxView beamPrefab;

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
                VuforiaApplication.Instance.OnVuforiaStarted += PlaceZonesByMap;
            }

            _dataProxy.PlaceRandomBeamForSelectedZone.Subscribe(_ =>
            {
                _newBeamTimer?.Dispose();
                _newBeamTimer = CreateNewBeam().ToObservable().Subscribe();
            }).AddTo(this);
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

            BeamData beamData = new()
            {
                Position = Random.insideUnitCircle * selectedZone.MaximumDropDistance +
                           Vector2.one * selectedZone.MinimumDropDistance,
                Name = uncollectedReward.Name,
                Url = uncollectedReward.Url,
                ZoneId = uncollectedReward.ZoneId,
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
                Vector3 objectPosition = playerPosition + new Vector3(data.Position.x, 0f, data.Position.y);

                MannaBoxView follower =
                    Instantiate(beamPrefab, objectPosition, Quaternion.identity, _coordinator.GetContentTransform());

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
    }
}