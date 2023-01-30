using Screens.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.MainScreen
{
    public class MainScreenView : ScreenView, IMainScreenView
    {
        [SerializeField] private BottomNavigationBar bottomNavigationBar;

        [Space] [SerializeField] private TextMeshProUGUI availableGiftsText;
        [SerializeField] private TextMeshProUGUI nextGiftTimerText;
        [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        [Space] [SerializeField] private Image userIcon;
        [SerializeField] private TextMeshProUGUI usernameText;

        public IMapUserInterface MapUserInterface => mapUserInterfaceView;
        public IBottomNavigationBar BottomNavigationBar => bottomNavigationBar;

        public void SetNextGiftTime(int timeToNextGift)
        {
            nextGiftTimerText.gameObject.SetActive(timeToNextGift > 0);
            nextGiftTimerText.text = $"Next gift in: {timeToNextGift}";
        }

        public void SetAvailableRewards(int rewards)
        {
            availableGiftsText.text = "Available gifts: " + rewards;
        }

        public void ConfigureView(MainScreenViewInfo userInfo)
        {
            userIcon.sprite = userInfo.UserIcon;
            usernameText.text = userInfo.Username;
        }
    }
}