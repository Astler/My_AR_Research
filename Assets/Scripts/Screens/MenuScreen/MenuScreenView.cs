using System;
using Screens.Transitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.MenuScreen
{
    public interface IMenuScreenView: IScreenView
    {
        event Action ChangeUsernameClicked;
        event Action HelpCenterClicked;
        event Action LegalClicked;
        event Action LogoutClicked;
        
        void ConfigureView(MenuScreenViewInfo viewModel);
    }
    
    public class MenuScreenView : ScreenView, IMenuScreenView
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button background;
        [SerializeField] private Button changeUsernameButton;
        [SerializeField] private Button helpCenterButton;
        [SerializeField] private Button legalButton;
        [SerializeField] private Button logOutButton;
        [SerializeField] private TextMeshProUGUI versionText;
        
        public event Action ChangeUsernameClicked;
        public event Action HelpCenterClicked;
        public event Action LegalClicked;
        public event Action LogoutClicked;

        private void Awake()
        {
            closeButton.onClick.AddListener(CloseScreen);
            background.onClick.AddListener(CloseScreen);
            
            changeUsernameButton.onClick.AddListener(() => ChangeUsernameClicked?.Invoke());
            logOutButton.onClick.AddListener(() => LogoutClicked?.Invoke());
            helpCenterButton.onClick.AddListener(() => HelpCenterClicked?.Invoke());
            legalButton.onClick.AddListener(() => LegalClicked?.Invoke());
            
            SetShowTransitionAnimation(new RightToFrontAnimation(CanvasGroup, false));
            SetHideTransitionAnimation(new RightToFrontAnimation(CanvasGroup, true));
        }

        public void ConfigureView(MenuScreenViewInfo viewModel)
        {
            versionText.text = $"App ver. {viewModel.AppVersion}";
        }
    }
}