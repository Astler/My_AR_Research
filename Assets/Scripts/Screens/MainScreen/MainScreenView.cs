using System;
using DG.Tweening;
using Screens.PortalsListScreen;
using Screens.RewardClaimedScreen;
using Screens.RewardsListScreen;
using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button placeRandomBeamButton;
        [SerializeField] private Button warningOkButton;
        [SerializeField] private Button collectedRewardsButton;
        [SerializeField] private GameObject placementParent;
        [SerializeField] private TextMeshProUGUI availableGiftsText;
        [SerializeField] private TextMeshProUGUI nextGiftTimerText;
        [SerializeField] private LocationInfoView locationInfoView;
        [SerializeField] private PlayerBalanceBarView playerBalancesView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        [SerializeField] private RewardClaimedScreenView rewardClaimedScreenView;
        [SerializeField] private RewardClaimedScreenView rewardAlreadyClaimedScreenView;

        [Space, Header("Steps"), SerializeField]
        private CanvasGroup warningStep;

        [SerializeField] private CanvasGroup scanningStep;
        [SerializeField] private CanvasGroup locationSearchScreen;

        [SerializeField] private ZonesListView zonesListView;
        [SerializeField] private Button closeZonesList;
        [SerializeField] private RewardsListView rewardsListView;
        [SerializeField] private Button closeRewardsList;

        public event Action WarningOkClicked;
        public event Action OpenMapClicked;
        public event Action CollectedRewardsClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;

        public void SetIsMapActive(bool isMapActive)
        {
            clearButton.gameObject.SetActive(!isMapActive);
            restartButton.gameObject.SetActive(!isMapActive);
            placeRandomBeamButton.gameObject.SetActive(!isMapActive);
            mapUserInterfaceView.SetIsMapActive(isMapActive);
        }

        public void SetNextGiftTime(int timeToNextGift)
        {
            nextGiftTimerText.gameObject.SetActive(timeToNextGift > 0);
            nextGiftTimerText.text = $"Next gift in: {timeToNextGift}";
        }

        public void SetCoins(int coins)
        {
            playerBalancesView.SetCoins(coins);
        }

        public void SetupActiveZone(string zoneName)
        {
            bool hasZone = zoneName != null;

            availableGiftsText.gameObject.SetActive(hasZone);
            mapUserInterfaceView.SetIsRewardsButtonActive(hasZone);

            locationInfoView.SetActiveZoneName(hasZone
                ? $"<color=green>{zoneName}</color>"
                : "<color=red>Go to the event area!</color>");

            placementParent.SetActive(hasZone);
        }

        public void SetAvailableGifts(int gifts)
        {
            availableGiftsText.text = "Available gifts: " + gifts;
        }

        public void ShowLocationSearchStatus(string status)
        {
            locationInfoView.ShowResponse(status);
        }

        public IMapUserInterface GetMapUserInterface() => mapUserInterfaceView;

        public void HideInterface()
        {
            openMapButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(false);
            placeRandomBeamButton.gameObject.SetActive(false);
            locationInfoView.gameObject.SetActive(false);
            playerBalancesView.gameObject.SetActive(false);
            mapUserInterfaceView.gameObject.SetActive(false);

            warningStep.alpha = 0;
            scanningStep.alpha = 0;
            locationSearchScreen.alpha = 0;
            warningStep.interactable = false;
            scanningStep.interactable = false;
            locationSearchScreen.interactable = false;
            warningStep.blocksRaycasts = false;
            scanningStep.blocksRaycasts = false;
            locationSearchScreen.blocksRaycasts = false;
        }

        public void ShowWarningMessage()
        {
            warningStep.DOKill();
            warningStep.interactable = true;
            warningStep.blocksRaycasts = true;
            warningStep.DOFade(1f, 1f);
        }

        public void ShowBaseInterface()
        {
            playerBalancesView.gameObject.SetActive(true);
        }

        public void HideWarningMessage()
        {
            warningStep.DOKill();
            warningStep.interactable = false;
            warningStep.blocksRaycasts = false;
            warningStep.DOFade(0f, 1f);
        }

        public void ShowLocationDetectionPopup()
        {
            locationInfoView.gameObject.SetActive(true);
            locationSearchScreen.DOKill();
            locationSearchScreen.DOFade(1f, 1f);
        }

        public void HideLocationDetectionPopup()
        {
            locationSearchScreen.DOKill();
            locationSearchScreen.DOFade(0f, 1f);
        }

        public void ShowScanningPopup()
        {
            scanningStep.DOKill();
            scanningStep.DOFade(1f, 1f);
        }

        public void HideScanningPopup()
        {
            scanningStep.DOKill();
            scanningStep.DOFade(0f, 1f);
        }

        public void ShowGameInterface()
        {
            openMapButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            placeRandomBeamButton.gameObject.SetActive(true);
            locationInfoView.gameObject.SetActive(true);
            playerBalancesView.gameObject.SetActive(true);
        }

        public void ShowAllZonesList() => zonesListView.SetActive(true);
        public void HideZonesList() => zonesListView.SetActive(false);

        public void ShowRewardPopup(Sprite sprite, string itemName) =>
            rewardClaimedScreenView.ShowReward(sprite, itemName);

        public void ShowAlreadyClaimedRewardPopup(Sprite sprite, string itemName) =>
            rewardAlreadyClaimedScreenView.ShowReward(sprite, itemName);

        public void ShowRewardsList() => rewardsListView.SetActive(true);

        public void HideRewardsList() => rewardsListView.SetActive(false);

        public IPortalsListScreenView GetZonesListView() => zonesListView;

        public IRewardsListScreenView GetRewardsListView() => rewardsListView;

        private void Awake()
        {
            warningOkButton.ActionWithThrottle(() => WarningOkClicked?.Invoke());
            clearButton.ActionWithThrottle(() => { ClearButtonClicked?.Invoke(); });
            collectedRewardsButton.ActionWithThrottle(() => { CollectedRewardsClicked?.Invoke(); });
            restartButton.ActionWithThrottle(() => { RestartButtonClicked?.Invoke(); });
            openMapButton.ActionWithThrottle(() => { OpenMapClicked?.Invoke(); });
            closeZonesList.ActionWithThrottle(HideZonesList);
            closeRewardsList.ActionWithThrottle(HideRewardsList);
            HideZonesList();
            HideRewardsList();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !Utils.Utils.IsPointerOverUIObject())
            {
                EmptyScreenClicked?.Invoke(Input.mousePosition);
            }
#else
        if (Input.touches.Length > 0 && !Utils.Utils.IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                EmptyScreenClicked?.Invoke(Input.GetTouch(0).position);
            }
        }
#endif
        }
    }
}