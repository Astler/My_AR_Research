using System;
using Prototype.World;
using UniRx;
using UnityEngine;

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
            Vector2 target = _context.GetLocationController().NearestPortalZone.Value.GetPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnMyPositionClicked()
        {
            Vector2 target = LocationController.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
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