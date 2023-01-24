using System;
using Pointers;
using Screens.RewardClaimedScreen;
using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private DropLocationDirectionPointer pointer;

        public IDropLocationDirectionPointer DirectionPointer => pointer;
        
        //OLD
        
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button collectedRewardsButton;
        [SerializeField] private Button historyButton;
        [SerializeField] private TextMeshProUGUI availableGiftsText;
        [SerializeField] private TextMeshProUGUI nextGiftTimerText;
        [SerializeField] private LocationInfoView locationInfoView;
        [SerializeField] private PlayerBalanceBarView playerBalancesView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        [SerializeField] private RewardClaimedScreenView rewardClaimedScreenView;
        [SerializeField] private RewardClaimedScreenView rewardAlreadyClaimedScreenView;
        
        public event Action OpenMapClicked;
        public event Action CollectedRewardsClicked;
        public event Action HistoryClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;
        
        public void SetIsMapActive(bool isMapActive)
        {
            clearButton.gameObject.SetActive(!isMapActive);
            restartButton.gameObject.SetActive(!isMapActive);
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
            locationInfoView.gameObject.SetActive(false);
            playerBalancesView.gameObject.SetActive(false);
            mapUserInterfaceView.gameObject.SetActive(false);
        }

        public void ShowBaseInterface()
        {
            playerBalancesView.gameObject.SetActive(true);
        }
        
        public void ShowLocationInfo()
        {
            locationInfoView.gameObject.SetActive(true);
        }

        public void ShowGameInterface()
        {
            openMapButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            locationInfoView.gameObject.SetActive(true);
            playerBalancesView.gameObject.SetActive(true);
        }
        
        public void ShowRewardPopup(Sprite sprite, string itemName) =>
            rewardClaimedScreenView.ShowReward(sprite, itemName);

        public void ShowAlreadyClaimedRewardPopup(Sprite sprite, string itemName) =>
            rewardAlreadyClaimedScreenView.ShowReward(sprite, itemName);
        
        
        private void Awake()
        {
            clearButton.ActionWithThrottle(() => { ClearButtonClicked?.Invoke(); });
            collectedRewardsButton.ActionWithThrottle(() => { CollectedRewardsClicked?.Invoke(); });
            historyButton.ActionWithThrottle(() => { HistoryClicked?.Invoke(); });
            restartButton.ActionWithThrottle(() => { RestartButtonClicked?.Invoke(); });
            openMapButton.ActionWithThrottle(() => { OpenMapClicked?.Invoke(); });
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