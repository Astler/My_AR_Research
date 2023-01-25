using System;
using System.Collections.Generic;
using System.Linq;
using AR;
using ARLocation;
using BestHTTP.WebSocket;
using Core;
using Core.WebSockets;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Geo;
using Mapbox.Utils;
using UniRx;
using UnityEngine;
using Utils;
using CameraType = GameCamera.CameraType;

namespace Data
{
    public class DataProxy : IDataProxy
    {
        private readonly IApiInterface _apiInterface;
        private readonly WebImagesLoader _webImagesLoader;
        private readonly IWebSocketService _webSocketService;
        private EventData[] _eventsData;

        private readonly ReactiveProperty<CameraType> _activeCameraType = new(CameraType.ArCamera);
        private readonly ReactiveProperty<int> _selectedOnMapDropZoneId = new(-1);
        private readonly ReactiveProperty<GameStates> _gameState = new(GameStates.Loading);
        private readonly ReactiveCollection<HistoryStepData> _historyLines = new();

        private readonly ReactiveProperty<float> _scannedArea = new();

        private readonly Subject<bool> _reset = new();
        private readonly Subject<bool> _clear = new();
        private readonly ReactiveProperty<int> _availableGifts = new();
        private readonly ReactiveProperty<bool> _mapOpened = new();
        private readonly ReactiveProperty<DropZoneViewInfo> _selectedPortalZone = new();
        private readonly ReactiveProperty<EventData> _activeEventData = new();
        private readonly ReactiveProperty<DropZoneViewInfo> _nearestPortalZone = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();
        private readonly ReactiveProperty<LocationDetectResult> _locationDetectResult = new();
        private readonly Subject<ActiveBoxData> _placeRewardBoxInsideZone = new();
        private readonly Subject<ActiveBoxData> _removeRewardBoxFromZone = new();
        private readonly ReactiveProperty<int> _timeToNextGift = new();

        private readonly List<DropZoneViewInfo> _portalsList = new();
        private readonly List<PrizeData> _collectedPrizes = new();
        private readonly ReactiveCollection<RewardViewInfo> _collectedPrizesInfos = new();

