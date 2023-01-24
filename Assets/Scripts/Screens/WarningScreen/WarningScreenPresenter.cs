using System;
using AR;
using Data;

namespace Screens.WarningScreen
{
    public class WarningScreenPresenter
    {
        private readonly IWarningScreenView _screenView;
        private IDisposable _rewardsListener;
        private readonly IDataProxy _dataProxy;

        public WarningScreenPresenter(IWarningScreenView screenView, IDataProxy dataProxy)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OkClicked += OnOkClicked;
        }

        private void OnOkClicked() => _dataProxy.CompleteStateStep(GameStates.WarningMessage);
    }
}