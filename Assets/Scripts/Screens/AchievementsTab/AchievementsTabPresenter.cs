using System;

namespace Screens.AchievementsTab
{
    public class AchievementsTabPresenter
    {
        private readonly IAchievementsTabView _view;
        private IDisposable _rewardsListener;

        public AchievementsTabPresenter(IAchievementsTabView view)
        {
            _view = view;

            Initialize();
        }

        private void Initialize()
        {
            _view.OnShowCallback += Show;
        }

        private void Show(object data) { }
    }
}