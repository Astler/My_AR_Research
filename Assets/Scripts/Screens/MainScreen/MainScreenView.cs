using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private BottomNavigationBar bottomNavigationBar;
        [Space] [SerializeField] private RawImage pointerImage;
        
        [Space] [SerializeField] private TextMeshProUGUI availableGiftsText;
        [SerializeField] private TextMeshProUGUI nextGiftTimerText;
        [SerializeField] private LocationInfoView locationInfoView;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        
        public IMapUserInterface MapUserInterface => mapUserInterfaceView;
        public IBottomNavigationBar BottomNavigationBar => bottomNavigationBar;

        public void SetUIFlags(MainScreenMode mainScreenUI)
        {
            bool uiHidden = mainScreenUI.HasFlag(MainScreenMode.Hide);
            bool isMap = mainScreenUI.HasFlag(MainScreenMode.Map);
            bool topBarActive = mainScreenUI.HasFlag(MainScreenMode.TopBar);
            bool contentVisible = mainScreenUI.HasFlag(MainScreenMode.MidContent);
            bool bottomBarActive = mainScreenUI.HasFlag(MainScreenMode.BottomBar);

            locationInfoView.gameObject.SetActive(!uiHidden && contentVisible);
            pointerImage.gameObject.SetActive(!uiHidden && !isMap && contentVisible);
        }

        public void SetNextGiftTime(int timeToNextGift)
        {
            nextGiftTimerText.gameObject.SetActive(timeToNextGift > 0);
            nextGiftTimerText.text = $"Next gift in: {timeToNextGift}";
        }

        public void SetupActiveZone(string zoneName)
        {
            bool hasZone = zoneName != null;

            availableGiftsText.gameObject.SetActive(hasZone);
            
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
    }
}