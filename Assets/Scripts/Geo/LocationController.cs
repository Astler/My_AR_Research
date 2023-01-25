using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
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
                _dataProxy.SetActivePortalZone(_dataProxy.GetAllActiveZones()
                    .FirstOrDefault(it => it.Name == "Dev Portal"));
                return;
            }

            Vector2 playerPosition = _dataProxy.GetPlayerPosition();

            Dictionary<DropZoneViewInfo, double> distances = new();

            string zonesInfo = "";

            foreach (DropZoneViewInfo portalZoneModel in _dataProxy.GetAllActiveZones().ToList())
            {
                double distance = CoordinatesUtils.Distance(playerPosition.x,
                    playerPosition.y,
                    portalZoneModel.Coordinates.x,
                    portalZoneModel.Coordinates.y);
                distances.Add(portalZoneModel, distance);
            }

            List<KeyValuePair<DropZoneViewInfo, double>> detectAvailableZones =
                distances.Where(it => it.Value < it.Key.Radius / 1000).ToList();

            if (detectAvailableZones.Any())
            {
                KeyValuePair<DropZoneViewInfo, double> closestPoint =
                    detectAvailableZones.OrderBy(it => it.Value).FirstOrDefault();
                _dataProxy.SetActivePortalZone(closestPoint.Key);
                return;
            }

            _dataProxy.SetActivePortalZone(null);

            List<KeyValuePair<DropZoneViewInfo, double>> nearZones =
                distances.OrderBy(it => it.Value).ToList();

            _dataProxy.SetNearestPortalZone(nearZones.FirstOrDefault().Key);

            for (int i = 0; i < (nearZones.Count > 5 ? 5 : nearZones.Count); i++)
            {
                KeyValuePair<DropZoneViewInfo, double> zoneData = nearZones[i];

                zonesInfo += (zonesInfo.Length == 0 ? "" : "\n") + zoneData.Key.Name + " " +
                             zoneData.Key.Coordinates.ToHumanReadableDistanceFromPlayer(_dataProxy.GetPlayerPosition());
            }
            
            //TODO Restore
            // locationInfoView.ShowAllZones($"Nearest zones:\n{zonesInfo}");
        }
    }
}