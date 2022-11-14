using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Data;
using Data.Objects;
using UnityEngine;
using Utils;
using Zenject;

namespace Geo
{
    [RequireComponent(typeof(LocationDetectService))]
    public class LocationController : MonoBehaviour
    {
        private LocationDetectService _locationService;
        private Coroutine _zonesLocator;
        private AssetsScriptableObject _assetsScriptableObject;
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(AssetsScriptableObject assetsScriptableObject, IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
            _assetsScriptableObject = assetsScriptableObject;
        }

        private void Awake()
        {
            _locationService = GetComponent<LocationDetectService>();
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

            while (_dataProxy.GetLocationDetectResult() != LocationDetectResult.Success)
            {
                Debug.Log("connect try");

                _locationService.Connect(result =>
                {
                    _dataProxy.SetLocationDetectStatus(result);

                    if (result == LocationDetectResult.Success)
                    {
                        StartZonesLocator();
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
            if (Application.isEditor)
            {
                _dataProxy.SetActivePortalZone(_assetsScriptableObject.GetZonesList()
                    .FirstOrDefault(it => it.name == "Dev Portal"));
                return;
            }

            List<PortalZoneModel> points = _assetsScriptableObject.GetZonesList().ToList();

            Dictionary<PortalZoneModel, double> distances = new();

            string zonesInfo = "";

            foreach (PortalZoneModel portalZoneModel in points)
            {
                if (!portalZoneModel.isActive) continue;

                double distance = CoordinatesUtils.Distance(Input.location.lastData.latitude,
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

                _dataProxy.SetActivePortalZone(closestPoint.Key);
                _dataProxy.SetNearestPortalZone(closestPoint.Key);

                return;
            }

            _dataProxy.SetActivePortalZone(null);

            List<KeyValuePair<PortalZoneModel, double>> nearZones =
                distances.OrderBy(it => it.Value).ToList();

            _dataProxy.SetNearestPortalZone(nearZones.FirstOrDefault().Key);

            for (int i = 0; i < (nearZones.Count > 5 ? 5 : nearZones.Count); i++)
            {
                KeyValuePair<PortalZoneModel, double> zoneData = nearZones[i];

                zonesInfo += (zonesInfo.Length == 0 ? "" : "\n") + zoneData.Key.name + " " +
                             zoneData.Key.GetPosition().ToHumanReadableDistanceFromPlayer();
            }

            //TODO Restore
            // locationInfoView.ShowAllZones($"Nearest zones:\n{zonesInfo}");
        }

        public static Vector2 GetPlayerPosition() =>
            new(Input.location.lastData.latitude, Input.location.lastData.longitude);

        private void FixedUpdate()
        {
            if (Application.isEditor)
            {
                _dataProxy.SetPlayerPosition(new Vector2(48.5089416503906f, 35.077808380127f));
                return;
            }

            if (_dataProxy.GetLocationDetectResult() != LocationDetectResult.Success) return;

            _dataProxy.SetPlayerPosition(GetPlayerPosition());
        }
    }
}