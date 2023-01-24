using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Screens.Factories;

namespace Screens.RewardsListScreen
{
    public class RewardsListScreenPresenter
    {
        private readonly IRewardsListScreenView _screenView;
        private readonly IDataProxy _dataProxy;
        private readonly IWebImagesLoader _webImagesLoader;
        private readonly RewardCardsFactory _rewardCardsFactory;
        private readonly List<RewardCardView> _rewardsList = new();

        private IDisposable _scanningProgress;

        public RewardsListScreenPresenter(IRewardsListScreenView screenView, IDataProxy dataProxy,
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
        }

        private void OnShowScreen(object data)
        {
            foreach (RewardCardView rewardView in _rewardsList)
            {
                rewardView.Dispose();
            }

            _rewardsList.Clear();

            foreach (RewardViewInfo rewardViewInfo in _dataProxy.GetRewardsForActiveZone())
            {
                rewardViewInfo.Parent = _screenView.GetListContainer();
                RewardCardView cardView = _rewardCardsFactory.Create(rewardViewInfo);

                _webImagesLoader.TryToLoadSprite(rewardViewInfo.Url,
                    sprite => { cardView.SetRewardIcon(sprite); });

                _rewardsList.Add(cardView);
            }
        }
    }
}