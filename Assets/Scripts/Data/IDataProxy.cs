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
        IReadOnlyReactiveCollection<DropZoneViewInfo> DropZones { get; }
        IReadOnlyReactiveCollection<ActiveBoxData> ActiveBoxes { get; }
        IReadOnlyReactiveProperty<bool> HasNewCollectedDrops { get; }

        IReadOnlyReactiveProperty<EventData> ActiveEventData { get; }

        void ClearNewDropNotification();
        void SetNewDropNotification(bool succeed);
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
        IReadOnlyReactiveProperty<int> TimeToNextGift { get; }
        IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos { get; }
        IReadOnlyReactiveProperty<float> ScannedArea { get; }

        void LoadClaimedRewards();
        void SetActivePortalZone(DropZoneViewInfo dropZoneModel);
        void SetPlayerPosition(Vector2 position);
        LocationDetectResult GetLocationDetectResult();
        void SetLocationDetectStatus(LocationDetectResult result);
        Vector2 GetPlayerPosition();
        IEnumerable<DropZoneViewInfo> GetAllActiveZones();
        bool IsInsideEvent();
        void SetIsMapOpened(bool isMapOpened);
        void AddEvents(EventsData data);
        void TryToCollectBeam(int id, Action<PrizeData> success, Action failed);
        void RefreshCollectedRewards();
        void SetScannedArea(float totalArea);
        void AddToAvailableToCollectDrops(ICollectable collectable);
        void RemoveFromAvailableToCollectDrops(ICollectable collectable);
        void RemoveFromAvailableBoxes(int id);
    }
}