        public DataProxy(IApiInterface apiInterface, WebImagesLoader webImagesLoader,
            IWebSocketService webSocketService)
        {
            _apiInterface = apiInterface;
            _webImagesLoader = webImagesLoader;
            _webSocketService = webSocketService;
            _webSocketService.OnIncomingMessageReceived += OnIncomingMessageReceived;

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                if (_timeToNextGift.Value > 0)
                {
                    _timeToNextGift.Value--;
                }
            });
        }

        private void OnIncomingMessageReceived(WebSocket socket, IncomingMessage message)
        {
            string type = message.GetType();

            if (type == IncomingMessagesTypes.spawn_new_box.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());
                _placeRewardBoxInsideZone.OnNext(boxData);
                _historyLines.Add(new HistoryStepData
                {
                    Message = "^new_drop".GetTranslation(),
                    TimeUtc = Extensions.GetCurrentUtcTime()
                });
            }
            else if (type == IncomingMessagesTypes.update_next_spawn_time.ToString())
            {
                BoxTimerData timerData = JsonUtility.FromJson<BoxTimerData>(message.GetData());
                _timeToNextGift.Value = Extensions.GetCurrentUtcTime() - timerData.next_spawn_time;
            }
            else if (type == IncomingMessagesTypes.update_box_status.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());
                _removeRewardBoxFromZone.OnNext(boxData);
            }
            else if (type == IncomingMessagesTypes.prize_claimed.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());
                _removeRewardBoxFromZone.OnNext(boxData);
                _availableGifts.Value -= 1;
                _historyLines.Add(new HistoryStepData
                {
                    Message = "^drop_collected_by".GetTranslation(boxData.id, boxData.user_id),
                    TimeUtc = Extensions.GetCurrentUtcTime()
                });
            }
        }

        public IReadOnlyReactiveProperty<CameraType> ActiveCameraType => _activeCameraType;
        public IReadOnlyReactiveProperty<int> SelectedOnMapDropZoneId => _selectedOnMapDropZoneId;
        public IReadOnlyReactiveCollection<HistoryStepData> SessionHistory => _historyLines;


        public IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos => _collectedPrizesInfos;
        public IReadOnlyReactiveProperty<int> AvailableGifts => _availableGifts;

        #region New

        public void SetScannedArea(float totalArea)
        {
            _scannedArea.Value = totalArea / AreaScanRequirements;
        }

        public bool IsRequestedAreaScanned() => _scannedArea.Value >= 1;

        public void CompleteStateStep(GameStates states)
        {
            if (_gameState.Value == states)
            {
                _gameState.Value = _gameState.Value.Next();
            }
        }

        #endregion


        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;

        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        public IReadOnlyReactiveProperty<DropZoneViewInfo> SelectedPortalZone => _selectedPortalZone;
        public IReadOnlyReactiveProperty<DropZoneViewInfo> NearestPortalZone => _nearestPortalZone;
        public IReadOnlyReactiveProperty<EventData> ActiveEventData => _activeEventData;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;
        public IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult => _locationDetectResult;
        public System.IObservable<ActiveBoxData> PlaceRewardBoxInsideZone => _placeRewardBoxInsideZone;
        public System.IObservable<ActiveBoxData> RemoveRewardBox => _removeRewardBoxFromZone;
        public System.IObservable<bool> Reset => _reset;
        public System.IObservable<bool> Clear => _clear;
        public IReadOnlyReactiveProperty<float> ScannedArea => _scannedArea;
        public IReadOnlyReactiveProperty<int> TimeToNextGift => _timeToNextGift;

        public void SetSelectedOnMapDropZone(int id)
        {
            _selectedOnMapDropZoneId.Value = id;
        }

        public void SetActivePortalZone(DropZoneViewInfo dropZoneModel)
        {
            if (dropZoneModel == null)
            {
                _selectedPortalZone.Value = null;
                return;
            }

            if (dropZoneModel.Id == (_activeEventData.Value?.id ?? 0)) return;

            _selectedPortalZone.Value = dropZoneModel;

            SetNearestPortalZone(dropZoneModel);

            _availableGifts.Value = dropZoneModel.Rewards.Count(it => !it.IsCollected);

            _apiInterface.ShowEventData(dropZoneModel.Id, data =>
            {
                if (data.event_data == null)
                {
                    _activeEventData.Value = null;
                    return;
                }

                _activeEventData.Value = data.event_data;
                _webSocketService.SubscribeToEventSessionChannel(dropZoneModel.Id);
            }, Debug.LogError);
        }

        public void SetNearestPortalZone(DropZoneViewInfo dropZoneModel) => _nearestPortalZone.Value = dropZoneModel;

        public void SetPlayerPosition(Vector2 position) => _playerLocationChanged.Value = position;

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;

        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public IEnumerable<DropZoneViewInfo> GetAllActiveZones()
        {
            List<DropZoneViewInfo> activeZones = _portalsList.Where(it => it.IsActive()).ToList();

            foreach (DropZoneViewInfo portalViewInfo in activeZones)
            {
                portalViewInfo.Distance =
                    portalViewInfo.Coordinates.ToHumanReadableDistanceFromPlayer(GetPlayerPosition());
            }

            return activeZones;
        }

        public void ClearScene() => _clear.OnNext(true);

        public void ResetScene()
        {
            _gameState.Value = GameStates.WarningMessage;
            _scannedArea.Value = 0;
            _reset.OnNext(true);
        }

        public bool IsInsideEvent() => _activeEventData.Value != null;

        //TODO Find usage
        public void RestartGeoLocation() => ARLocationManager.Instance.Restart();

        public void SetIsMapOpened(bool isMapOpened)
        {
            _mapOpened.Value = isMapOpened;
            SetActiveCamera(MapOpened.Value ? CameraType.MapCamera : CameraType.Disabled);
        }

        public void SetActiveCamera(CameraType type)
        {
            _activeCameraType.Value = type;
        }

        public void LoadEvents()
        {
            _apiInterface.GetEventsList(AddEvents, Debug.LogError);
        }

        public void LoadClaimedRewards()
        {
            _apiInterface.GetAllCollectedRewardsList(AddClaimedRewards, Debug.LogError);
        }

        private void AddClaimedRewards(CollectedPrizesData collectedPrizesData)
        {
            _collectedPrizes.Clear();
            _collectedPrizes.AddRange(collectedPrizesData.prizes);

            _collectedPrizesInfos.Clear();

            foreach (PrizeData collectedPrize in _collectedPrizes)
            {
                _collectedPrizesInfos.Add(collectedPrize.ToRewardViewInfo());
            }
        }

        public void AddEvents(EventsData data)
        {
            _portalsList.Clear();
            _eventsData = data.events;

            foreach (EventData eventData in _eventsData)
            {
                DropZoneViewInfo viewInfo = new()
                {
                    Id = eventData.id,
                    Name = eventData.title,
                    MinimumDropDistance = eventData.drop_distance_min,
                    MaximumDropDistance = eventData.drop_distance_max,
                    InitialBoxes = eventData.initial_boxes,
                    MaximumBoxes = eventData.simultaneous_boxes,
                    Radius = eventData.radius,
                    StartTime = eventData.start_time,
                    FinishTime = eventData.finish_time,
                    Coordinates = new Vector2d(eventData.latitude, eventData.longitude).ToUnityVector(),
                    Rewards = eventData.prizes.Select(it => new RewardViewInfo
                    {
                        Url = it.image,
                        IsCollected = it.is_claimed,
                        Id = it.id,
                        EventId = eventData.id,
                        Name = it.name
                    }).ToList(),
                };

                _portalsList.Add(viewInfo);
            }
        }

        public IEnumerable<RewardViewInfo> GetRewardsForActiveZone()
        {
            if (SelectedPortalZone.Value == null) return Array.Empty<RewardViewInfo>();
            return SelectedPortalZone.Value.Rewards;
        }

        public RewardViewInfo GetAvailableRewardForZone()
        {
            List<RewardViewInfo> rewards = GetRewardsForActiveZone().Where(it => !it.IsCollected).ToList();
            return rewards.Count == 0 ? null : rewards.GetRandomElement();
        }

        public void TryToCollectBeam(BeamData data, Action<Sprite> success, Action failed)
        {
            _apiInterface.CollectReward(_activeEventData.Value?.id ?? 0, data.Id,
                result =>
                {
                    LoadEvents();
                    _webImagesLoader.TryToLoadSprite(result.prize.image, sprite => { success?.Invoke(sprite); });
                },
                error =>
                {
                    LoadEvents();
                    failed?.Invoke();
                });
        }

        public void GetSpriteByUrl(string url, Action<Sprite> action)
        {
            _webImagesLoader.TryToLoadSprite(url, sprite => { action?.Invoke(sprite); });
        }

        public void RefreshCollectedRewards() => LoadClaimedRewards();

        private const float AreaScanRequirements = 100;

        public Vector2 GetPlayerPosition()
        {
            return Application.isEditor ? GlobalConstants.MockPosition : _playerLocationChanged.Value;
        }
    }
}