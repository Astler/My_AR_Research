using System.Collections.Generic;
using AR;
using Data.Objects;
using Geo;
using Screens.PortalsListScreen;
using UniRx;
using UnityEngine;

namespace Data
{
    public interface IDataProxy
    {
        IReadOnlyReactiveProperty<GameStates> GameState { get; }
        IReadOnlyReactiveProperty<bool> MapOpened { get; }
        IReadOnlyReactiveProperty<bool> InRewardZone { get; }
        IReadOnlyReactiveProperty<float> DistanceToClosestReward { get; }
        IReadOnlyReactiveProperty<PortalZoneModel> SelectedPortalZone { get; }
        IReadOnlyReactiveProperty<PortalZoneModel> NearestPortalZone { get; }
        IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged { get; }
        IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult { get; }
        System.IObservable<bool> PlaceRandomBeamForSelectedZone { get; }
        System.IObservable<bool> Reset { get; }
        System.IObservable<bool> Clear { get; }
        IReadOnlyReactiveProperty<int> Coins { get; }

        void SetActivePortalZone(PortalZoneModel zoneModel);
        void SetNearestPortalZone(PortalZoneModel zoneModel);
        void SetPlayerPosition(Vector2 position);
        LocationDetectResult GetLocationDetectResult();
        void SetLocationDetectStatus(LocationDetectResult result);
        void PlaceRandomBeam();
        Vector2 GetPlayerPosition();
        void CollectedCoin();
        IEnumerable<PortalViewInfo> GetAllZones();
        void NextStateStep();
        void ClearScene();
        void ResetScene();
        void RestartGeoLocation();
        void ToggleMap();
    }
}