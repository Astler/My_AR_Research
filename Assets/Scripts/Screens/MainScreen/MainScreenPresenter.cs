using System;
using System.Collections.Generic;
using AR;
using AR.World.Collectable;
using Data;
using ExternalTools.ImagesLoader;
using GameCamera;
using Geo;
using Infrastructure.GameStateMachine;
using Pointers;
using Screens.Factories;
using Screens.RewardClaimedScreen;
using Screens.RewardsListScreen;
using UniRx;
using UnityEngine;

namespace Screens.MainScreen
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IMainScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IDataProxy _dataProxy;
        private readonly GameStateMachine _gameStateMachine;

        public MainScreenPresenter(IMainScreenView view, IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy, GameStateMachine gameStateMachine)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _dataProxy = dataProxy;
            _gameStateMachine = gameStateMachine;
            
            Init();
        }

        private void Init()
        {
            _view.ClearButtonClicked += OnClearButtonClicked;
            _view.RestartButtonClicked += OnRestartButtonClicked;
            _view.EmptyScreenClicked += OnScreenClicked;
            _view.OpenMapClicked += OnOpenMapClicked;
            _view.CollectedRewardsClicked += OnCollectedRewardsClicked;
            _view.HistoryClicked += OnHistoryClicked;

            _view.MapUserInterface.PortalsListClicked += OnZonesListClicked;
            _view.MapUserInterface.RewardsListClicked += OnRewardsListClicked;
            _view.MapUserInterface.MyPositionClicked += OnMyPositionClicked;
            _view.MapUserInterface.NearestPortalClicked += OnNearestPortalClicked;
            _view.MapUserInterface.MapCloseClicked += () => _dataProxy.ToggleMap();

            _dataProxy.AvailableGifts.Subscribe(_view.SetAvailableRewards).AddTo(_disposables);

            _dataProxy.MapOpened
                .Subscribe(mapOpened =>
                    _view.SetUIFlags(mapOpened
                        ? MainScreenMode.Map | MainScreenMode.TopBar
                        : MainScreenMode.TopBar | MainScreenMode.MidContent | MainScreenMode.BottomBar))
                .AddTo(_disposables);

            _dataProxy.Coins.Subscribe(_view.SetCoins).AddTo(_disposables);
            _dataProxy.TimeToNextGift.Subscribe(_view.SetNextGiftTime).AddTo(_disposables);

            _dataProxy.GameState.Subscribe(state =>
            {
                switch (state)
                {
                    case GameStates.Loading:
                        _view.SetUIFlags(MainScreenMode.Hide);
                        Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>
                            {
                                _dataProxy.CompleteStateStep(GameStates.Loading);
                            })
                            .AddTo(_disposables);
                        break;
                    case GameStates.WarningMessage:
                        _screenNavigationSystem.ExecuteNavigationCommand(
                            new NavigationCommand().ShowNextScreen(ScreenName.WarningScreen));
                        break;
                    case GameStates.LocationDetection:
                        _screenNavigationSystem.ExecuteNavigationCommand(
                            new NavigationCommand().ShowNextScreen(ScreenName.DetectingLocationPopup));
                        _view.SetUIFlags(MainScreenMode.TopBar);
                        break;
                    case GameStates.Scanning:
                        _screenNavigationSystem.ExecuteNavigationCommand(
                            new NavigationCommand().ShowNextScreen(ScreenName.ArScanningPopup));
                        _view.SetUIFlags(MainScreenMode.TopBar | MainScreenMode.MidContent);
                        break;
                    case GameStates.Active:
                        _view.SetUIFlags(MainScreenMode.TopBar | MainScreenMode.MidContent | MainScreenMode.BottomBar);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }).AddTo(_disposables);

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

        public void Dispose() => _disposables?.Dispose();
        
        private void OnZonesListClicked()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.DropZonesListScreen));
        }

        private void OnHistoryClicked()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.HistoryScreen));
        }


        private void OnClearButtonClicked() => _dataProxy.ClearScene();

        private void OnRestartButtonClicked()
        {
            _gameStateMachine.Initialize();
            _view.CloseScreen();
            _dataProxy.ResetScene();
        }

        private void OnScreenClicked(Vector2 clickPosition)
        {
            if (_dataProxy.MapOpened.Value) return;

            Camera camera = Camera.main;

            if (!camera) return;

            Debug.Log("camera found");

            CameraView cameraView = camera.GetComponent<CameraView>();

            if (cameraView == null) return;

            Debug.Log("cameraview found");

            RaycastHit[] hits = cameraView.GetHitsByMousePosition(clickPosition);

            if (hits.Length > 0)
            {
                Debug.Log("Unity Hit");
                foreach (RaycastHit hit in hits)
                {
                    Debug.Log($"hit name = {hit.collider.gameObject.name}");

                    if (hit.collider.gameObject.TryGetComponent(out ICollectable beam))
                    {
                        Debug.Log($"hit ICollectable");

                        bool collectable = beam.CanBeCollected(cameraView.GetPosition());

                        if (!collectable) continue;

                        if (beam is not MannaBoxView mannaBoxView) continue;

                        Debug.Log($"hit MannaBoxView");
                        _dataProxy.TryToCollectBeam(mannaBoxView.GetBeamData(), sprite =>
                        {
                            mannaBoxView.Interact();
                            _dataProxy.CollectedCoin(5);
                            OnRewardClaimed(new RewardScreenViewInfo
                            {
                                ItemName = mannaBoxView.GetBeamData().Name,
                                Sprite = sprite
                            }, true);
                        }, () =>
                        {
                            mannaBoxView.Interact();

                            _dataProxy.GetSpriteByUrl(mannaBoxView.GetBeamData().Url, sprite =>
                            {
                                _dataProxy.CollectedCoin();
                                OnRewardClaimed(new RewardScreenViewInfo
                                {
                                    ItemName = mannaBoxView.GetBeamData().Name,
                                    Sprite = sprite
                                }, false);
                            });
                        });
                    }
                }
            }
            //
            // (bool hasHits, Pose? poseTransform) = _arController.CheckIfRaycastHits(clickPosition);
            //
            // if (hasHits)
            // {
            //     Debug.Log("AR Hit");
            //     Pose nonNullPose = poseTransform.GetValueOrDefault();
            //     giftsController.SpawnNewGift(nonNullPose.position, nonNullPose.rotation);
            // }
            //
            // Debug.Log("No hits. Clicked nowhere!!");
        }

        private void OnRewardsListClicked()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.RewardsListScreen));
        }

        private void OnRewardClaimed(RewardScreenViewInfo rewardScreenViewInfo, bool succeed)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand()
                    .ShowNextScreen(succeed ? ScreenName.RewardClaimedScreen : ScreenName.RewardAlreadyClaimedScreen)
                    .WithExtraData(rewardScreenViewInfo));
        }

        private void OnCollectedRewardsClicked()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand().ShowNextScreen(ScreenName.CollectedRewardsScreen));
        }

        private void OnOpenMapClicked() => _dataProxy.ToggleMap();

        private void OnNearestPortalClicked()
        {
            Vector2 target = _dataProxy.NearestPortalZone.Value.Coordinates;
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnMyPositionClicked()
        {
            Vector2 target = _dataProxy.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }
    }
}