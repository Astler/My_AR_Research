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

namespace Data
{
    public class DataProxy : IDataProxy
    {
        private readonly IApiInterface _apiInterface;
        private readonly WebImagesLoader _webImagesLoader;
        private readonly IWebSocketService _webSocketService;
        private readonly PlayerData _playerData;
        private EventData[] _eventsData;

        private readonly Subject<bool> _reset = new();
        private readonly Subject<bool> _clear = new();
        private readonly ReactiveProperty<int> _availableGifts = new();
        private readonly ReactiveProperty<bool> _inRewardZone = new();
        private readonly ReactiveProperty<bool> _mapOpened = new();
        private readonly ReactiveProperty<float> _distanceToClosestReward = new();
        private readonly ReactiveProperty<GameStates> _gameState = new(GameStates.Loading);
        private readonly ReactiveProperty<ZoneViewInfo> _selectedPortalZone = new();
        private readonly ReactiveProperty<EventData> _activeEventData = new();
        private readonly ReactiveProperty<ZoneViewInfo> _nearestPortalZone = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();
        private readonly ReactiveProperty<LocationDetectResult> _locationDetectResult = new();
        private readonly Subject<ActiveBoxData> _placeRewardBoxInsideZone = new();
        private readonly Subject<ActiveBoxData> _removeRewardBoxFromZone = new();
        private readonly ReactiveProperty<int> _coins = new();
        private readonly ReactiveProperty<bool> _surfaceScanned = new();
        private readonly ReactiveProperty<float> _scannedArea = new();
        private readonly ReactiveProperty<int> _timeToNextGift = new();

        private readonly List<ZoneViewInfo> _portalsList = new();
        private readonly List<PrizeData> _collectedPrizes = new();
        private readonly ReactiveCollection<RewardViewInfo> _collectedPrizesInfos = new();
        private readonly ReactiveCollection<HistoryStepData> _historyLines = new();

