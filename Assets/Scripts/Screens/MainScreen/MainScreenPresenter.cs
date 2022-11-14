using System;
using System.Collections.Generic;
using AR;
using AR.World;
using Data;
using Data.Objects;
using GameCamera;
using Geo;
using Screens.PortalsListScreen;
using UniRx;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Screens.MainScreen
{
    public class MainScreenPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly List<PortalCardView> _cards = new();
        private readonly IMainScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IDataProxy _dataProxy;

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
                        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => { _dataProxy.NextStateStep(); })
                            .AddTo(_disposables);
                        _view.ShowBaseInterface();
                        _view.HideWarningMessage();
                        _view.ShowLocationDetectionPopup();
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

            _view.GetMapUserInterface().PortalsListClicked += OnPortalsListClicked;
            _view.GetMapUserInterface().MyPositionClicked += OnMyPositionClicked;
            _view.GetMapUserInterface().NearestPortalClicked += OnNearestPortalClicked;

            _view.WarningOkClicked += OnWarningOkClicked;

            _dataProxy.SelectedPortalZone.Subscribe(zone => { _view.SetupActiveZone(zone?.name); }).AddTo(_disposables);

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

        private void OnOpenMapClicked()
        {
            _dataProxy.ToggleMap();
        }

        private void OnRestartButtonClicked()
        {
            _dataProxy.ResetScene();
        }

        private void OnClearButtonClicked()
        {
            _dataProxy.ClearScene();
        }

        private void OnWarningOkClicked()
        {
            _dataProxy.NextStateStep();
        }

        private void OnPlaceRandomBeamClicked()
        {
            _dataProxy.PlaceRandomBeam();
        }

        private void OnNearestPortalClicked()
        {
            Vector2 target = _dataProxy.NearestPortalZone.Value.GetPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnMyPositionClicked()
        {
            Vector2 target = LocationController.GetPlayerPosition();
            OnlineMaps.instance.position = new Vector2(target.y, target.x);
        }

        private void OnPortalsListClicked()
        {
            foreach (PortalCardView portalCardView in _cards)
            {
                portalCardView.DestroyCard();
            }

            _cards.Clear();

            foreach (PortalViewInfo portalViewInfo in _dataProxy.GetAllZones())
            {
                PortalCardView portalCardView = Object.Instantiate(_view.GetZonesListView().GetCardPrefab(),
                    _view.GetZonesListView().GetListContainer());
                portalCardView.transform.SetAsLastSibling();
                portalCardView.ConfigureView(portalViewInfo);

                portalCardView.MoveToClicked += OnMoveToClicked;

                _cards.Add(portalCardView);
            }

            _view.ShowAllZonesList();
        }

        private void OnMoveToClicked(Vector2 coordinates)
        {
            OnlineMaps.instance.position = new Vector2(coordinates.y, coordinates.x);
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
                    if (hit.collider.gameObject.TryGetComponent(out GiftView gift))
                    {
                        double distance = Vector3.Distance(gift.transform.position, cameraView.transform.position);

                        Debug.Log($"gift Hit {distance}");
                        
                        if (distance > 5) return;

                        Debug.Log("gift Hit");
                        gift.Interact();
                        return;
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

        // private void SpawnPortalWithRewards()
        // {
        //     if (locationController.SelectedPortalZone.Value == null) return;
        //
        //     portalController.OpenPortalInPosition(_arController.GetPointerPosition(), _arController.GetCeilPosition());
        // }
    }
}