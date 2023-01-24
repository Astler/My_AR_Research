using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Screens.PortalsListScreen
{
    public class ZonesListScreenPresenter
    {
        private readonly IZonesListScreenView _screenView;
        private readonly IDataProxy _dataProxy;
        private readonly List<PortalCardView> _zonesList = new();

        private IDisposable _scanningProgress;

        public ZonesListScreenPresenter(IZonesListScreenView screenView, IDataProxy dataProxy)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OnShowCallback += OnShowScreen;
        }

        private void OnShowScreen(object data)
        {
            foreach (PortalCardView portalCardView in _zonesList)
            {
                portalCardView.DestroyCard();
            }

            _zonesList.Clear();

            foreach (ZoneViewInfo portalViewInfo in _dataProxy.GetAllActiveZones())
            {
                PortalCardView portalCardView = Object.Instantiate(_screenView.GetCardPrefab(),
                    _screenView.GetListContainer());
                portalCardView.transform.SetAsLastSibling();
                portalCardView.ConfigureView(portalViewInfo);

                portalCardView.MoveToClicked += OnMoveToClicked;

                _zonesList.Add(portalCardView);
            }
        }

        private void OnMoveToClicked(Vector2 coordinates)
        {
            OnlineMaps.instance.position = new Vector2(coordinates.y, coordinates.x);
            _screenView.CloseScreen();
        }
    }
}