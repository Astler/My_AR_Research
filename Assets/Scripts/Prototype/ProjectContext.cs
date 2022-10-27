using System.Collections.Generic;
using System.Linq;
using Items;
using Niantic.ARDK.AR.Anchors;
using Niantic.ARDK.Configuration;
using Prototype.AR;
using Prototype.AR.FoundationAR;
using Prototype.Assets;
using Prototype.Core;
using Prototype.Data;
using Prototype.Location;
using Prototype.Screens.MainScreen;
using Prototype.World;
using UniRx;
using UnityEngine;

namespace Prototype
{
    public class ProjectContext : MonoBehaviour
    {
        [SerializeField] private AssetsScriptableObject assetsScriptableObject;

        [Space, SerializeField] private PortalController portalController;
        [SerializeField] private GiftsController giftsController;
        [SerializeField] private CoinsController coinsController;
        [SerializeField] private LocationController locationController;
        [SerializeField] private ScreensInstaller screensInstaller;

        private List<PortalZoneModel> _customZones = new();
        private IPlayerData _playerData;
        private CameraView _cameraView;
        private IARController _arController;
        private ReactiveProperty<bool> _mapOpened = new();
        private ReactiveProperty<int> _coins = new();
        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<int> Coins => _coins;

        public IARAnchor AddAnchor(Vector2 position, Quaternion rotation = default)
        {
            return _arController.AddAnchor(position, rotation);
        }

        public void ClearAnchors()
        {
            _arController.ClearAnchors();
        }

        private void Awake()
        {
            ArdkGlobalConfig.SetUserIdOnLogin(Application.identifier);

            _playerData = new PlayerData();
            _customZones = PlayerPrefsHelper.CustomZonesData.Length == 0
                ? new List<PortalZoneModel>()
                : JsonUtility.FromJson<List<PortalZoneModel>>(PlayerPrefsHelper.CustomZonesData);

            _arController = FindObjectOfType<ARDKController>();
        }

        private void Start()
        {
            if (_arController == null) return;

            _cameraView = _arController.GetCamera();

            screensInstaller.MainScreenPresenter.ConfigureAction(new MainSceneHUDViewInfo
            {
                ClearButtonOnClick = Clear,
                RestartButtonOnClick = Restart,
                SpawnPortalButtonOnClick = SpawnPortalWithRewards,
                OnScreenClick = OnScreenClicked,
                OnOpenMapClick = OnMapClicked
            });

            coinsController.CollectedCoin += OnCollectedCoin;

            _coins.Value = _playerData.GetCoins();
        }

        private void OnMapClicked()
        {
            _mapOpened.Value = !_mapOpened.Value;
        }

        private void OnDestroy()
        {
            coinsController.CollectedCoin -= OnCollectedCoin;
        }

        private void OnCollectedCoin()
        {
            _playerData.AddCoin();
            _coins.Value = _playerData.GetCoins();
        }

        private void OnScreenClicked(Vector2 clickPosition)
        {
            if (_mapOpened.Value) return;

            RaycastHit[] hits = _cameraView.GetHitsByMousePosition(clickPosition);

            if (hits.Length > 0)
            {
                Debug.Log("Unity Hit");
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.TryGetComponent(out GiftView gift))
                    {
                        giftsController.Collect(gift);
                        return;
                    }
                }
            }

            (bool hasHits, Pose? poseTransform) = _arController.CheckIfRaycastHits(clickPosition);

            if (hasHits)
            {
                Debug.Log("AR Hit");
                Pose nonNullPose = poseTransform.GetValueOrDefault();
                giftsController.SpawnNewGift(nonNullPose.position, nonNullPose.rotation);
            }

            Debug.Log("No hits. Clicked nowhere!!");
        }

        private void SpawnPortalWithRewards()
        {
            if (locationController.SelectedPortalZone.Value == null) return;

            portalController.OpenPortalInPosition(_arController.GetPointerPosition(), _arController.GetCeilPosition());
        }

        private void Restart()
        {
            Clear();
            _arController.Reset();
        }

        private void Clear()
        {
            portalController.ClosePortal();
            giftsController.DeleteAllGifts();
        }

        public AssetsScriptableObject GetAssets() => assetsScriptableObject;

        public Vector3 GetCameraForwardDirection() => _cameraView.CameraForwardVector;

        public CoinsController GetCoinsController() => coinsController;

        public Vector3 GetPlayerFacePosition() => _cameraView.GetFacePosition();

        public List<PortalZoneModel> GetPortalPoints() => assetsScriptableObject.portalZones.ToList();

        public LocationController GetLocationController() => locationController;

        public ScreensInstaller GetScreensInstaller() => screensInstaller;

        public List<PortalViewInfo> GetAllPortals()
        {
            List<PortalViewInfo> portalsList = new();

            IEnumerable<PortalZoneModel> allZones = _customZones.Concat(GetAssets().portalZones);

            foreach (PortalZoneModel portalZoneModel in allZones.Where(it => it.isActive))
            {
                PortalViewInfo viewInfo = portalZoneModel.ToViewInfo();

                viewInfo.Distance = viewInfo.Coordinates.ToHumanReadableDistanceFromPlayer();

                portalsList.Add(viewInfo);
            }

            return portalsList;
        }

        public void AddCustomZone(PortalZoneModel newZone)
        {
            _customZones.Add(newZone);
            PlayerPrefsHelper.CustomZonesData = JsonUtility.ToJson(_customZones);
        }

        public IARController GetARController() => _arController;
    }
}