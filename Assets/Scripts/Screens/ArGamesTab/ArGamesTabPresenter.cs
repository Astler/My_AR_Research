using System;

namespace Screens.ArGamesTab
{
    public class ArGamesTabPresenter
    {
        private readonly ArGamesTabView _view;
        private IDisposable _rewardsListener;

        public ArGamesTabPresenter(ArGamesTabView view)
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