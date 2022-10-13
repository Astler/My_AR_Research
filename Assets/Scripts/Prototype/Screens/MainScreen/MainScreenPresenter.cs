using System;
using UniRx;

namespace Prototype.Screens.MainScreen
{
    public class MainScreenPresenter: IDisposable
    {
        private readonly IMainScreenView _view;
        private readonly CompositeDisposable _compositeDisposable = new();

        public MainScreenPresenter(IMainScreenView view, ProjectContext context)
        {
            _view = view;
            context.MapOpened.Subscribe(_view.SetIsMapActive).AddTo(_compositeDisposable);
            context.Coins.Subscribe(_view.SetCoins).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        public void ConfigureAction(MainSceneHUDViewInfo mainSceneHUDViewInfo)
        {
            _view.ConfigureAction(mainSceneHUDViewInfo);
        }
    }
}