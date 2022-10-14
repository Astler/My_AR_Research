using System;
using Prototype.World;
using UniRx;

namespace Prototype.Screens.MainScreen
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly IMainScreenView _view;
        private readonly ProjectContext _context;
        private readonly CompositeDisposable _compositeDisposable = new();

        public MainScreenPresenter(IMainScreenView view, ProjectContext context)
        {
            _view = view;
            _context = context;

            context.MapOpened.Subscribe(_view.SetIsMapActive).AddTo(_compositeDisposable);
            context.Coins.Subscribe(_view.SetCoins).AddTo(_compositeDisposable);

            context.GetLocationController().SelectedPortalZone.Subscribe(zone =>
            {
                _view.SetCanPlacePortal(zone != null);
            }).AddTo(_compositeDisposable);
            
            _view.GetMapUserInterface().PortalsListClicked += OnPortalsListClicked;
            _view.GetMapUserInterface().MyPositionClicked += OnMyPositionClicked;
            _view.GetMapUserInterface().NearestPortalClicked += OnNearestPortalClicked;
        }

        private void OnNearestPortalClicked()
        {
            OnlineMaps.instance.position = _context.GetLocationController().NearestPortalZone.Value.GetPosition();
        }

        private void OnMyPositionClicked()
        {
            OnlineMaps.instance.position = LocationController.GetPlayerPosition();
        }

        private void OnPortalsListClicked()
        {
            _context.GetScreensInstaller().PortalsListPresenter.ShowScreen();
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        public void ConfigureAction(MainSceneHUDViewInfo mainSceneHUDViewInfo)
        {
            _view.ConfigureAction(mainSceneHUDViewInfo);
        }
    }
}