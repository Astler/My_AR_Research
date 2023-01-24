using System;
using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private RawImage pointerImage;
        [SerializeField] private Button openMapButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button collectedRewardsButton;
        [SerializeField] private Button historyButton;

        [Space] [SerializeField] private TextMeshProUGUI availableGiftsText;
        [SerializeField] private TextMeshProUGUI nextGiftTimerText;
        [SerializeField] private LocationInfoView locationInfoView;
        [SerializeField] private PlayerBalanceBarView playerBalancesView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;

        public event Action OpenMapClicked;
        public event Action CollectedRewardsClicked;
        public event Action HistoryClicked;
        public event Action ClearButtonClicked;
        public event Action RestartButtonClicked;
        public event Action<Vector2> EmptyScreenClicked;

        public IMapUserInterface MapUserInterface => mapUserInterfaceView;

        public void SetUIFlags(MainScreenMode mainScreenUI)
        {
            bool uiHidden = mainScreenUI.HasFlag(MainScreenMode.Hide);
            bool isMap = mainScreenUI.HasFlag(MainScreenMode.Map);
            bool topBarActive = mainScreenUI.HasFlag(MainScreenMode.TopBar);
            bool contentVisible = mainScreenUI.HasFlag(MainScreenMode.MidContent);
            bool bottomBarActive = mainScreenUI.HasFlag(MainScreenMode.BottomBar);

            playerBalancesView.gameObject.SetActive(!uiHidden && topBarActive);
            locationInfoView.gameObject.SetActive(!uiHidden && contentVisible);
            pointerImage.gameObject.SetActive(!uiHidden && !isMap && contentVisible);
            openMapButton.gameObject.SetActive(!uiHidden && bottomBarActive);
            clearButton.gameObject.SetActive(!uiHidden && !isMap && bottomBarActive);
            restartButton.gameObject.SetActive(!uiHidden && !isMap && bottomBarActive);

            mapUserInterfaceView.SetActive(!uiHidden && isMap);
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

        public void SetAvailableRewards(int rewards)
        {
            availableGiftsText.text = "Available gifts: " + rewards;
        }

        public void ShowLocationSearchStatus(string status)
        {
            locationInfoView.ShowResponse(status);
        }

        public IMapUserInterface GetMapUserInterface() => mapUserInterfaceView;

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