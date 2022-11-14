using System.Collections.Generic;
using AR;
using ARLocation;
using Assets;
using Data.Objects;
using Geo;
using UniRx;
using UnityEngine;
using Utils;

namespace Data
{
    public class DataProxy : IDataProxy
    {
        private readonly AssetsScriptableObject _assetsScriptableObject;
        private Subject<bool> _reset = new();
        private Subject<bool> _clear = new();
        private ReactiveProperty<bool> _inRewardZone = new();
        private ReactiveProperty<bool> _mapOpened = new();
        private ReactiveProperty<float> _distanceToClosestReward = new();
        private readonly ReactiveProperty<GameStates> _gameState = new(GameStates.Loading);
        private readonly ReactiveProperty<PortalZoneModel> _selectedPortalZone = new();
        private readonly ReactiveProperty<PortalZoneModel> _nearestPortalZone = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();
        private readonly ReactiveProperty<LocationDetectResult> _locationDetectResult = new();
        private readonly Subject<bool> _placeRandomBeamForSelectedZone = new();
        private ReactiveProperty<int> _coins = new();
        private readonly PlayerData _playerData;

        public DataProxy(AssetsScriptableObject assetsScriptableObject)
        {
            _assetsScriptableObject = assetsScriptableObject;
            _playerData = new PlayerData();
            _coins.Value = _playerData.GetCoins();
        }

        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        public IReadOnlyReactiveProperty<bool> InRewardZone => _inRewardZone;
        public IReadOnlyReactiveProperty<float> DistanceToClosestReward => _distanceToClosestReward;
        public IReadOnlyReactiveProperty<PortalZoneModel> SelectedPortalZone => _selectedPortalZone;
        public IReadOnlyReactiveProperty<PortalZoneModel> NearestPortalZone => _nearestPortalZone;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;
        public IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult => _locationDetectResult;
        public System.IObservable<bool> PlaceRandomBeamForSelectedZone => _placeRandomBeamForSelectedZone;
        public System.IObservable<bool> Reset => _reset;
        public System.IObservable<bool> Clear => _clear;
        public IReadOnlyReactiveProperty<int> Coins => _coins;

        public void SetActivePortalZone(PortalZoneModel zoneModel)
        {
            _selectedPortalZone.Value = zoneModel;
        }

        public void SetNearestPortalZone(PortalZoneModel zoneModel)
        {
            _nearestPortalZone.Value = zoneModel;
        }

        public void SetPlayerPosition(Vector2 position)
        {
            _playerLocationChanged.Value = position;
        }

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;
        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public void PlaceRandomBeam()
        {
            _placeRandomBeamForSelectedZone.OnNext(true);
        }

        public void CollectedCoin()
        {
            _playerData.AddCoin();
            _coins.Value = _playerData.GetCoins();
        }

        public IEnumerable<PortalViewInfo> GetAllZones()
        {
            List<PortalViewInfo> portalsList = new();

            foreach (PortalZoneModel portalZoneModel in _assetsScriptableObject.GetZonesList())
            {
                PortalViewInfo viewInfo = portalZoneModel.ToViewInfo();
                viewInfo.Distance = viewInfo.Coordinates.ToHumanReadableDistanceFromPlayer();
                portalsList.Add(viewInfo);
            }

            return portalsList;
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

        public Vector2 GetPlayerPosition() => _playerLocationChanged.Value;
    }
}