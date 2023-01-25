using System;
using Data;
using Geo;
using UniRx;
using UnityEngine;
using static Screens.MainScreen.BottomBarButtonType;

namespace Screens.MainScreen
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IMainScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IDataProxy _dataProxy;

        private readonly BottomBarButtonType _startTab = Find;

        private BottomBarButtonType? _selectedTab;

        public MainScreenPresenter(IMainScreenView view, IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _dataProxy = dataProxy;

            Init();
        }

        private void Init()
        {
            Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => { ConfigureBySelectedTab(_startTab); })
                .AddTo(_disposables);

            _view.BottomNavigationBar.ClickedNavigationBarButton += ConfigureBySelectedTab;
            _view.MapUserInterface.NearestPortalClicked += OnNearestPortalClicked;

            _dataProxy.AvailableGifts.Subscribe(_view.SetAvailableRewards).AddTo(_disposables);
            _dataProxy.TimeToNextGift.Subscribe(_view.SetNextGiftTime).AddTo(_disposables);

            // _dataProxy.GameState.Subscribe(state =>
            // {
            //     switch (state)
            //     {
            //         case GameStates.Loading:
            //             _view.SetUIFlags(MainScreenMode.Hide);
            //             Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
            //                 {
            //                     _dataProxy.CompleteStateStep(GameStates.Loading);
            //                 })
            //                 .AddTo(_disposables);
            //             break;
            //         case GameStates.WarningMessage:
            //             _screenNavigationSystem.ExecuteNavigationCommand(
            //                 new NavigationCommand().ShowNextScreen(ScreenName.WarningScreen));
            //             break;
            //         case GameStates.LocationDetection:
            //             _screenNavigationSystem.ExecuteNavigationCommand(
            //                 new NavigationCommand().ShowNextScreen(ScreenName.DetectingLocationPopup));
            //             _view.SetUIFlags(MainScreenMode.TopBar);
            //             break;
            //         case GameStates.Scanning:
            //             _screenNavigationSystem.ExecuteNavigationCommand(
            //                 new NavigationCommand().ShowNextScreen(ScreenName.ArScanningPopup));
            //             _view.SetUIFlags(MainScreenMode.TopBar | MainScreenMode.MidContent);
            //             break;
            //         case GameStates.Active:
            //             _view.SetUIFlags(MainScreenMode.TopBar | MainScreenMode.MidContent | MainScreenMode.BottomBar);
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException(nameof(state), state, null);
            //     }
            // }).AddTo(_disposables);

            _dataProxy.SelectedPortalZone.Subscribe(zone => { _view.SetupActiveZone(zone?.Name); }).AddTo(_disposables);

            _dataProxy.LocationDetectResult.Subscribe(result =>
            {
                switch (result)
                {
                    case LocationDetectResult.NoPermission:
                        _view.ShowLocationSearchStatus("Location permission denied.");
                        break;
                    case LocationDetectResult.Timeout:
                        _view.ShowLocationSearchStatus("Location request timed out.");
                        break;
                    case LocationDetectResult.Error:
                        _view.ShowLocationSearchStatus("Location request undefined error.");
                        break;
                    case LocationDetectResult.Success:
                        _view.ShowLocationSearchStatus("Location detected!");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(result), result, null);
                }
            }).AddTo(_disposables);
        }

        private void ConfigureBySelectedTab(BottomBarButtonType type)
        {
            if (type is Ball or Games)
            {
                //TODO hint
                Debug.Log("not available yet");
                return;
            }

            if (_selectedTab == type)
            {
                Debug.Log("already selected");
                return;
            }

            _selectedTab = type;

            _view.BottomNavigationBar.SetSelectedButton(_selectedTab ?? _startTab);

            switch (type)
            {
                case Find:
                    OnFindDropZonesClicked();
                    break;
                case MyDrops:
                    OnMyDropsClicked();
                    break;
                case ArCamera:
                    OnArCameraClicked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void Dispose() => _disposables?.Dispose();

        #region New

        private void OnFindDropZonesClicked() => ChangeTab(ScreenName.DropZonesListScreen);

        private void OnMyDropsClicked() => ChangeTab(ScreenName.CollectedRewardsScreen);

        private void OnArCameraClicked() => ChangeTab(ScreenName.ArModeTab);

        private void ChangeTab(ScreenName tabName)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(tabName).CloseOtherTabs());
        }

        #endregion

        private void OnNearestPortalClicked()
        {
            Vector2 target = _dataProxy.NearestPortalZone.Value.Coordinates;
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }
    }
}