        public DataProxy(IApiInterface apiInterface, WebImagesLoader webImagesLoader,
            IWebSocketService webSocketService)
        {
            _apiInterface = apiInterface;
            _webImagesLoader = webImagesLoader;
            _webSocketService = webSocketService;
            _playerData = new PlayerData();
            _coins.Value = _playerData.GetCoins();

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
                    Message = $"New box spawned!",
                    TimeUtc = Extensions.GetCurrentUtcTime()
                });
            }
            else if (type == IncomingMessagesTypes.update_next_spawn_time.ToString())
            {
                BoxTimerData timerData = JsonUtility.FromJson<BoxTimerData>(message.GetData());
                Debug.Log(
                    $"current utc time {Extensions.GetCurrentUtcTime()} and next reward time {timerData.next_spawn_time}");
                _timeToNextGift.Value = Extensions.GetCurrentUtcTime() - timerData.next_spawn_time;
                _historyLines.Add(new HistoryStepData
                {
                    Message = $"Next prize will be available in {_timeToNextGift.Value} seconds",
                    TimeUtc = Extensions.GetCurrentUtcTime()
                });
            }
            else if (type == IncomingMessagesTypes.update_box_status.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());
                _removeRewardBoxFromZone.OnNext(boxData);
                // _historyLines.Add(new HistoryStepData
                // {
                //     Message = $"Old sad box was hidden!",
                //     TimeUtc = Extensions.GetCurrentUtcTime()
                // });
            }
            else if (type == IncomingMessagesTypes.prize_claimed.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());
                _removeRewardBoxFromZone.OnNext(boxData);
                _availableGifts.Value -= 1;
                _historyLines.Add(new HistoryStepData
                {
                    Message = $"User {boxData.user_id} claimed prize {boxData.id}",
                    TimeUtc = Extensions.GetCurrentUtcTime()
                });
            }
        }

        public IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos => _collectedPrizesInfos;
        public IReadOnlyReactiveCollection<HistoryStepData> SessionHistory => _historyLines;
        public IReadOnlyReactiveProperty<int> AvailableGifts => _availableGifts;
        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        public IReadOnlyReactiveProperty<bool> InRewardZone => _inRewardZone;
        public IReadOnlyReactiveProperty<float> DistanceToClosestReward => _distanceToClosestReward;
        public IReadOnlyReactiveProperty<ZoneViewInfo> SelectedPortalZone => _selectedPortalZone;
        public IReadOnlyReactiveProperty<ZoneViewInfo> NearestPortalZone => _nearestPortalZone;
        public IReadOnlyReactiveProperty<EventData> ActiveEventData => _activeEventData;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;
        public IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult => _locationDetectResult;
        public System.IObservable<ActiveBoxData> PlaceRewardBoxInsideZone => _placeRewardBoxInsideZone;
        public System.IObservable<ActiveBoxData> RemoveRewardBox => _removeRewardBoxFromZone;
        public System.IObservable<bool> Reset => _reset;
        public System.IObservable<bool> Clear => _clear;
        public IReadOnlyReactiveProperty<int> Coins => _coins;
        public IReadOnlyReactiveProperty<bool> SurfaceScanned => _surfaceScanned;
        public IReadOnlyReactiveProperty<float> ScannedArea => _scannedArea;
        public IReadOnlyReactiveProperty<int> TimeToNextGift => _timeToNextGift;

        public void SetActivePortalZone(ZoneViewInfo zoneModel)
        {
            if (zoneModel == null)
            {
                _selectedPortalZone.Value = null;
                return;
            }

            if (zoneModel.Id == (_activeEventData.Value?.id ?? 0)) return;

            _selectedPortalZone.Value = zoneModel;

            SetNearestPortalZone(zoneModel);

            _availableGifts.Value = zoneModel.Rewards.Count(it => !it.IsCollected);

            _apiInterface.ShowEventData(zoneModel.Id, data =>
            {
                if (data.event_data == null)
                {
                    _activeEventData.Value = null;
                    return;
                }

                _activeEventData.Value = data.event_data;
                _webSocketService.SubscribeToEventSessionChannel(zoneModel.Id);
            }, Debug.LogError);
        }

        public void SetNearestPortalZone(ZoneViewInfo zoneModel) => _nearestPortalZone.Value = zoneModel;

        public void SetPlayerPosition(Vector2 position) => _playerLocationChanged.Value = position;

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;

        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public void CollectedCoin(int amount = 1)
        {
            _playerData.AddCoins(amount);
            _coins.Value = _playerData.GetCoins();
        }

        public IEnumerable<ZoneViewInfo> GetAllActiveZones()
        {
            List<ZoneViewInfo> activeZones = _portalsList.Where(it => it.IsActive()).ToList();

            foreach (ZoneViewInfo portalViewInfo in activeZones)
            {
                portalViewInfo.Distance =
                    portalViewInfo.Coordinates.ToHumanReadableDistanceFromPlayer(GetPlayerPosition());
            }

            return activeZones;
        }

        public void NextStateStep()
        {
            _gameState.Value = _gameState.Value.Next();
        }

        public void ClearScene() => _clear.OnNext(true);

        public void ResetScene()
        {
            _scannedArea.Value = 0;
            _surfaceScanned.Value = false;
            _reset.OnNext(true);
        }

        public bool IsInsideEvent() => _activeEventData.Value != null;

        public void RestartGeoLocation() => ARLocationManager.Instance.Restart();

        public void ToggleMap()
        {
            _mapOpened.Value = !_mapOpened.Value;
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
                ZoneViewInfo viewInfo = new()
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

        public void SetScannedArea(float totalArea)
        {
            _scannedArea.Value = totalArea / AreaScanRequirements;
            _surfaceScanned.Value = _scannedArea.Value >= 1;
        }

        private const float AreaScanRequirements = 100;

        public Vector2 GetPlayerPosition()
        {
            return Application.isEditor ? GlobalConstants.MockPosition : _playerLocationChanged.Value;
        }
    }
}