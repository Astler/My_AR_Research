using System;
using System.Collections.Generic;
using System.Linq;
using AR;
using ARLocation;
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
        private readonly Subject<bool> _reset = new();
        private readonly Subject<bool> _clear = new();
        private readonly ReactiveProperty<int> _availableGifts = new();
        private readonly ReactiveProperty<bool> _inRewardZone = new();
        private readonly ReactiveProperty<bool> _mapOpened = new();
        private readonly ReactiveProperty<float> _distanceToClosestReward = new();
        private readonly ReactiveProperty<GameStates> _gameState = new(GameStates.Loading);
        private readonly ReactiveProperty<ZoneViewInfo> _selectedPortalZone = new();
        private readonly ReactiveProperty<ZoneViewInfo> _nearestPortalZone = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();
        private readonly ReactiveProperty<LocationDetectResult> _locationDetectResult = new();
        private readonly Subject<bool> _placeRandomBeamForSelectedZone = new();
        private readonly ReactiveProperty<int> _coins = new();
        private readonly ReactiveProperty<float> _timeToNextGift = new();
        private readonly PlayerData _playerData;
        private readonly List<ZoneViewInfo> _portalsList = new();
        private EventData[] _eventsData;

        public DataProxy(IApiInterface apiInterface, WebImagesLoader webImagesLoader,
            IWebSocketService webSocketService)
        {
            _apiInterface = apiInterface;
            _webImagesLoader = webImagesLoader;
            _webSocketService = webSocketService;
            _playerData = new PlayerData();
            _coins.Value = _playerData.GetCoins();
        }

        public IReadOnlyReactiveProperty<int> AvailableGifts => _availableGifts;
        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        public IReadOnlyReactiveProperty<bool> InRewardZone => _inRewardZone;
        public IReadOnlyReactiveProperty<float> DistanceToClosestReward => _distanceToClosestReward;
        public IReadOnlyReactiveProperty<ZoneViewInfo> SelectedPortalZone => _selectedPortalZone;
        public IReadOnlyReactiveProperty<ZoneViewInfo> NearestPortalZone => _nearestPortalZone;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;
        public IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult => _locationDetectResult;
        public System.IObservable<bool> PlaceRandomBeamForSelectedZone => _placeRandomBeamForSelectedZone;
        public System.IObservable<bool> Reset => _reset;
        public System.IObservable<bool> Clear => _clear;
        public IReadOnlyReactiveProperty<int> Coins => _coins;
        public IReadOnlyReactiveProperty<float> TimeToNextGift => _timeToNextGift;

        public void SetTimeToNextGift(float time)
        {
            _timeToNextGift.Value = time;
        }
        
        public void SetActivePortalZone(ZoneViewInfo zoneModel)
        {
            _availableGifts.Value = zoneModel.Rewards.Count(it => !it.IsCollected);

            _selectedPortalZone.Value = zoneModel;
            SetNearestPortalZone(zoneModel);

            _webSocketService.SubscribeToEventSessionChannel(zoneModel.Id);
        }

        public void SetNearestPortalZone(ZoneViewInfo zoneModel) => _nearestPortalZone.Value = zoneModel;

        public void SetPlayerPosition(Vector2 position) => _playerLocationChanged.Value = position;

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;

        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public void PlaceRandomBeam() => _placeRandomBeamForSelectedZone.OnNext(true);

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

        public void ResetScene() => _reset.OnNext(true);

        public void RestartGeoLocation() => ARLocationManager.Instance.Restart();

        public void ToggleMap()
        {
            _mapOpened.Value = !_mapOpened.Value;
        }

        public void LoadEvents()
        {
            _apiInterface.GetEventsList(AddEvents, status => { });
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
                    Radius = eventData.radius,
                    StartTime = eventData.start_time,
                    FinishTime = eventData.finish_time,
                    Coordinates = new Vector2d(eventData.latitude, eventData.longitude).ToUnityVector(),
                    Rewards = eventData.prizes.Select(it => new RewardViewInfo
                    {
                        Url = it.image,
                        IsCollected = it.is_claimed,
                        Id = it.id,
                        ZoneId = eventData.id,
                        Name = it.name
                    }).ToList(),
                };

                if (_selectedPortalZone.Value != null && _selectedPortalZone.Value.Id == viewInfo.Id)
                {
                    SetActivePortalZone(viewInfo);
                }

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
            _apiInterface.CollectReward(data.ZoneId, data.Id,
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

        public Vector2 GetPlayerPosition()
        {
            return Application.isEditor ? GlobalConstants.MockPosition : _playerLocationChanged.Value;
        }
    }
}