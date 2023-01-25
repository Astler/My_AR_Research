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
            _view.OnLostFocusCallback += OnLostFocus;

            _view.EmptyScreenClicked += OnScreenClicked;
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

                        bool collectable = beam.CanBeCollected(cameraView.GetPosition());

                        if (!collectable) continue;

                        if (beam is not MannaBoxView mannaBoxView) continue;

                        Debug.Log($"hit MannaBoxView");
                        _dataProxy.TryToCollectBeam(mannaBoxView.GetBeamData(), sprite =>
                        {
                            mannaBoxView.Interact();
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

        private void OnRewardClaimed(RewardScreenViewInfo rewardScreenViewInfo, bool succeed)
        {
            _screenNavigationSystem.ExecuteNavigationCommand(
                new NavigationCommand()
                    .ShowNextScreen(succeed ? ScreenName.RewardClaimedScreen : ScreenName.RewardAlreadyClaimedScreen)
                    .WithExtraData(rewardScreenViewInfo));
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