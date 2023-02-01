using System;
using Data;
using SceneManagement;
using UniRx;
using UnityEngine;

namespace Screens.LoadingScreen
{
    public class LoadingScreenPresenter
    {
        private readonly ILoadingScreenView _view;
        private readonly SceneLoader _sceneLoader;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IDataProxy _dataProxy;
        private readonly CompositeDisposable _compositeDisposable = new();

        public LoadingScreenPresenter(ILoadingScreenView view, SceneLoader sceneLoader,
            IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy)
        {
            _view = view;
            _sceneLoader = sceneLoader;
            _screenNavigationSystem = screenNavigationSystem;
            _dataProxy = dataProxy;
            Init();
        }

        private void Init()
        {
            _view.OnShowCallback += obj =>
            {
                _view.SetLoadingProgressValue(0);
                _view.SetViewModel(Application.version);
            };

            _view.OnLoadingAnimationFinish += () =>
            {
                _screenNavigationSystem.ExecuteNavigationCommand(
                    new NavigationCommand().ShowNextScreen(ScreenName.MainScreen).WithoutAnimation());

                Observable.Interval(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => { _view.CloseScreen(); });
            };

            _sceneLoader.LoadSceneProgress.Subscribe((sceneProgress) =>
            {
                _view.SetLoadingProgressValue(sceneProgress);
            }).AddTo(_compositeDisposable);
        }
    }
}