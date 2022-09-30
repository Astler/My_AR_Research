using System;
using System.Collections;
using UnityEngine;

namespace Prototype.Services
{
    public class GpsService : MonoBehaviour
    {
        public LocationInfo LastLocationInfo { private set; get; }
        public bool IsActive { private set; get; }
        public event Action<LocationInfo> OnLocationInfoChanged;
        public event Action<bool> OnLocationTrackingStateChanged;
        public event Action<string> OnError;

        private const float TargetLongitude = 35.03178f;
        private const float TargetLatitude = 48.43561f;
        
        public void TryToStartGpsService()
        {
            StartCoroutine(StartGpsService());
        }

        private IEnumerator StartGpsService()
        {
#if UNITY_ANDROID
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogFormat("Android and Location not enabled");
                OnError?.Invoke("Android and Location not enabled");
                yield return null;
                TryToStartGpsService();
                yield break;
            }

#elif UNITY_IOS
            if (!Input.location.isEnabledByUser) {
                Debug.LogFormat("IOS and Location not enabled");
                yield break;
            }
#endif
            Input.location.Start(0.1f, 0.1f);

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                Debug.Log("Timed out");
                OnError?.Invoke("Timed out");
                yield return null;
                TryToStartGpsService();
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
                OnError?.Invoke("Unable to determine device location");
                yield return null;
                TryToStartGpsService();
                yield break;
            }

            IsActive = true;
            OnLocationTrackingStateChanged?.Invoke(IsActive);
            while (IsActive)
            {
                LastLocationInfo = Input.location.lastData;
                OnLocationInfoChanged?.Invoke(LastLocationInfo);
                yield return new WaitForSeconds(1);
            }
        }

        public void StopService()
        {
            IsActive = false;
            OnLocationTrackingStateChanged?.Invoke(IsActive);
            Input.location.Stop();
        }

        public float GetDistanceToTarget()
        {
            return CalculateDistance(LastLocationInfo.latitude, LastLocationInfo.longitude, TargetLatitude,
                TargetLongitude);
        }

        private float CalculateDistance(float lat1, float long1, float lat2, float long2)
        {
            int R = 6371;
            float latRad1 = Mathf.Deg2Rad * lat1;
            float latRad2 = Mathf.Deg2Rad * lat2;
            float deltaLatRad = Mathf.Deg2Rad * (lat2 - lat1);
            float deltaLongRad = Mathf.Deg2Rad * (long2 - long1);
            float a = Mathf.Pow(Mathf.Sin(deltaLatRad / 2), 2) + (Mathf.Pow(Mathf.Sin(deltaLongRad / 2), 2)
                                                                  * Mathf.Cos(latRad1) * Mathf.Cos(latRad2));
            float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            float totalDist = R * c * 1000;
            return totalDist;
        }
    }
}