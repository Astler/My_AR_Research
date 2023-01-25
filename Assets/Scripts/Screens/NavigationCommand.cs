namespace Screens
{
    public class NavigationCommand
    {
        public ScreenName NextScreenName => _nextScreenName;
        public ScreenView ScreenToClose => _screenToClose;

        public object ExtraData => _extraData;

        public bool IsWithAnimation => _withAnimation;

        public bool IsDelayed
        {
            get => _isDelayed;
            set => _isDelayed = value;
        }

        public bool IsCloseCurrentScreen => _isCloseCurrentScreen;
        public bool IsCloseAllScreens => _isCloseAllScreens;

        public bool CanReturnToPreviousScreen => _canReturnToPreviousScreen;

        public bool ShouldCloseAfterNextScreenShown => _closeAfterNextScreenShown;
        
        public bool ShouldCloseOtherTabs => _closeOtherTabs;

        private object _extraData;
        private bool _withAnimation = true;
        private bool _isDelayed;
        private bool _isCloseCurrentScreen;
        private bool _isCloseAllScreens;
        private ScreenName _nextScreenName;
        private ScreenView _screenToClose;
        private bool _canReturnToPreviousScreen;
        private bool _closeAfterNextScreenShown;
        private bool _closeOtherTabs;

        public NavigationCommand CloseCurrentScreen()
        {
            _isCloseCurrentScreen = true;
            _screenToClose = null;
            return this;
        }

        public NavigationCommand CloseAllScreens()
        {
            _isCloseAllScreens = true;
            return this;
        }

        public NavigationCommand ShowNextScreen(ScreenName screenName)
        {
            _nextScreenName = screenName;
            return this;
        }

        public NavigationCommand WithExtraData(object data)
        {
            _extraData = data;
            return this;
        }

        public NavigationCommand WithoutAnimation()
        {
            _withAnimation = false;
            return this;
        }

        public NavigationCommand WithAnimation()
        {
            _withAnimation = true;
            return this;
        }

        public NavigationCommand DelayedShow()
        {
            _isDelayed = true;
            return this;
        }

        public NavigationCommand CloseScreen(ScreenView screenView)
        {
            _screenToClose = screenView;
            _isCloseCurrentScreen = false;
            return this;
        }

        public bool IsNextScreenInQueue()
        {
            return !string.IsNullOrEmpty(_nextScreenName.ToString());
        }

        public NavigationCommand CanReturnToPrevious()
        {
            _canReturnToPreviousScreen = true;
            return this;
        }

        public NavigationCommand CloseAfterNextScreenShown()
        {
            _closeAfterNextScreenShown = true;
            return this;
        }

        public NavigationCommand CloseOtherTabs()
        {
            _closeOtherTabs = true;
            return this;
        }
    }
}