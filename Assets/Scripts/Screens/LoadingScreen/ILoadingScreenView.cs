using System;

namespace Screens.LoadingScreen
{
    public interface ILoadingScreenView : IScreenView
    {
        public void SetViewModel(string appVersion);
        public void SetLoadingProgressValue(float progress);
        public event Action OnLoadingAnimationFinish;
    }
}