using System;
using System.Collections.Generic;
using System.Linq;
using AR.World.Collectable;
using Data;
using Data.Objects;
using GameCamera;
using Screens.Factories;
using Screens.RewardClaimedScreen;
using UniRx;
using UnityEngine;
using CameraType = GameCamera.CameraType;

namespace Screens.ArModeTab
{
    public class ArModeTabPresenter
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly IDataProxy _dataProxy;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly HistoryCardsFactory _historyCardsFactory;
        private readonly IArModeTabView _view;
        private IDisposable _historyEventsListener;
        private readonly List<HistoryEventCardView> _eventsList = new();

        public ArModeTabPresenter(IArModeTabView view, IDataProxy dataProxy,
            IScreenNavigationSystem screenNavigationSystem, HistoryCardsFactory historyCardsFactory)
        {
            _view = view;
            _dataProxy = dataProxy;
            _screenNavigationSystem = screenNavigationSystem;
            _historyCardsFactory = historyCardsFactory;

            Initialize();
        }

        private void Initialize()
        {
            _view.OnShowCallback += OnShowTab;
            _view.OnHideCallback += OnLostFocus;
            _view.EmptyScreenClicked += OnScreenClicked;
            _view.CollectButtonClicked += OnCollectButtonClicked;

            _dataProxy.AvailableGifts.Subscribe(_view.SetAvailableRewards).AddTo(_compositeDisposable);
            _dataProxy.TimeToNextGift.Subscribe(_view.SetTimeToNextDrop).AddTo(_compositeDisposable);

            _dataProxy.EnteredPortalZone.Subscribe(zone => { _view.SetDropZoneName(zone?.Name); })
                .AddTo(_compositeDisposable);

            _dataProxy.AvailableCollectables.ObserveCountChanged().Subscribe(size =>
            {
                _view.SetCollectButtonIsActive(size > 0);
            }).AddTo(_compositeDisposable);

            _dataProxy.ScannedArea.Subscribe(scanned =>
            {
                _view.IsScanActive(_dataProxy.IsRequestedAreaScanned(), scanned);
            }).AddTo(_compositeDisposable);
        }

        private void OnCollectButtonClicked()
        {
            ICollectable view = _dataProxy.AvailableCollectables.FirstOrDefault();

            if (view is not MannaBoxView mannaBoxView) return;

            BeamData data = mannaBoxView.GetBeamData();

            _dataProxy.TryToCollectBeam(data, prizeData => { OnRewardClaimed(mannaBoxView, prizeData, true); },
                () =>
                {
                    OnRewardClaimed(mannaBoxView, new PrizeData()
                    {
                        name = "Claimed!"
                    }, false);
                });
        }

        private void OnLostFocus()
        {
            _dataProxy.SetActiveCamera(CameraType.Disabled);
            _historyEventsListener?.Dispose();
        }

        private void OnShowTab(object data)
        {
            _dataProxy.SetActiveCamera(CameraType.ArCamera);
            _historyEventsListener = _dataProxy.SessionHistory.ObserveCountChanged().Subscribe(_ => LoadHistory());
            LoadHistory();
        }

        private void OnScreenClicked(Vector2 clickPosition)
        {
            if (_dataProxy.MapOpened.Value) return;

            Camera camera = Camera.main;

            if (!camera) return;

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

                        bool collectable = beam.IsCanBeCollected(cameraView.GetPosition());

                        if (!collectable) continue;

                        if (beam is not MannaBoxView mannaBoxView) continue;

                        BeamData data = mannaBoxView.GetBeamData();

                        _dataProxy.TryToCollectBeam(data,
                            prizeData => { OnRewardClaimed(mannaBoxView, prizeData, true); },
                            () =>
                            {
                                OnRewardClaimed(mannaBoxView, new PrizeData()
                                {
                                    name = "Claimed!"
                                }, false);
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

        private void OnRewardClaimed(MannaBoxView view, PrizeData data, bool succeed)
        {
            view.Interact();
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand()
                    .ShowNextScreen(succeed ? ScreenName.RewardClaimedScreen : ScreenName.RewardAlreadyClaimedScreen)
                    .WithExtraData(new RewardScreenViewInfo
                    {
                        ItemName = data.name,
                        ImageUrl = data.image
                    }));
        }

        private void LoadHistory()
        {
            foreach (HistoryEventCardView rewardView in _eventsList)
            {
                rewardView.Dispose();
            }

            foreach (HistoryStepData stepData in _dataProxy.SessionHistory.Reverse())
            {
                stepData.Parent = _view.EventsParent;
                HistoryEventCardView eventCardView = _historyCardsFactory.Create(stepData);
                _eventsList.Add(eventCardView);
            }
        }
    }
}