using System;
using System.Collections;
using Prototype.Data;
using UnityEngine;

namespace Prototype.Location
{
    public class LocationDetectService : MonoBehaviour
    {
        private LocationInfo _lastLocation;
        private Coroutine _trackCoroutine;

        public LocationInfo GetCurrentLocation() => _lastLocation;

        public void Connect(Action<LocationDetectResult> callback)
        {
            if (_trackCoroutine != null)
            {
                return;
            }

            _trackCoroutine = StartCoroutine(TryToConnect(callback));
        }

        public void Disconnect()
        {
            if (_trackCoroutine != null)
            {
                StopCoroutine(_trackCoroutine);
            }

            Input.location.Stop();
        }

        private IEnumerator TryToConnect(Action<LocationDetectResult> callback)
        {
            if (!Input.location.isEnabledByUser)
            {
                callback?.Invoke(LocationDetectResult.NoPermission);
                _trackCoroutine = null;
                yield break;
            }

            Input.compass.enabled = true;
            Input.location.Start(0.1f, 0.1f);

            int waitTime = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
            {
                yield return new WaitForSeconds(1);
                waitTime--;
            }

            if (waitTime < 1)
            {
                callback?.Invoke(LocationDetectResult.Timeout);
                _trackCoroutine = null;
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                callback?.Invoke(LocationDetectResult.Error);
                _trackCoroutine = null;
                yield break;
            }

            _lastLocation = Input.location.lastData;

            callback?.Invoke(LocationDetectResult.Success);

            while (true)
            {
                _lastLocation = Input.location.lastData;
                yield return new WaitForSeconds(1);
            }
        }
    }
}