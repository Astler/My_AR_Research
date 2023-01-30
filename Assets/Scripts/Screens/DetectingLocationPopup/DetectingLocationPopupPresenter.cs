using System;
using AR;
using Data;
using Geo;
using UniRx;
using UnityEngine;

namespace Screens.DetectingLocationPopup
{
    public class DetectingLocationPopupPresenter
    {
        private readonly IDetectingLocationPopupPresenter _screenView;
        private readonly IDataProxy _dataProxy;
        private IDisposable _locationDetectListener;

        public DetectingLocationPopupPresenter(IDetectingLocationPopupPresenter screenView, IDataProxy dataProxy)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OnShowCallback += OnShowScreen;
        }

        private void OnShowScreen(object obj)
        {
            if (Application.isEditor)
            {
                Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
                {
                    _screenView.CloseScreen();
                    _dataProxy.CompleteStateStep(GameStates.LocationDetection);
                });
            }

            _locationDetectListener = _dataProxy.LocationDetectResult.Subscribe(result =>
            {
                Debug.Log($"LOCATION RESULT = {result}");
                if (result != LocationDetectResult.Success) return;

                Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                {
                    Debug.Log($"close location popup");
                    _screenView.CloseScreen();
                    _dataProxy.CompleteStateStep(GameStates.LocationDetection);
                    _locationDetectListener?.Dispose();
                });
            });
        }
    }
}