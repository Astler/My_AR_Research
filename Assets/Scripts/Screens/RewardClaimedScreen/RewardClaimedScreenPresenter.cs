using System;

namespace Screens.RewardClaimedScreen
{
    public class RewardClaimedScreenPresenter
    {
        private readonly IRewardClaimedScreenView _screenView;
        private IDisposable _rewardsListener;

        public RewardClaimedScreenPresenter(IRewardClaimedScreenView screenView)
        {
            _screenView = screenView;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OnShowCallback += OnShowScreen;
        }

        private void OnShowScreen(object data)
        {
            if (data is not RewardScreenViewInfo rewardScreenViewInfo) return;
            
            _screenView.ShowReward(rewardScreenViewInfo);
        }
    }
}