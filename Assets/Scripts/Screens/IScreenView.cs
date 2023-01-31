using System;
using System.Collections.Generic;
using Screens.Transitions;
using UnityEngine;

namespace Screens
{
    public interface IScreenView
    {
        event Action<object> OnShowCallback;
        event Action OnGotFocusCallback;
        event Action OnLostFocusCallback;
        event Action ClosedScreen;
        void SetShowTransitionAnimation(IScreenTransitionAnimation animation);
        void SetHideTransitionAnimation(IScreenTransitionAnimation animation);
        void CloseScreen();
        CanvasGroup CanvasGroup { get; }
        bool IsFocused { get; }
        List<CanvasGroup> InfoBlocks { get; }
    }
}