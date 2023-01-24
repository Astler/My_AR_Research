using System;
using AR;
using Data;
using UniRx;
using UnityEngine;

namespace Screens.ArScanningPopup
{
    public class ArScanningPopupPresenter
    {
        private readonly IArScanningPopupView _screenView;
        private readonly IDataProxy _dataProxy;
        private IDisposable _scanningProgress;

        public ArScanningPopupPresenter(IArScanningPopupView screenView, IDataProxy dataProxy)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;

            Initialize();
        }

        private void Initialize()
        {
            if (Application.isEditor)
            {
                Observable.Timer(TimeSpan.FromSeconds(5f)).Subscribe(_ =>
                {
                    _screenView.CloseScreen();
                    _dataProxy.CompleteStateStep(GameStates.Scanning);
                });
                return;
            }

            _scanningProgress = _dataProxy.ScannedArea.Subscribe(areaCoefficient =>
            {
                _screenView.SetScannedProgressValue(areaCoefficient);

                if (areaCoefficient < 1) return;

                _screenView.CloseScreen();
                _dataProxy.CompleteStateStep(GameStates.Scanning);
                _scanningProgress?.Dispose();
            });
        }
    }
}