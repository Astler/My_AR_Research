using System;
using System.Collections.Generic;
using System.Globalization;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Screens.Factories;
using Screens.MainScreen;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Screens.DropZoneDetailsScreen
{
    public class DropZoneDetailsScreenPresenter
    {
        private readonly IDropZoneDetailsScreenView _screenView;
        private readonly IDataProxy _dataProxy;
        private readonly IWebImagesLoader _webImagesLoader;
        private readonly RewardCardsFactory _rewardCardsFactory;
        private readonly List<RewardCardView> _rewardsList = new();

        private IDisposable _scanningProgress;
        private DropZoneViewInfo _selectedZoneInfo;

        public DropZoneDetailsScreenPresenter(IDropZoneDetailsScreenView screenView, IDataProxy dataProxy,
            IWebImagesLoader webImagesLoader, RewardCardsFactory rewardCardsFactory)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;
            _webImagesLoader = webImagesLoader;
            _rewardCardsFactory = rewardCardsFactory;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OnShowCallback += OnShowScreen;
            _screenView.OpenNativeMapsClicked += OpenPointInNativeMap;
            _screenView.OpenArMapsClicked += OpenArMap;
        }

        private void OpenArMap()
        {
            _dataProxy.InvokeBottomBarAction(BottomBarButtonType.Find, _selectedZoneInfo.Id);
            _screenView.CloseScreen();
        }

        private void OnShowScreen(object data)
        {
            if (data is not int id)
            {
                _screenView.CloseScreen();
                return;
            }

            _selectedZoneInfo = _dataProxy.GetZoneInfoById(id);

            _screenView.ConfigureView(_selectedZoneInfo);

            foreach (RewardCardView rewardView in _rewardsList)
            {
                rewardView.Dispose();
            }

            _rewardsList.Clear();

            foreach (RewardViewInfo rewardViewInfo in _selectedZoneInfo.GetGroupedRewards())
            {
                rewardViewInfo.Parent = _screenView.RewardsListParent;
                RewardCardView cardView = _rewardCardsFactory.Create(rewardViewInfo);

                _webImagesLoader.TryToLoadSprite(rewardViewInfo.Url,
                    sprite => { cardView.SetRewardIcon(sprite); });

                _rewardsList.Add(cardView);
            }
        }
        
        private void OpenPointInNativeMap()
        {
            string latitude = _selectedZoneInfo.Coordinates.x.ToString(CultureInfo.InvariantCulture);
            string longitude = _selectedZoneInfo.Coordinates.y.ToString(CultureInfo.InvariantCulture);
            
            string ulrStart = Application.platform == RuntimePlatform.Android ? "https" : "maps";
            Application.OpenURL(ulrStart + $"://maps.google.com/maps?daddr={latitude},{longitude}&amp;ll=");
        }
    }
}