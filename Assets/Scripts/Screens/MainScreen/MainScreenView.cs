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
        [SerializeField] private BottomNavigationBar bottomNavigationBar;
        [SerializeField] private Button menuButton;

        [Space] [SerializeField] private MapUserInterfaceView mapUserInterfaceView;
        [Space] [SerializeField] private Image userIcon;
        [SerializeField] private TextMeshProUGUI usernameText;

        public event Action MenuClicked;

        public IMapUserInterface MapUserInterface => mapUserInterfaceView;
        
        public IBottomNavigationBar BottomNavigationBar => bottomNavigationBar;

        public void ConfigureView(MainScreenViewInfo userInfo)
        {
            userIcon.sprite = userInfo.UserIcon;
            usernameText.text = userInfo.Username;
        }

        private void Awake()
        {
            menuButton.ActionWithThrottle(() => MenuClicked?.Invoke());
        }
    }
}