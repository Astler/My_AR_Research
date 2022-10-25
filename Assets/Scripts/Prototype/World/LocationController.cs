using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.Assets;
using Prototype.Data;
using Prototype.Location;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.World
{
    using UniRx;

    public class LocationController : MonoBehaviour
    {
        [SerializeField] private LocationDetectService locationService;
        [SerializeField] private LocationInfoView locationInfoView;

        private readonly ReactiveProperty<PortalZoneModel> _selectedPortalZone = new();
        private readonly ReactiveProperty<PortalZoneModel> _nearestPortalZone = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();

        private ProjectContext _context;
        private Coroutine _zonesLocator;
        private LocationDetectResult _locationDetectResult;

        public IReadOnlyReactiveProperty<PortalZoneModel> SelectedPortalZone => _selectedPortalZone;
        public IReadOnlyReactiveProperty<PortalZoneModel> NearestPortalZone => _nearestPortalZone;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;


        private void Awake()
        {
            _context = FindObjectOfType<ProjectContext>();

            SelectedPortalZone.Subscribe(delegate(PortalZoneModel model)
            {
                if (model == null)
                {
                    locationInfoView.SetActiveZoneName("<color=red>You are not in the portal area!</color>");
                    return;
                }

                locationInfoView.SetActiveZoneName($"<color=green>{model.name}</color>");
            }).AddTo(this);
        }

        private void Start()
        {
            StartCoroutine(Connector());
        }

        private IEnumerator Connector()
        {
            if (Application.isEditor)
            {
                StartZonesLocator();
                yield break;
            }

            while (_locationDetectResult != LocationDetectResult.Success)
            {
                Debug.Log("connect try");

                locationService.Connect(result =>
                {
                    switch (result)
                    {
                        case LocationDetectResult.NoPermission:
                            locationInfoView.ShowResponse("Location permission denied.");
                            break;
                        case LocationDetectResult.Timeout:
                            locationInfoView.ShowResponse("Location request timed out.");
                            break;
                        case LocationDetectResult.Error:
                            locationInfoView.ShowResponse("Location request undefined error.");
                            break;
                        case LocationDetectResult.Success:
                            locationInfoView.ShowResponse("Location detected!");
                            StartZonesLocator();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(result), result, null);
                    }
                });

                yield return new WaitForSeconds(1f);
            }

            Debug.Log("connector finished");
        }

        private void StartZonesLocator()
        {
            if (_zonesLocator != null)
            {
                StopCoroutine(_zonesLocator);
            }

            _zonesLocator = StartCoroutine(ZonesLocator());
        }

        private IEnumerator ZonesLocator()
        {
            while (true)
            {
                CheckPortalZones();
                yield return new WaitForSeconds(1f);
            }
        }

        private void CheckPortalZones()
        {
            // if (Application.isEditor)
            // {
            //     _selectedPortalZone.Value = _context.GetAssets().portalZones.GetRandomElement();
            //     return;
            // }

            List<PortalZoneModel> points = _context.GetPortalPoints();

            Dictionary<PortalZoneModel, double> distances = new();

            string zonesInfo = "";

            foreach (PortalZoneModel portalZoneModel in points)
            {
                if (!portalZoneModel.isActive) continue;

                double distance = LocationUtils.Distance(Input.location.lastData.latitude,
                    Input.location.lastData.longitude,
                    portalZoneModel.latitude,
                    portalZoneModel.longitude);
                distances.Add(portalZoneModel, distance);
            }

            List<KeyValuePair<PortalZoneModel, double>> detectAvailableZones =
                distances.Where(it => it.Value < it.Key.radius / 1000).ToList();

            if (detectAvailableZones.Any())
            {
                KeyValuePair<PortalZoneModel, double> closestPoint =
                    detectAvailableZones.OrderBy(it => it.Value).FirstOrDefault();

                _selectedPortalZone.Value = closestPoint.Key;
                _nearestPortalZone.Value = closestPoint.Key;

                locationInfoView.HideAllZonesList();

                return;
            }

            _selectedPortalZone.Value = null;

            List<KeyValuePair<PortalZoneModel, double>> nearZones =
                distances.OrderBy(it => it.Value).ToList();

            _nearestPortalZone.Value = nearZones.FirstOrDefault().Key;

            for (int i = 0; i < (nearZones.Count > 5 ? 5 : nearZones.Count); i++)
            {
                KeyValuePair<PortalZoneModel, double> zoneData = nearZones[i];

                zonesInfo += (zonesInfo.Length == 0 ? "" : "\n") + zoneData.Key.name + " " +
                             zoneData.Key.GetPosition().ToHumanReadableDistanceFromPlayer();
            }

            locationInfoView.ShowAllZones($"Nearest zones:\n{zonesInfo}");
        }

        public static Vector2 GetPlayerPosition() =>
            new(Input.location.lastData.latitude, Input.location.lastData.longitude);

        private void FixedUpdate()
        {
            _playerLocationChanged.Value = new Vector2(locationService.GetCurrentLocation().latitude,
                locationService.GetCurrentLocation().longitude);
        }
    }
}