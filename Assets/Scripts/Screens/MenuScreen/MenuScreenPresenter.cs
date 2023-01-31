using System;
using Toasts;
using UnityEngine;
using Utils;

namespace Screens.MenuScreen
{
    public class MenuScreenPresenter
    {
        private readonly IMenuScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private readonly IToastsController _toastsController;
        private IDisposable _rewardsListener;

        public MenuScreenPresenter(IMenuScreenView view, IScreenNavigationSystem screenNavigationSystem,
            IToastsController toastsController)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _toastsController = toastsController;
            Initialize();
        }

        private void Initialize()
        {
            _view.OnShowCallback += Show;

            _view.ChangeUsernameClicked += () =>
            {
                _toastsController.ShowToast(new ToastViewInfo
                {
                    Text = "^change_username_not_available",
                    Duration = 1
                });
            };

            _view.LogoutClicked += () =>
            {
                _toastsController.ShowToast(new ToastViewInfo
                {
                    Text = "^logout_not_available",
                    Duration = 1
                });
            };

            _view.LegalClicked += () => { Application.OpenURL(StringConstants.TermslURL); };
            _view.HelpCenterClicked += () => { Application.OpenURL(StringConstants.TermslURL); };
        }

        private void Show(object data) { }
    }
}