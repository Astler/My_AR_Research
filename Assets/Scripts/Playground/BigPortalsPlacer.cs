using System;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Prototype;
using Prototype.Assets;
using Prototype.Location;
using UniRx;
using UnityEngine;

namespace Playground
{
    public class BigPortalsPlacer : MonoBehaviour
    {
        [SerializeField] private ARAnchorFollower bigPortal;
        [SerializeField] private ARAnchorFollower controlPoint;
        [SerializeField] private ARAnchorFollower movePoint;
        [SerializeField] private Transform map;
        [SerializeField] private Transform camera;
        private ProjectContext _context;
        private ARAnchorFollower _pointMe;
        private WayspotAnchorService _wayspotAnchorService;
        private readonly Dictionary<Guid, ARAnchorFollower> _wayspotAnchorGameObjects = new();
        private readonly List<ARAnchorFollower> _anchors = new();
        private ARAnchorFollower _zeroAnchor;
        private float _startDeviceRotation;

        private void Start()
        {
            _context = FindObjectOfType<ProjectContext>();

            _context.GetARController().Initialized
                .Where(x => x)
                .Subscribe(_ => PlaceObjects());
        }

        private void PlaceObjects()
        {
            // _wayspotAnchorService = CreateWayspotAnchorService();

            _zeroAnchor = Instantiate(controlPoint, Vector3.zero, Quaternion.identity);
            _zeroAnchor.Anchor = _context.AddAnchor(Vector2.zero);
            _zeroAnchor.name = "Me";

            _context.GetLocationController().PlayerLocationChanged.Subscribe(delegate(Vector2 position)
            {
                _startDeviceRotation = camera.transform.rotation.eulerAngles.y - Input.compass.trueHeading;
                map.rotation = Quaternion.Euler(new Vector3(0f, _startDeviceRotation, 0f));
                _context.ClearAnchors();

                foreach (ARAnchorFollower arAnchorFollower in _anchors)
                {
                    arAnchorFollower.gameObject.Destroy();
                }

                _anchors.Clear();

                Vector2 playerPosition = Conversions.GeoToWorldPosition(position.ToVector2d(),
                    Vector2d.zero).ToUnityVector();

                foreach (PortalZoneModel portalZoneModel in _context.GetAssets().portalZones.Where(it => it.isActive))
                {
                    Vector2 objectPosition = Conversions.GeoToWorldPosition(portalZoneModel.GetPosition2d(),
                        playerPosition.ToVector2d()).ToUnityVector();

                    ARAnchorFollower follower = Instantiate(bigPortal, Vector3.zero, Quaternion.identity);
                    follower.Anchor = _context.AddAnchor(objectPosition);
                    follower.name = portalZoneModel.name;

                    _anchors.Add(follower);

                    Debug.Log("Placed " + portalZoneModel.name + " at " + objectPosition);
                    Debug.Log("position.ToVector2d() " + position.ToVector2d());
                }
            }).AddTo(this);
        }

        private void Update()
        {
            // map.rotation = Quaternion.Euler(new Vector3(0f, camera.transform.rotation.eulerAngles.y - Input.compass.trueHeading, 0f));
            Debug.Log($"rota = {-Input.compass.trueHeading}");
        }

        #region altAnchors

        private WayspotAnchorService CreateWayspotAnchorService()
        {
            IWayspotAnchorsConfiguration wayspotAnchorsConfiguration = WayspotAnchorsConfigurationFactory.Create();

            ILocationService locationService =
                LocationServiceFactory.Create(_context.GetARController().GetSession().RuntimeEnvironment);
            locationService.Start();

            WayspotAnchorService wayspotAnchorService = new(_context.GetARController().GetSession(), locationService,
                wayspotAnchorsConfiguration);
            return wayspotAnchorService;
        }


        private void PlaceAnchor(Matrix4x4 localPose)
        {
            IWayspotAnchor[] anchors = _wayspotAnchorService.CreateWayspotAnchors(localPose);
            CreateAnchorGameObjects(anchors);
        }

        private void CreateAnchorGameObjects(IWayspotAnchor[] wayspotAnchors)
        {
            foreach (var wayspotAnchor in wayspotAnchors)
            {
                if (_wayspotAnchorGameObjects.ContainsKey(wayspotAnchor.ID))
                {
                    continue;
                }

                wayspotAnchor.TransformUpdated += HandleWayspotAnchorTrackingUpdated;
                var id = wayspotAnchor.ID;
                ARAnchorFollower anchor = Instantiate(bigPortal);
                anchor.SetActive(false);
                anchor.name = $"Anchor {id}";
                _wayspotAnchorGameObjects.Add(id, anchor);
            }
        }

        private void HandleWayspotAnchorTrackingUpdated(WayspotAnchorResolvedArgs wayspotAnchorResolvedArgs)
        {
            var anchor = _wayspotAnchorGameObjects[wayspotAnchorResolvedArgs.ID].transform;
            anchor.position = wayspotAnchorResolvedArgs.Position;
            anchor.rotation = wayspotAnchorResolvedArgs.Rotation;
            anchor.gameObject.SetActive(true);
        }

        #endregion
    }
}