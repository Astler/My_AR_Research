using System;
using System.Collections.Generic;
using System.Linq;
using AR;
using AR.World.Collectable;
using Assets;
using BestHTTP.WebSocket;
using Core;
using Core.WebSockets;
using Data.Objects;
using Geo;
using Mapbox.Utils;
using Screens.MainScreen;
using UniRx;
using UnityEngine;
using Utils;
using CameraType = GameCamera.CameraType;
using Random = UnityEngine.Random;

namespace Data
{
    public class DataProxy : IDataProxy
    {
        private readonly IApiInterface _apiInterface;
        private readonly IWebSocketService _webSocketService;
        private readonly GameAssets _gameAssets;
        private EventData[] _eventsData;

        private readonly ReactiveProperty<CameraType> _activeCameraType = new(CameraType.ArCamera);
        private readonly ReactiveProperty<int> _selectedOnMapDropZoneId = new(-1);
        private readonly ReactiveProperty<GameStates> _gameState = new(GameStates.Loading);
        private readonly ReactiveCollection<HistoryStepData> _historyLines = new();
        private readonly ReactiveCollection<ICollectable> _collectables = new();
        private readonly ReactiveCollection<DropZoneViewInfo> _dropZones = new();
        private readonly ReactiveCollection<ActiveBoxData> _activeBoxes = new();

        private readonly ReactiveProperty<float> _scannedArea = new();

        private readonly Subject<(BottomBarButtonType type, object data)> _bottomNavigationAction = new();
        private readonly ReactiveProperty<int> _availableGifts = new();
        private readonly ReactiveProperty<bool> _mapOpened = new();
        private readonly ReactiveProperty<bool> _hasNewCollectedDrops = new();
        private readonly ReactiveProperty<EventData> _activeEventData = new();
        private readonly ReactiveProperty<Vector2> _playerLocationChanged = new();
        private readonly ReactiveProperty<LocationDetectResult> _locationDetectResult = new();
        private readonly ReactiveProperty<int> _timeToNextGift = new();

        private readonly List<PrizeData> _collectedPrizes = new();
        private readonly ReactiveCollection<RewardViewInfo> _collectedPrizesInfos = new();
        private UserData _responseUser;
        private readonly EditorAssets _editorAssets;
        private IDisposable _eventsReloadTimer;

        public DataProxy(IApiInterface apiInterface, IWebSocketService webSocketService, GameAssets gameAssets,
            EditorAssets editorAssets)
        {
            _editorAssets = editorAssets;
            _apiInterface = apiInterface;
            _webSocketService = webSocketService;
            _gameAssets = gameAssets;
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
                _activeBoxes.Add(boxData);

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
                ActiveBoxData box = _activeBoxes.FirstOrDefault(x => x.id == boxData.id);

                if (box != null)
                {
                    _activeBoxes.Remove(boxData);
                }
            }
            else if (type == IncomingMessagesTypes.prize_claimed.ToString())
            {
                ActiveBoxData boxData = JsonUtility.FromJson<ActiveBoxData>(message.GetData());

                ActiveBoxData box = _activeBoxes.FirstOrDefault(x => x.id == boxData.id);

                if (box != null)
                {
                    _activeBoxes.Remove(boxData);
                }

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
        public IReadOnlyReactiveCollection<ICollectable> AvailableCollectables => _collectables;
        public IReadOnlyReactiveCollection<DropZoneViewInfo> DropZones => _dropZones;
        public IReadOnlyReactiveCollection<ActiveBoxData> ActiveBoxes => _activeBoxes;

        public IReadOnlyReactiveCollection<RewardViewInfo> CollectedPrizesInfos => _collectedPrizesInfos;
        public IReadOnlyReactiveProperty<int> AvailableGifts => _availableGifts;


        #region Collectables

        public void AddToAvailableToCollectDrops(ICollectable collectable)
        {
            if (_collectables.Contains(collectable)) return;
            _collectables.Add(collectable);
        }

        public void RemoveFromAvailableToCollectDrops(ICollectable collectable) => _collectables.Remove(collectable);

        public void ClearAvailableToCollectDrops() => _collectables.Clear();

        #endregion

        #region Available Boxes

        public void RemoveFromAvailableBoxes(int id)
        {
            ActiveBoxData box = _activeBoxes.FirstOrDefault(it => it.id == id);

            if (box == null) return;

            _activeBoxes.Remove(box);
        }

        #endregion

        #region Area Scanning

        public void SetScannedArea(float totalArea) => _scannedArea.Value = totalArea / AreaScanRequirements;

        public bool IsRequestedAreaScanned() => _scannedArea.Value >= 1;

        #endregion

        #region My Drops Notification

        public void ClearNewDropNotification()
        {
            _hasNewCollectedDrops.Value = false;
        }

        public void SetNewDropNotification(bool succeed)
        {
            _hasNewCollectedDrops.Value = succeed || _hasNewCollectedDrops.Value;
        }

        #endregion

        #region User Info

        public MainScreenViewInfo GetUserInfo() => new()
        {
            Username = _responseUser?.username ?? "User",
            UserIcon = _gameAssets.GetUserIconById(_responseUser?.id ?? Random.Range(0, 1000))
        };

        public void SetUserData(UserData responseUser) => _responseUser = responseUser;

        #endregion

        public DropZoneViewInfo GetZoneInfoById(int id)
        {
            return _dropZones.FirstOrDefault(it => it.Id == id);
        }

        public void InvokeBottomBarAction(BottomBarButtonType button, object data)
        {
            _bottomNavigationAction.OnNext((button, data));
        }

        public void CompleteStateStep(GameStates states)
        {
            if (_gameState.Value == states)
            {
                _gameState.Value = _gameState.Value.Next();
            }
        }

        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<bool> HasNewCollectedDrops => _hasNewCollectedDrops;
        public System.IObservable<(BottomBarButtonType, object)> BottomNavigationAction => _bottomNavigationAction;
        public IReadOnlyReactiveProperty<GameStates> GameState => _gameState;
        public IReadOnlyReactiveProperty<EventData> ActiveEventData => _activeEventData;
        public IReadOnlyReactiveProperty<Vector2> PlayerLocationChanged => _playerLocationChanged;
        public IReadOnlyReactiveProperty<LocationDetectResult> LocationDetectResult => _locationDetectResult;
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
                _activeEventData.Value = null;
                return;
            }

