using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using Screens.Factories;
using Screens.MainScreen;
using UniRx;
using UnityEngine;
using CameraType = GameCamera.CameraType;

namespace Screens.FindDropZonesScreen
{
    public class ZonesListScreenPresenter
    {
        private readonly IZonesListScreenView _view;
        private readonly IDataProxy _dataProxy;
        private readonly DropZonesCardsFactory _dropZonesCardsFactory;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly List<DropZoneCardView> _zonesList = new();

        private IDisposable _scanningProgress;

        private FindTabType _selectedTab = FindTabType.Map;

        public ZonesListScreenPresenter(IZonesListScreenView view, IDataProxy dataProxy,
            DropZonesCardsFactory dropZonesCardsFactory,
            IScreenNavigationSystem screenNavigationSystem)
        {
            _view = view;
            _dataProxy = dataProxy;
            _dropZonesCardsFactory = dropZonesCardsFactory;
            _screenNavigationSystem = screenNavigationSystem;

            Initialize();
        }

        private void Initialize()
        {
            _view.TabBar.ClickedOnTab += OnTabClicked;
            _view.OnShowCallback += OnShow;
            _view.OnLostFocusCallback += OnLostFocus;
            _view.OnGotFocusCallback += OnGotFocus;
            _view.MapToPlayerPositionClicked += OnMapToMeClicked;
            _view.LaunchArClicked += OnLaunchArClicked;
            _view.SelectedZoneAboutClicked += OnSelectedZoneAboutClicked;

            _dataProxy.EnteredPortalZone.Subscribe(zone => { _view.SetDropZoneName(zone?.Name); });

            _dataProxy.SelectedOnMapDropZoneId.Subscribe(id =>
            {
                DropZoneViewInfo viewInfo = _dataProxy.GetZoneInfoById(id);
                _view.ShowSelectedZoneInfo(viewInfo);
            });
        }

        private void OnGotFocus()
        {
            _dataProxy.SetIsMapOpened(_selectedTab == FindTabType.Map);
            _dataProxy.SetActiveCamera(CameraType.MapCamera);
        }

        private void OnSelectedZoneAboutClicked()
        {
            int id = _dataProxy.SelectedOnMapDropZoneId.Value;

            if (id == -1) return;

            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.DropZoneDetailsScreen).WithExtraData(id));
        }

        private void OnLaunchArClicked()
        {
            _dataProxy.InvokeBottomBarAction(BottomBarButtonType.ArCamera, null);
        }

        private void OnMapToMeClicked()
        {
            Vector2 target = _dataProxy.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnLostFocus()
        {
            _dataProxy.SetIsMapOpened(false);
            _dataProxy.SetActiveCamera(CameraType.Disabled);
        }

        private void OnTabClicked(FindTabType tab)
        {
            _selectedTab = tab;
            ConfigureBySelectedTab();
        }

        private void OnShow(object data)
        {
            if (data is int zoneId)
            {
                DropZoneViewInfo selectedZoneInfo = _dataProxy.GetZoneInfoById(zoneId);

                if (selectedZoneInfo != null)
                {
                    _selectedTab = FindTabType.Map;
                    OnlineMaps.instance.position =
                        new Vector2(selectedZoneInfo.Coordinates.y, selectedZoneInfo.Coordinates.x);
                }
            }
            
            ConfigureBySelectedTab();

            foreach (DropZoneCardView portalCardView in _zonesList)
            {
                portalCardView.Dispose();
            }

            _zonesList.Clear();

            foreach (DropZoneViewInfo dropZoneViewInfo in _dataProxy.GetAllActiveZones())
            {
                DropZoneCardView card = _dropZonesCardsFactory.Create(dropZoneViewInfo);
                card.transform.SetParent(_view.CardsParent);
                card.transform.SetAsLastSibling();
                card.ViewZoneInfoClicked += OnViewZoneInfoClicked;

                _zonesList.Add(card);
            }
        }

        private void ConfigureBySelectedTab()
        {
            _view.TabBar.SetSelectedTab(_selectedTab);
            _dataProxy.SetIsMapOpened(_selectedTab == FindTabType.Map);
            _view.ShowContentByTab(_selectedTab);
        }

        private void OnViewZoneInfoClicked(int zoneId)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.DropZoneDetailsScreen).WithExtraData(zoneId));
        }
    }
}