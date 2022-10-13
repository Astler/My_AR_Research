using System.Collections.Generic;
using System.Linq;
using Items;
using Prototype.AR;
using Prototype.Assets;
using Prototype.Core;
using Prototype.Data;
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

        private IPlayerData _playerData;
        private CameraView _cameraView;
        private IARController _arController;
        private bool _isRewardReceived;
        private ReactiveProperty<bool> _mapOpened = new();
        private ReactiveProperty<int> _coins = new();
        public IReadOnlyReactiveProperty<bool> MapOpened => _mapOpened;
        public IReadOnlyReactiveProperty<int> Coins => _coins;

        private void Awake()
        {
            _playerData = new PlayerData();
        }

        private void Start()
        {
            _arController = FindObjectOfType<ARDKController>();

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
            _isRewardReceived = false;
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
    }
}