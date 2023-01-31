using System;
using System.Collections.Generic;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Screens.DropZoneDetailsScreen;
using Screens.Factories;
using UniRx;

namespace Screens.CollectedRewards
{
    public class CollectedRewardsScreenPresenter
    {
        private readonly ICollectedRewardsScreenView _view;
        private readonly IDataProxy _dataProxy;
        private readonly IWebImagesLoader _webImagesLoader;
        private readonly RewardCardsFactory _rewardCardsFactory;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly List<RewardCardView> _rewardsList = new();
        private IDisposable _rewardsListener;

        public CollectedRewardsScreenPresenter(ICollectedRewardsScreenView view,
            IDataProxy dataProxy, IWebImagesLoader webImagesLoader, RewardCardsFactory rewardCardsFactory,
            IScreenNavigationSystem screenNavigationSystem)
        {
            _view = view;
            _dataProxy = dataProxy;
            _webImagesLoader = webImagesLoader;
            _rewardCardsFactory = rewardCardsFactory;
            _screenNavigationSystem = screenNavigationSystem;

            Initialize();
        }

        private void Initialize()
        {
            _view.OnShowCallback += OnShow;
            _view.OnLostFocusCallback += OnLostFocus;
            _view.RefreshClicked += OnRefreshClicked;
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

            IReadOnlyReactiveCollection<RewardViewInfo> collectedInfos = _dataProxy.CollectedPrizesInfos;

            _view.SetIsAnyCollectedDrops(collectedInfos.Count > 0);
            
            foreach (RewardViewInfo rewardViewInfo in collectedInfos)
            {
                rewardViewInfo.Parent = _view.CardsParent;
                
                rewardViewInfo.ViewAction += () =>
                {
                    _screenNavigationSystem.ExecuteNavigationCommand(
                        new NavigationCommand()
                            .ShowNextScreen(ScreenName.RewardClaimedScreen)
                            .WithExtraData(rewardViewInfo));
                };
                
                RewardCardView cardView = _rewardCardsFactory.Create(rewardViewInfo);

                _webImagesLoader.TryToLoadSprite(rewardViewInfo.Url,
                    sprite => { cardView.SetRewardIcon(sprite); });

                _rewardsList.Add(cardView);
            }
        }
    }
}