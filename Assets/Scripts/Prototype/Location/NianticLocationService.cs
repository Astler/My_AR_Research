using System.Collections.Generic;
using Niantic.ARDK;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.VirtualStudio.VpsCoverage;
using Niantic.ARDK.VPSCoverage;
using UnityEngine;

namespace Prototype.Location
{
    public class NianticLocationService: MonoBehaviour
    {
        private ILocationService _locationService;
        
        private RuntimeEnvironment _coverageClientRuntime = RuntimeEnvironment.Default;
        private ICoverageClient _coverageClient;
        private VpsCoverageResponses _mockResponses;

// Default is the Ferry Building in San Francisco
        private LatLng _spoofLocation = new LatLng(37.79531921750984, -122.39360429639748);
        private int _queryRadius = 100;

        // Start is called before the first frame update
        void Awake()
        {
            _locationService = LocationServiceFactory.Create();

#if UNITY_EDITOR
            var spoofService = (SpoofLocationService)_locationService;

            // In editor, the specified spoof location will be used.
            spoofService.SetLocation(_spoofLocation);
#endif

            _locationService.LocationUpdated += OnLocationUpdated;
            _locationService.Start();
            
            _coverageClient = CoverageClientFactory.Create(_coverageClientRuntime, _mockResponses);
        }

        private void OnLocationUpdated(LocationUpdatedArgs args)
        {
            _locationService.LocationUpdated -= OnLocationUpdated;
            _coverageClient.RequestCoverageAreas(args.LocationInfo, _queryRadius, ProcessAreasResult);
        }

        private void ProcessAreasResult(CoverageAreasResult result)
        {
            if (result.Status == ResponseStatus.Success)
            {
                Debug.Log("Coverage areas: " + result.Areas);
            }
            else
            {
                Debug.LogError("Coverage areas request failed: " + result.Status);
            }
        }
        
        // public void LoadLocalReference()
        // {
        //     // if (PlayerPrefs.HasKey(LocalSaveKey))
        //     // {
        //
        //         List<WayspotAnchorPayload> payloads = new List<WayspotAnchorPayload>();
        //
        //         string json = PlayerPrefs.GetString(LocalSaveKey);
        //         MyStoredAnchorsData storedData = JsonUtility.FromJson<MyStoredAnchorsData>(json);
        //
        //         foreach (var wayspotAnchorPayload in storedData.Payloads)
        //         {
        //             var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
        //             payloads.Add(payload);
        //         }
        //
        //         if (payloads.Count > 0)
        //         {
        //             var wayspotAnchors = wayspotAnchorService.RestoreWayspotAnchors(payloads.ToArray());
        //             OnWayspotAnchorsAdded(wayspotAnchors);
        //         }
        //
        //     // }
        // }
    }
}