using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using Screens.Factories;
using UnityEngine;
using CameraType = GameCamera.CameraType;

namespace Screens.FindDropZonesScreen
{
    public class ZonesListScreenPresenter
    {
        private readonly IZonesListScreenView _screenView;
        private readonly IDataProxy _dataProxy;
        private readonly DropZonesCardsFactory _dropZonesCardsFactory;
        private readonly List<DropZoneCardView> _zonesList = new();

        private IDisposable _scanningProgress;

        private FindTabType _selectedTab = FindTabType.Map;

        public ZonesListScreenPresenter(IZonesListScreenView screenView, IDataProxy dataProxy, DropZonesCardsFactory dropZonesCardsFactory)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;
            _dropZonesCardsFactory = dropZonesCardsFactory;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.TabBar.ClickedOnTab += OnTabClicked;
            _screenView.OnShowCallback += OnShowScreen;
            _screenView.OnLostFocusCallback += OnLostFocus;
            _screenView.MapToPlayerPositionClicked += OnMapToMeClicked;
            _screenView.LaunchArClicked += OnLaunchArClicked;
        }

        private void OnLaunchArClicked()
        {
            
        }

        private void OnMapToMeClicked()
        {
            Vector2 target = _dataProxy.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnLostFocus()
        {
            _dataProxy.SetActiveCamera(CameraType.Disabled);
        }

        private void OnTabClicked(FindTabType tab)
        {
            _selectedTab = tab;
            ConfigureBySelectedTab();
        }

        private void OnShowScreen(object data)
        {
            ConfigureBySelectedTab();

            foreach (DropZoneCardView portalCardView in _zonesList)
            {
                portalCardView.Dispose();
            }

            _zonesList.Clear();

            foreach (DropZoneViewInfo dropZoneViewInfo in _dataProxy.GetAllActiveZones())
            {
                DropZoneCardView card = _dropZonesCardsFactory.Create(dropZoneViewInfo);
                card.transform.SetParent(_screenView.CardsParent);
                card.transform.SetAsLastSibling();
                card.MoveToClicked += OnMoveToClicked;

                _zonesList.Add(card);
            }
        }

        private void ConfigureBySelectedTab()
        {
            _screenView.TabBar.SetSelectedTab(_selectedTab);
            _dataProxy.SetIsMapOpened(_selectedTab == FindTabType.Map);
            _screenView.ShowContentByTab(_selectedTab);
        }

        private void OnMoveToClicked(Vector2 coordinates)
        {
            OnlineMaps.instance.position = new Vector2(coordinates.y, coordinates.x);
            _screenView.CloseScreen();
        }
    }
}