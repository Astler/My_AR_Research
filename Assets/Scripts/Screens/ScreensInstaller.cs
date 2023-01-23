using System;
using System.Collections.Generic;
using Assets;
using Data;
using ExternalTools.ImagesLoader;
using GameCamera;
using Infrastructure.GameStateMachine;
using Pointers;
using SceneManagement;
using Screens.CollectedRewards;
using Screens.Factories;
using Screens.HistoryScreen;
using Screens.LoadingScreen;
using Screens.MainScreen;
using UnityEngine;
using Utils;
using Zenject;

namespace Screens
{
    public class ScreensInstaller : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> _localScreens = new();

        private SceneLoader _sceneLoader;
        private IScreenNavigationSystem _screenNavigationSystem;
        private IDataProxy _dataProxy;
        private IWebImagesLoader _webImagesLoader;
        private GameStateMachine _gameStateMachine;
        private RewardCardsFactory _rewardCardsFactory;
        private HistoryCardsFactory _historyCardsFactory;
        private IPointersController _pointersController;

        [Inject]
        public void Construct(ScreenAssets screenAssets, SceneLoader sceneLoader,
            IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy, IWebImagesLoader webImagesLoader, GameStateMachine gameStateMachine,
            RewardCardsFactory rewardCardsFactory, HistoryCardsFactory historyCardsFactory,
            IPointersController pointersController)
        {
            _pointersController = pointersController;
            _historyCardsFactory = historyCardsFactory;
            _rewardCardsFactory = rewardCardsFactory;
            _gameStateMachine = gameStateMachine;
            _webImagesLoader = webImagesLoader;
            _dataProxy = dataProxy;
            _sceneLoader = sceneLoader;
            _screenNavigationSystem = screenNavigationSystem;
            foreach (var somePrefab in screenAssets.screenPrefabs)
            {
                _localScreens.Add(somePrefab.name, somePrefab);
            }
        }

        private void InstantiateView<T>(string screenName, Action<T> onSuccess)
        {
            GameObject newScreen;

            if (_localScreens.ContainsKey(screenName))
            {
                newScreen = Instantiate(_localScreens[screenName], transform);
                SetupScreen(false);
            }
            else
            {
                Debug.LogWarning($"Screen {screenName} not found in local list. Try load from Addressables.");
            }

            void SetupScreen(bool fromAddressable)
            {
                newScreen.name = screenName;
                if (newScreen.TryGetComponent(out T view))
                {
                    onSuccess.Invoke(view);
                    ScreenView screenView = newScreen.GetComponent<ScreenView>();
                    if (screenView.ShouldDeleteAfterHide)
                    {
                        screenView.OnHideCallback += () => { };
                    }
                }
                else
                {
                    Debug.LogError("Script not found on instantiated screen.");
                }
            }
        }

        public void AddScreenToScene(ScreenName name, Action<ScreenView> onSuccess)
        {
            switch (name)
            {
                case ScreenName.LoadingScreen:
                    InstantiateView(name.ToString(), delegate(LoadingScreenView loadingScreenView)
                    {
                        new LoadingScreenPresenter(loadingScreenView, _sceneLoader, _screenNavigationSystem,
                            _dataProxy);
                        onSuccess.Invoke(loadingScreenView);
                    });
                    break;
                case ScreenName.MainScreen:
                    InstantiateView(name.ToString(), delegate(MainScreenView mainScreenView)
                    {
                        new MainScreenPresenter(mainScreenView, _screenNavigationSystem, _dataProxy,
                            _webImagesLoader, _gameStateMachine, _rewardCardsFactory, _pointersController);
                        onSuccess.Invoke(mainScreenView);
                    });
                    break;
                case ScreenName.CollectedRewardsScreen:
                    InstantiateView(name.ToString(), delegate(CollectedRewardsScreenView view)
                    {
                        new CollectedRewardsScreenPresenter(view, _dataProxy, _webImagesLoader, _rewardCardsFactory);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.HistoryScreen:
                    InstantiateView(name.ToString(), delegate(HistoryScreenView view)
                    {
                        new HistoryScreenPresenter(view, _dataProxy, _historyCardsFactory);
                        onSuccess.Invoke(view);
                    });
                    break;
                default:
                    Debug.LogError($"Screen {name} not found in ScreenInstaller");
                    break;
            }
        }
    }
}