using System;
using System.Collections.Generic;
using System.Linq;
using AR;
using ARLocation;
using Assets;
using Core;
using Data.Objects;
using Geo;
using Mapbox.Utils;
using UniRx;
using UnityEngine;
using Utils;

namespace Data
{
    public class DataProxy : IDataProxy
    {
        private readonly AssetsScriptableObject _assetsScriptableObject;
        private readonly IApiInterface _apiInterface;
        private readonly LocalStorageHelper _localStorageHelper;
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
        private readonly PlayerData _playerData;
        private readonly List<ZoneViewInfo> _portalsList = new();
        private EventData[] _eventsData;

        public DataProxy(AssetsScriptableObject assetsScriptableObject, IApiInterface apiInterface,
            LocalStorageHelper localStorageHelper)
        {
            _assetsScriptableObject = assetsScriptableObject;
            _apiInterface = apiInterface;
            _localStorageHelper = localStorageHelper;
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

        public void SetActivePortalZone(ZoneViewInfo zoneModel)
        {
            _availableGifts.Value = zoneModel.Rewards.Count(it => !it.IsCollected);

            _selectedPortalZone.Value = zoneModel;
            SetNearestPortalZone(zoneModel);
        }

        public void SetNearestPortalZone(ZoneViewInfo zoneModel) => _nearestPortalZone.Value = zoneModel;

        public void SetPlayerPosition(Vector2 position) => _playerLocationChanged.Value = position;

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;

        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public void PlaceRandomBeam() => _placeRandomBeamForSelectedZone.OnNext(true);

        public void CollectedCoin()
        {
            _playerData.AddCoin();
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
                    Name = eventData.title,
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
                    _localStorageHelper.LoadSprite(result.prize.image, sprite => { success?.Invoke(sprite); });
                },
                error => { failed?.Invoke(); });
        }

        public void GetSpriteByUrl(string url, Action<Sprite> action)
        {
            _localStorageHelper.LoadSprite(url, sprite => { action?.Invoke(sprite); });
        }

        public Vector2 GetPlayerPosition()
        {
            return Application.isEditor ? GlobalConstants.MockPosition : _playerLocationChanged.Value;
        }
    }
}