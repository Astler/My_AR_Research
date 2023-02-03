using System;
using AR;
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

            _view.BottomNavigationBar.ClickedNavigationBarButton += type => ConfigureBySelectedTab(type);
            _view.MapUserInterface.NearestPortalClicked += OnNearestPortalClicked;

            _dataProxy.BottomNavigationAction.Subscribe(tuple => { ConfigureBySelectedTab(tuple.type, tuple.data); })
                .AddTo(_disposables);
            
            _dataProxy.HasNewCollectedDrops.Subscribe(has => { _view.BottomNavigationBar.SetIsHasNewDrops(has); })
                .AddTo(_disposables);

            _view.ConfigureView(_dataProxy.GetUserInfo());

            _view.MenuClicked += () =>
            {
                _screenNavigationSystem.ExecuteNavigationCommand(
                    new NavigationCommand().ShowNextScreen(ScreenName.MenuScreen).WithAnimation());
            };

            // _dataProxy.GameState.Subscribe(state =>
            // {
            //     switch (state)
            //     {
            //         case GameStates.Loading:
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
            //             if (_dataProxy.LocationDetectResult.Value != LocationDetectResult.Success)
            //             {
            //                 _screenNavigationSystem.ExecuteNavigationCommand(
            //                     new NavigationCommand().ShowNextScreen(ScreenName.DetectingLocationPopup));
            //             }
            //
            //             break;
            //     }
            // }).AddTo(_disposables);

            // _dataProxy.LocationDetectResult.Subscribe(result =>
            // {
            //     switch (result)
            //     {
            //         case LocationDetectResult.NoPermission:
            //             _view.ShowLocationSearchStatus("Location permission denied.");
            //             break;
            //         case LocationDetectResult.Timeout:
            //             _view.ShowLocationSearchStatus("Location request timed out.");
            //             break;
            //         case LocationDetectResult.Error:
            //             _view.ShowLocationSearchStatus("Location request undefined error.");
            //             break;
            //         case LocationDetectResult.Success:
            //             _view.ShowLocationSearchStatus("Location detected!");
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException(nameof(result), result, null);
            //     }
            // }).AddTo(_disposables);
        }

        private void ConfigureBySelectedTab(BottomBarButtonType type, object data = null)
        {
            if (_selectedTab == type && data == null)
            {
                Debug.Log("already selected");
                return;
            }

            _selectedTab = type;

            _view.BottomNavigationBar.SetSelectedButton(_selectedTab ?? _startTab);
            _view.BottomNavigationBar.SetIsTransparentBar(_selectedTab == ArCamera);

            switch (type)
            {
                case Find:
                    OnFindDropZonesClicked(data);
                    break;
                case MyDrops:
                    OnMyDropsClicked(data);
                    break;
                case ArCamera:
                    OnArCameraClicked(data);
                    break;
                case Ball:
                    OnAchievementsClicked(data);
                    break;
                case Games:
                    OnGamesClicked(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void Dispose() => _disposables?.Dispose();

        #region New

        private void OnGamesClicked(object data) => ChangeTab(ScreenName.ArGamesTab, data);

        private void OnAchievementsClicked(object data) => ChangeTab(ScreenName.AchievementsTab, data);

        private void OnFindDropZonesClicked(object data) => ChangeTab(ScreenName.DropZonesListScreen, data);

        private void OnMyDropsClicked(object data) => ChangeTab(ScreenName.CollectedRewardsScreen, data);

        private void OnArCameraClicked(object data) => ChangeTab(ScreenName.ArModeTab, data);

        private void ChangeTab(ScreenName tabName, object data = null)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(tabName).CloseOtherTabs().WithExtraData(data));
        }

        #endregion

        private void OnNearestPortalClicked()
        {
            Debug.Log("removed for now..");
            // Vector2 target = _dataProxy.NearestPortalZone.Value.Coordinates;
            // OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }
    }
}