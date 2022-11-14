using System;

namespace Screens.Transitions
{
    public interface IScreenTransitionAnimation
    {
        void PerformAnimation(Action onEndCallback = null);
        bool IsPlaying();
        void KillAnim();
    }
}
