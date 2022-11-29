using System;
using System.Collections.Generic;
using AR;
using AR.World;
using Data;
using Data.Objects;
using GameCamera;
using Geo;
using Infrastructure.GameStateMachine;
using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens.PortalsListScreen;
using Screens.RewardsListScreen;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Object = UnityEngine.Object;

namespace Screens.MainScreen
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly List<PortalCardView> _zonesList = new();
        private readonly List<RewardCardView> _rewardsList = new();
        private readonly IMainScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IDataProxy _dataProxy;
        private readonly ILocalStorageHelper _localStorageHelper;
        private readonly GameStateMachine _gameStateMachine;

        public MainScreenPresenter(IMainScreenView view, IScreenNavigationSystem screenNavigationSystem,
            IDataProxy dataProxy, ILocalStorageHelper localStorageHelper, GameStateMachine gameStateMachine)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _dataProxy = dataProxy;
            _localStorageHelper = localStorageHelper;
            _gameStateMachine = gameStateMachine;
            Init();
        }

        private void Init()
        {
            _view.PlaceRandomBeamClicked += OnPlaceRandomBeamClicked;
            _view.ClearButtonClicked += OnClearButtonClicked;
            _view.RestartButtonClicked += OnRestartButtonClicked;
            _view.EmptyScreenClicked += OnScreenClicked;
            _view.OpenMapClicked += OnOpenMapClicked;

            _dataProxy.MapOpened.Subscribe(isOpened => { _view.SetIsMapActive(isOpened); }).AddTo(_disposables);

            _dataProxy.GameState.Subscribe(state =>
            {
                switch (state)
                {
                    case GameStates.Loading:
                        _view.HideInterface();
                        Observable.Timer(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => { _dataProxy.NextStateStep(); })
                            .AddTo(_disposables);
                        break;
                    case GameStates.WarningMessage:
                        _view.ShowWarningMessage();
                        break;
                    case GameStates.LocationDetection:
                        _dataProxy.LocationDetectResult.Subscribe(result =>
                        {
                            if (result == LocationDetectResult.Success || Application.isEditor)
                            {
                                _dataProxy.NextStateStep();
                                _view.HideLocationDetectionPopup();
                            }
                            else
                            {
                                _view.ShowLocationDetectionPopup();
                            }
                        }).AddTo(_disposables);

                        _view.ShowBaseInterface();
                        _view.HideWarningMessage();
                        break;
                    case GameStates.Scanning:
                        Observable.Timer(TimeSpan.FromSeconds(4f)).Subscribe(_ => { _dataProxy.NextStateStep(); })
                            .AddTo(_disposables);
                        _view.HideLocationDetectionPopup();
                        _view.ShowScanningPopup();
                        break;
                    case GameStates.Active:
                        _view.HideScanningPopup();
                        _view.ShowGameInterface();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }).AddTo(_disposables);

            _dataProxy.Coins.Subscribe(_view.SetCoins).AddTo(_disposables);

            _view.GetMapUserInterface().PortalsListClicked += OnZonesListClicked;
            _view.GetMapUserInterface().RewardsListClicked += OnRewardsListClicked;
            _view.GetMapUserInterface().MyPositionClicked += OnMyPositionClicked;
            _view.GetMapUserInterface().NearestPortalClicked += OnNearestPortalClicked;
            _view.GetMapUserInterface().MapCloseClicked += () => _dataProxy.ToggleMap();

            _view.WarningOkClicked += OnWarningOkClicked;

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

        private void OnRewardsListClicked()
        {
            foreach (RewardCardView rewardView in _rewardsList)
            {
                rewardView.DestroyCard();
            }

            _rewardsList.Clear();

            foreach (RewardViewInfo rewardViewInfo in _dataProxy.GetRewardsForActiveZone())
            {
                RewardCardView portalCardView = Object.Instantiate(_view.GetRewardsListView().GetRewardsPrefab(),
                    _view.GetRewardsListView().GetListContainer());

                portalCardView.SetupCardData(rewardViewInfo.Name, rewardViewInfo.IsCollected);

                _localStorageHelper.LoadSprite(rewardViewInfo.Url, sprite => { portalCardView.SetRewardIcon(sprite); });

                portalCardView.transform.SetAsLastSibling();
                _rewardsList.Add(portalCardView);
            }

            _view.ShowRewardsList();
        }

        private void OnOpenMapClicked() => _dataProxy.ToggleMap();

        private void OnRestartButtonClicked()
        {
            _gameStateMachine.Initialize();
            _view.CloseScreen();
            _dataProxy.ResetScene();
        }

        private void OnClearButtonClicked() => _dataProxy.ClearScene();

        private void OnWarningOkClicked() => _dataProxy.NextStateStep();

        private void OnPlaceRandomBeamClicked() => _dataProxy.PlaceRandomBeam();

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

        private void OnZonesListClicked()
        {
            foreach (PortalCardView portalCardView in _zonesList)
            {
                portalCardView.DestroyCard();
            }

            _zonesList.Clear();

            foreach (ZoneViewInfo portalViewInfo in _dataProxy.GetAllActiveZones())
            {
                PortalCardView portalCardView = Object.Instantiate(_view.GetZonesListView().GetCardPrefab(),
                    _view.GetZonesListView().GetListContainer());
                portalCardView.transform.SetAsLastSibling();
                portalCardView.ConfigureView(portalViewInfo);

                portalCardView.MoveToClicked += OnMoveToClicked;

                _zonesList.Add(portalCardView);
            }

            _view.ShowAllZonesList();
        }

        private void OnMoveToClicked(Vector2 coordinates)
        {
            OnlineMaps.instance.position = new Vector2(coordinates.y, coordinates.x);
            _view.HideZonesList();
        }

        public void Dispose() => _disposables?.Dispose();

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

                    if (hit.collider.gameObject.TryGetComponent(out ArBeam beam))
                    {
                        if (beam.CanBeCollected(_dataProxy.GetPlayerPosition()))
                        {
                            _dataProxy.TryToCollectBeam(beam.GetBeamData(), sprite =>
                            {
                                beam.Collect();
                                _view.ShowRewardPopup(sprite, beam.GetBeamData().Name);
                            }, () =>
                            {
                                beam.Collect();
                                
                                _dataProxy.GetSpriteByUrl(beam.GetBeamData().Url, sprite =>
                                {
                                    _view.ShowAlreadyClaimedRewardPopup(sprite, beam.GetBeamData().Name);
                                });
                            });
                        }
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
    }
}