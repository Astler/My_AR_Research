using System;
using System.Collections.Generic;
using AR;
using AR.World.Collectable;
using Data.Objects;
using Geo;
using Screens.MainScreen;
using UniRx;
using UnityEngine;
using CameraType = GameCamera.CameraType;

namespace Data
{
    public interface IDataProxy
    {
        IObservable<(BottomBarButtonType type, object data)> BottomNavigationAction { get; }
        IReadOnlyReactiveProperty<GameStates> GameState { get; }
        IReadOnlyReactiveProperty<CameraType> ActiveCameraType { get; }
        IReadOnlyReactiveProperty<int> SelectedOnMapDropZoneId { get; }
        IReadOnlyReactiveCollection<HistoryStepData> SessionHistory { get; }
        IReadOnlyReactiveCollection<ICollectable> AvailableCollectables { get; }
        
        // IReadOnlyReactiveProperty<DropZoneViewInfo> SelectedPortalZone { get; }
        IReadOnlyReactiveProperty<DropZoneViewInfo> EnteredPortalZone { get; }
        IReadOnlyReactiveProperty<DropZoneViewInfo> NearestPortalZone { get; }
        
        void SetSelectedOnMapDropZone(int id);
        void CompleteStateStep(GameStates states);
        void SetActiveCamera(CameraType type);
        void SetUserData(UserData responseUser);
        bool IsRequestedAreaScanned();
        MainScreenViewInfo GetUserInfo();
        DropZoneViewInfo GetZoneInfoById(int id);
        void InvokeBottomBarAction(BottomBarButtonType button, object data);

        //TODO rethink OLD

        IReadOnlyReactiveProperty<bool> MapOpened { get; }
        IReadOnlyReactiveProperty<int> AvailableGifts { get; }
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
        bool IsInsideEvent();
        void SetIsMapOpened(bool isMapOpened);
        void AddEvents(EventsData data);
        void TryToCollectBeam(BeamData data, Action<PrizeData> success, Action failed);
        void GetSpriteByUrl(string url, Action<Sprite> action);
        void RefreshCollectedRewards();
        void SetScannedArea(float totalArea);
        void AddToAvailableCollectables(ICollectable collectable);
        void RemoveFromAvailableCollectables(ICollectable collectable);
    }
}