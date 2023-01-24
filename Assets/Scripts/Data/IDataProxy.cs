using System;
using System.Collections.Generic;
using AR;
using Data.Objects;
using Geo;
using UniRx;
using UnityEngine;
using CameraType = GameCamera.CameraType;

namespace Data
{
    public interface IDataProxy
    {
        IReadOnlyReactiveProperty<GameStates> GameState { get; }
        IReadOnlyReactiveProperty<CameraType> ActiveCameraType { get; }
        IReadOnlyReactiveProperty<int> SelectedOnMapDropZoneId { get; }
        
        void SetSelectedOnMapDropZone(int id);
        void CompleteStateStep(GameStates states);

        //TODO rethink OLD

        IReadOnlyReactiveProperty<bool> MapOpened { get; }
        IReadOnlyReactiveProperty<int> AvailableGifts { get; }
        IReadOnlyReactiveProperty<ZoneViewInfo> SelectedPortalZone { get; }
        IReadOnlyReactiveProperty<ZoneViewInfo> NearestPortalZone { get; }
        IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged { get; }
        IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult { get; }
        IObservable<ActiveBoxData> PlaceRewardBoxInsideZone { get; }
        IObservable<ActiveBoxData> RemoveRewardBox { get; }
        IObservable<bool> Reset { get; }
        IObservable<bool> Clear { get; }
        IReadOnlyReactiveProperty<int> Coins { get; }
        IReadOnlyReactiveProperty<int> TimeToNextGift { get; }
        IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos { get; }
        IReadOnlyReactiveCollection<HistoryStepData> SessionHistory { get; }
        IReadOnlyReactiveProperty<EventData> ActiveEventData { get; }
        IReadOnlyReactiveProperty<bool> SurfaceScanned { get; }
        IReadOnlyReactiveProperty<float> ScannedArea { get; }

        void LoadClaimedRewards();
        void SetActivePortalZone(ZoneViewInfo zoneModel);
        void SetNearestPortalZone(ZoneViewInfo zoneModel);
        void SetPlayerPosition(Vector2 position);
        LocationDetectResult GetLocationDetectResult();
        void SetLocationDetectStatus(LocationDetectResult result);
        Vector2 GetPlayerPosition();
        void CollectedCoin(int amount = 1);
        IEnumerable<ZoneViewInfo> GetAllActiveZones();
        void ClearScene();
        void ResetScene();
        bool IsInsideEvent();
        void ToggleMap();
        void AddEvents(EventsData data);
        IEnumerable<RewardViewInfo> GetRewardsForActiveZone();
        RewardViewInfo GetAvailableRewardForZone();
        void TryToCollectBeam(BeamData data, Action<Sprite> success, Action failed);
        void GetSpriteByUrl(string url, Action<Sprite> action);
        void RefreshCollectedRewards();
        void SetScannedArea(float totalArea);
    }
}