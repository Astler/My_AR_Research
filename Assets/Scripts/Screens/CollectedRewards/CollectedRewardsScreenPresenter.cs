using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Screens.Factories;
using Screens.RewardsListScreen;
using UniRx;

namespace Screens.CollectedRewards
{
    public class CollectedRewardsScreenPresenter
    {
        private readonly ICollectedRewardsScreenView _collectedRewardsScreenView;
        private readonly IDataProxy _dataProxy;
        private readonly IWebImagesLoader _webImagesLoader;
        private readonly RewardCardsFactory _rewardCardsFactory;
        private readonly List<RewardCardView> _rewardsList = new();
        private IDisposable _rewardsListener;

        public CollectedRewardsScreenPresenter(ICollectedRewardsScreenView collectedRewardsScreenView,
            IDataProxy dataProxy, IWebImagesLoader webImagesLoader, RewardCardsFactory rewardCardsFactory)
        {
            _collectedRewardsScreenView = collectedRewardsScreenView;
            _dataProxy = dataProxy;
            _webImagesLoader = webImagesLoader;
            _rewardCardsFactory = rewardCardsFactory;

            Initialize();
        }

        private void Initialize()
        {
            _collectedRewardsScreenView.OnShowCallback += OnShow;
            _collectedRewardsScreenView.OnLostFocusCallback += OnLostFocus;
            _collectedRewardsScreenView.RefreshClicked += OnRefreshClicked;
        }

        private void OnRefreshClicked() => _dataProxy.RefreshCollectedRewards();

        private void OnLostFocus() => _rewardsListener?.Dispose();

        private void OnShow(object _)
        {
            _rewardsListener = _dataProxy.CollectedPrizesInfos.ObserveCountChanged().Subscribe(_ => LoadRewards());
            LoadRewards();
        }

        private void LoadRewards()
        {
            foreach (RewardCardView rewardView in _rewardsList)
            {
                rewardView.Dispose();
            }

            foreach (RewardViewInfo rewardViewInfo in _dataProxy.CollectedPrizesInfos)
            {
                rewardViewInfo.Parent = _collectedRewardsScreenView.CardsParent;
                RewardCardView cardView = _rewardCardsFactory.Create(rewardViewInfo);

                _webImagesLoader.TryToLoadSprite(rewardViewInfo.Url,
                    sprite => { cardView.SetRewardIcon(sprite); });

                _rewardsList.Add(cardView);
            }
        }
    }
}