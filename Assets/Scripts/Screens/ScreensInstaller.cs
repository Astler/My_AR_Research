using System;
using System.Collections.Generic;
using Assets;
using Data;
using ExternalTools.ImagesLoader;
using GameCamera;
using Infrastructure.GameStateMachine;
using Pointers;
using SceneManagement;
using Screens.ArModeTab;
using Screens.ArScanningPopup;
using Screens.CollectedRewards;
using Screens.DetectingLocationPopup;
using Screens.DropZoneDetailsScreen;
using Screens.Factories;
using Screens.FindDropZonesScreen;
using Screens.LoadingScreen;
using Screens.MainScreen;
using Screens.RewardClaimedScreen;
using Screens.WarningScreen;
using Toasts;
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
        private DropZonesCardsFactory _dropZonesCardsFactory;
        private IToastsController _toastsController;

        [Inject]
        public void Construct(ScreenAssets screenAssets, SceneLoader sceneLoader,
            IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy, IWebImagesLoader webImagesLoader, GameStateMachine gameStateMachine,
            RewardCardsFactory rewardCardsFactory, HistoryCardsFactory historyCardsFactory,
            DropZonesCardsFactory dropZonesCardsFactory, IToastsController toastsController)
        {
            _toastsController = toastsController;
            _dropZonesCardsFactory = dropZonesCardsFactory;
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
                        new MainScreenPresenter(mainScreenView, _screenNavigationSystem, _dataProxy, _toastsController);
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
                case ScreenName.WarningScreen:
                    InstantiateView(name.ToString(), delegate(WarningScreenView view)
                    {
                        new WarningScreenPresenter(view, _dataProxy);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.DetectingLocationPopup:
                    InstantiateView(name.ToString(), delegate(DetectingLocationPopupView view)
                    {
                        new DetectingLocationPopupPresenter(view, _dataProxy);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.ArScanningPopup:
                    InstantiateView(name.ToString(), delegate(ArScanningPopupView view)
                    {
                        new ArScanningPopupPresenter(view, _dataProxy);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.DropZonesListScreen:
                    InstantiateView(name.ToString(), delegate(ZonesListScreenView view)
                    {
                        new ZonesListScreenPresenter(view, _dataProxy, _dropZonesCardsFactory, _screenNavigationSystem);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.DropZoneDetailsScreen:
                    InstantiateView(name.ToString(), delegate(DropZoneDetailsScreenView view)
                    {
                        new DropZoneDetailsScreenPresenter(view, _dataProxy, _webImagesLoader, _rewardCardsFactory);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.RewardClaimedScreen:
                    InstantiateView(name.ToString(), delegate(RewardClaimedScreenView view)
                    {
                        new RewardClaimedScreenPresenter(view);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.RewardAlreadyClaimedScreen:
                    InstantiateView(name.ToString(), delegate(RewardClaimedScreenView view)
                    {
                        new RewardClaimedScreenPresenter(view);
                        onSuccess.Invoke(view);
                    });
                    break;
                case ScreenName.ArModeTab:
                    InstantiateView(name.ToString(), delegate(ArModeTabView view)
                    {
                        new ArModeTabPresenter(view, _dataProxy, _screenNavigationSystem, _historyCardsFactory);
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