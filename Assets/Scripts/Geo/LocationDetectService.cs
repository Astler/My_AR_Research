using System;
using System.Collections;
using Data;
using UnityEngine;
using Utils;
using Zenject;

namespace Geo
{
    public class LocationDetectService : MonoBehaviour
    {
        private Coroutine _trackCoroutine;
        
        private IDataProxy _dataProxy;

        [Inject]
        public void Construct(IDataProxy dataProxy)
        {
            _dataProxy = dataProxy;
        }

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
            if (Application.isEditor)
            {
                callback?.Invoke(LocationDetectResult.Success);
                _dataProxy.SetPlayerPosition(GlobalConstants.MockPosition);
                yield break;
            }
            
            if (!Input.location.isEnabledByUser)
            {
                callback?.Invoke(LocationDetectResult.NoPermission);
                _trackCoroutine = null;
                yield break;
            }

            Input.compass.enabled = true;
            Input.location.Start(GlobalConstants.GeoAccuracy, GlobalConstants.GeoAccuracy);

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

            _dataProxy.SetPlayerPosition(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude));
            
            callback?.Invoke(LocationDetectResult.Success);

            while (true)
            {
                _dataProxy.SetPlayerPosition(new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude));
                yield return new WaitForSeconds(1);
            }
        }
    }
}