            if (dropZoneModel.Id == (_activeEventData.Value?.id ?? 0)) return;

            _activeBoxes.Clear();

            _apiInterface.ShowEventData(dropZoneModel.Id, data =>
            {
                if (data.event_data == null)
                {
                    _activeEventData.Value = null;
                    return;
                }

                _activeEventData.Value = data.event_data;

                foreach (ActiveBoxData activeBoxData in data.event_data.active_boxes)
                {
                    _activeBoxes.Add(activeBoxData);
                }

                _availableGifts.Value = data.event_data.prizes.Count(it => !it.is_claimed);

                _webSocketService.SubscribeToEventSessionChannel(data.event_data.id);
            }, Debug.LogError);
        }

        public void SetPlayerPosition(Vector2 position) => _playerLocationChanged.SetValueAndForceNotify(position);

        public LocationDetectResult GetLocationDetectResult() => _locationDetectResult.Value;

        public void SetLocationDetectStatus(LocationDetectResult result) => _locationDetectResult.Value = result;

        public IEnumerable<DropZoneViewInfo> GetAllActiveZones()
        {
            List<DropZoneViewInfo> activeZones = _dropZones.Where(it => it.IsActive()).ToList();

            Vector2 playerPosition = GetPlayerPosition();

            foreach (DropZoneViewInfo portalViewInfo in activeZones)
            {
                portalViewInfo.OrderDistance = CoordinatesUtils.Distance(playerPosition.x,
                    playerPosition.y,
                    portalViewInfo.Coordinates.x,
                    portalViewInfo.Coordinates.y);
            }

            return activeZones.OrderBy(it => it.OrderDistance);
        }

        public bool IsInsideEvent() => _activeEventData.Value != null;

        public void SetIsMapOpened(bool isMapOpened)
        {
            _mapOpened.Value = isMapOpened;
            SetActiveCamera(MapOpened.Value ? CameraType.MapCamera : CameraType.Disabled);
        }

        public void SetActiveCamera(CameraType type)
        {
            _activeCameraType.Value = type;
        }

        private void LoadEvents() => _apiInterface.GetEventsList(AddEvents, Debug.LogError);

        public void LoadClaimedRewards()
        {
            if (_editorAssets.mockStartData) return;
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
            ClearAvailableToCollectDrops();
            _eventsReloadTimer?.Dispose();
            _dropZones.Clear();

            _eventsData = data.events;

            long time = DateTimeOffset.Now.ToUnixTimeSeconds();
            long nearestFinishTime = 0;

            foreach (EventData eventData in _eventsData)
            {
                if (eventData.finish_time < time)
                    continue;

                long timeToFinish = eventData.finish_time - time;

                if (nearestFinishTime == 0 || timeToFinish < nearestFinishTime)
                {
                    nearestFinishTime = timeToFinish;
                }

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

                _dropZones.Add(viewInfo);
            }

            if (nearestFinishTime > 0)
            {
                _eventsReloadTimer = Observable.Timer(TimeSpan.FromSeconds(nearestFinishTime))
                    .Subscribe(_ => LoadEvents());
            }
        }

        public void TryToCollectBeam(int id, Action<PrizeData> success, Action failed)
        {
            _apiInterface.CollectReward(_activeEventData.Value?.id ?? 0, id,
                result =>
                {
                    LoadEvents();
                    success?.Invoke(result.prize);
                },
                error =>
                {
                    LoadEvents();
                    Debug.LogError(error);
                    failed?.Invoke();
                });
        }

        public void RefreshCollectedRewards() => LoadClaimedRewards();

        private const float AreaScanRequirements = 100;

        public Vector2 GetPlayerPosition()
        {
            return Application.isEditor ? GlobalConstants.MockPosition : _playerLocationChanged.Value;
        }
    }
}