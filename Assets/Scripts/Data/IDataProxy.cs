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
        IReadOnlyReactiveCollection<HistoryStepData> SessionHistory { get; }
        
        void SetSelectedOnMapDropZone(int id);
        void CompleteStateStep(GameStates states);
        void SetActiveCamera(CameraType type);
        bool IsRequestedAreaScanned();

        //TODO rethink OLD

        IReadOnlyReactiveProperty<bool> MapOpened { get; }
        IReadOnlyReactiveProperty<int> AvailableGifts { get; }
        IReadOnlyReactiveProperty<DropZoneViewInfo> SelectedPortalZone { get; }
        IReadOnlyReactiveProperty<DropZoneViewInfo> NearestPortalZone { get; }
        IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged { get; }
        IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult { get; }
        IObservable<ActiveBoxData> PlaceRewardBoxInsideZone { get; }
        IObservable<ActiveBoxData> RemoveRewardBox { get; }
        IObservable<bool> Reset { get; }
        IObservable<bool> Clear { get; }
        IReadOnlyReactiveProperty<int> TimeToNextGift { get; }
        IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos { get; }
        IReadOnlyReactiveProperty<EventData> ActiveEventData { get; }
        IReadOnlyReactiveProperty<float> ScannedArea { get; }

        void LoadClaimedRewards();
        void SetActivePortalZone(DropZoneViewInfo dropZoneModel);
        void SetNearestPortalZone(DropZoneViewInfo dropZoneModel);
        void SetPlayerPosition(Vector2 position);
        LocationDetectResult GetLocationDetectResult();
        void SetLocationDetectStatus(LocationDetectResult result);
        Vector2 GetPlayerPosition();
        IEnumerable<DropZoneViewInfo> GetAllActiveZones();
        void ClearScene();
        void ResetScene();
        bool IsInsideEvent();
        void SetIsMapOpened(bool isMapOpened);
        void AddEvents(EventsData data);
        IEnumerable<RewardViewInfo> GetRewardsForActiveZone();
        RewardViewInfo GetAvailableRewardForZone();
        void TryToCollectBeam(BeamData data, Action<Sprite> success, Action failed);
        void GetSpriteByUrl(string url, Action<Sprite> action);
        void RefreshCollectedRewards();
        void SetScannedArea(float totalArea);
    }
}